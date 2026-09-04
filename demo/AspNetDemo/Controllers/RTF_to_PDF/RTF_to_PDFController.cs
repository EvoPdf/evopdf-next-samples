using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.RTF_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.RTF_to_PDF
{
    public class RTF_to_PDFController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public RTF_to_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConvertRtfToPdf(RTF_to_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create an RTF to PDF converter object with default settings
            RtfToPdfConverter rtfToPdfConverter = new RtfToPdfConverter();

            // Set whether a table of contents is automatically generated from headings
            rtfToPdfConverter.PdfDocumentOptions.GenerateTableOfContents = model.GenerateToc;

            // Set PDF page size which can be a predefined size like A4 or a custom size in points 
            // Leave it not set to have a default A4 PDF page
            rtfToPdfConverter.PdfDocumentOptions.PdfPageSize = SelectedPdfPageSize(model.PdfPageSize);

            // Set PDF page orientation to Portrait or Landscape
            // Leave it not set to have a default Portrait orientation for PDF page
            rtfToPdfConverter.PdfDocumentOptions.PdfPageOrientation = SelectedPdfPageOrientation(model.PdfPageOrientation);

            // Set PDF page margins in points or leave them not set to have a PDF page without margins
            rtfToPdfConverter.PdfDocumentOptions.LeftMargin = model.LeftMargin;
            rtfToPdfConverter.PdfDocumentOptions.RightMargin = model.RightMargin;
            rtfToPdfConverter.PdfDocumentOptions.TopMargin = model.TopMargin;
            rtfToPdfConverter.PdfDocumentOptions.BottomMargin = model.BottomMargin;

            // Set the RTF viewer zoom percentage
            rtfToPdfConverter.PdfDocumentOptions.Zoom = model.RtfViewerZoom;

            // Set PDF header and footer
            SetHeader(rtfToPdfConverter, model);
            SetFooter(rtfToPdfConverter, model);

            byte[] inputRtfBytes = null;

            // If an uploaded file exists, use it with priority
            if (model.RtfFile != null && model.RtfFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await model.RtfFile.CopyToAsync(ms);
                    inputRtfBytes = ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded RTF file", ex);
                }
            }
            else
            {
                // Otherwise, fall back to the URL
                string rtfUrl = model.RtfFileUrl?.Trim();
                if (string.IsNullOrWhiteSpace(rtfUrl))
                    throw new Exception("No RTF file provided: upload a file or specify a URL");

                try
                {
                    if (rtfUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                    {
                        string localPath = new Uri(rtfUrl).LocalPath;
                        inputRtfBytes = await System.IO.File.ReadAllBytesAsync(localPath);
                    }
                    else
                    {
                        using var httpClient = new System.Net.Http.HttpClient();
                        inputRtfBytes = await httpClient.GetByteArrayAsync(rtfUrl);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not download the RTF file from URL", ex);
                }
            }

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = rtfToPdfConverter.ConvertToPdf(inputRtfBytes);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            // send as attachment
            fileResult.FileDownloadName = "RTF_to_Pdf.pdf";

            return fileResult;
        }

        private void SetHeader(RtfToPdfConverter rtfToPdfConverter, RTF_to_PDF_ViewModel model)
        {
            bool headerEnabled = model.HeaderEnabled;
            if (!headerEnabled)
                return;

            // Set the header HTML from a URL or from an HTML string
            bool headerHtmlFromUrl = model.HeaderHtmlSource == "Url";
            if (headerHtmlFromUrl)
            {
                string headerUrl = model.HeaderUrlTextBox;

                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlSourceUrl = headerUrl;
            }
            else
            {
                string headerHtml = model.HeaderHtmlTextBox;
                string headerHtmlBaseUrl = model.HeaderHtmlBaseUrlTextBox;

                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Html = headerHtml;
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlBaseUrl = headerHtmlBaseUrl;
            }

            // Enable automatic height adjustment based on header HTML content
            bool autoSizeHeaderContentHeight = model.AutoSizeHeaderContentHeight;
            rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoSizeContentHeight = autoSizeHeaderContentHeight;

            // Set the minimum and maximum content height used when AutoSizeContentHeight is enabled
            rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.MinContentHeight = model.HeaderMinContentHeight;
            rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.MaxContentHeight = model.HeaderMaxContentHeight;

            // Set a fixed height for the header if AutoResizeHeight is disabled
            if (model.HeaderHeight.HasValue)
            {
                int headerHeight = model.HeaderHeight.Value;
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Height = headerHeight;
            }

            // If AutoResizeHeight is enabled and both Height and FitHeight are set,
            // the content may be scaled down to fit the specified height
            bool fitHeaderHeight = model.FitHeaderHeight;
            rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.FitHeight = fitHeaderHeight;

            // Enable automatic top margin adjustment in the PDF based on the header
            bool autoResizeTopMargin = model.AutoResizeTopMargin;
            rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoResizePdfMargins = autoResizeTopMargin;

            // Set header visibility on specific PDF pages: first page, odd-numbered pages and even-numbered pages
            rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInFirstPage = model.ShowHeaderInFirstPage;
            rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInOddPages = model.ShowHeaderInOddPages;
            rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInEvenPages = model.ShowHeaderInEvenPages;

            // Reserve space for the header on all pages, regardless of visibility
            // If false, the document will be rendered using print styles instead of screen styles
            rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ReserveSpaceAlways = model.ReserveHeaderSpace;

            // Optimize the header rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
            rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.SkipVariablesParsing = model.SkipHeaderVariablesParsing;

            // Optionally set additional time to wait for the asynchronous header HTML content before rendering
            if (model.HeaderConversionDelay.HasValue)
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ConversionDelay = model.HeaderConversionDelay.Value;
        }

        private void SetFooter(RtfToPdfConverter rtfToPdfConverter, RTF_to_PDF_ViewModel model)
        {
            bool footerEnabled = model.FooterEnabled;
            if (footerEnabled)
            {
                // Set the footer HTML from a URL or from an HTML string
                bool footerHtmlFromUrl = model.FooterHtmlSource == "Url";
                if (footerHtmlFromUrl)
                {
                    string footerUrl = model.FooterUrlTextBox;

                    rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlSourceUrl = footerUrl;
                }
                else
                {
                    string footerHtml = model.FooterHtmlTextBox;
                    string footerHtmlBaseUrl = model.FooterHtmlBaseUrlTextBox;

                    rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Html = footerHtml;
                    rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlBaseUrl = footerHtmlBaseUrl;
                }

                // Enable automatic height adjustment based on footer HTML content
                bool autoSizeFooterContentHeight = model.AutoSizeFooterContentHeight;
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoSizeContentHeight = autoSizeFooterContentHeight;

                // Set the minimum and maximum content height used when AutoSizeContentHeight is enabled
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.MinContentHeight = model.FooterMinContentHeight;
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.MaxContentHeight = model.FooterMaxContentHeight;

                // Set a fixed height for the footer if AutoResizeHeight is disabled
                if (model.FooterHeight.HasValue)
                {
                    int footerHeight = model.FooterHeight.Value;
                    rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Height = footerHeight;
                }

                // If AutoResizeHeight is enabled and both Height and FitHeight are set,
                // the content may be scaled down to fit the specified height
                bool fitFooterHeight = model.FitFooterHeight;
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.FitHeight = fitFooterHeight;

                // Enable automatic bottom margin adjustment in the PDF based on the footer
                bool autoResizeBottomMargin = model.AutoResizeBottomMargin;
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoResizePdfMargins = autoResizeBottomMargin;

                // Set footer visibility on specific PDF pages: first page, odd-numbered pages and even-numbered pages
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInFirstPage = model.ShowFooterInFirstPage;
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInOddPages = model.ShowFooterInOddPages;
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInEvenPages = model.ShowFooterInEvenPages;

                // Reserve space for the footer on all pages, regardless of visibility
                // If false, the document will be rendered using print styles instead of screen styles
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ReserveSpaceAlways = model.ReserveFooterSpace;

                // Optimize the footer rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.SkipVariablesParsing = model.SkipFooterVariablesParsing;

                // Optionally set additional time to wait for the asynchronous footer HTML content before rendering
                if (model.FooterConversionDelay.HasValue)
                    rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ConversionDelay = model.FooterConversionDelay.Value;
            }
        }

        private PdfPageSize SelectedPdfPageSize(string selectedValue)
        {
            switch (selectedValue)
            {
                case "A0":
                    return PdfPageSize.A0;
                case "A1":
                    return PdfPageSize.A1;
                case "A10":
                    return PdfPageSize.A10;
                case "A2":
                    return PdfPageSize.A2;
                case "A3":
                    return PdfPageSize.A3;
                case "A4":
                    return PdfPageSize.A4;
                case "A5":
                    return PdfPageSize.A5;
                case "A6":
                    return PdfPageSize.A6;
                case "A7":
                    return PdfPageSize.A7;
                case "A8":
                    return PdfPageSize.A8;
                case "A9":
                    return PdfPageSize.A9;
                case "ArchA":
                    return PdfPageSize.ArchA;
                case "ArchB":
                    return PdfPageSize.ArchB;
                case "ArchC":
                    return PdfPageSize.ArchC;
                case "ArchD":
                    return PdfPageSize.ArchD;
                case "ArchE":
                    return PdfPageSize.ArchE;
                case "B0":
                    return PdfPageSize.B0;
                case "B1":
                    return PdfPageSize.B1;
                case "B2":
                    return PdfPageSize.B2;
                case "B3":
                    return PdfPageSize.B3;
                case "B4":
                    return PdfPageSize.B4;
                case "B5":
                    return PdfPageSize.B5;
                case "Flsa":
                    return PdfPageSize.Flsa;
                case "HalfLetter":
                    return PdfPageSize.HalfLetter;
                case "Ledger":
                    return PdfPageSize.Ledger;
                case "Legal":
                    return PdfPageSize.Legal;
                case "Letter":
                    return PdfPageSize.Letter;
                case "Letter11x17":
                    return PdfPageSize.Letter11x17;
                case "Note":
                    return PdfPageSize.Note;
                default:
                    return PdfPageSize.A4;
            }
        }

        private PdfPageOrientation SelectedPdfPageOrientation(string selectedValue)
        {
            return selectedValue == "Portrait" ? PdfPageOrientation.Portrait : PdfPageOrientation.Landscape;
        }

        private RTF_to_PDF_ViewModel SetViewModel()
        {
            var model = new RTF_to_PDF_ViewModel();

            var contentRootPath = System.IO.Path.Combine(m_hostingEnvironment.ContentRootPath, "wwwroot");

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "RTF_To_PDF".Length);

            model.RtfFileUrl = rootUrl + "/DemoAppFiles/Input/RTF_Files/RTF_Document.rtf";
            model.HeaderHtmlTextBox = System.IO.File.ReadAllText(System.IO.Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Header_HTML.html"));
            model.FooterHtmlTextBox = System.IO.File.ReadAllText(System.IO.Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Footer_HTML.html"));
            model.HeaderHtmlBaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.HeaderUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/Header_HTML.html";
            model.FooterHtmlBaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.FooterUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/Footer_HTML.html";

            return model;
        }
    }
}
