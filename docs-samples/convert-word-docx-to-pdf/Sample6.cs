// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/convert-word-docx-to-pdf.htm
// Documentation page: Convert Word DOCX to PDF

using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.Word_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.Word_to_PDF
{
    public class Word_to_PDFController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Word_to_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConvertWordToPdf(Word_to_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create a Word to PDF converter object with default settings
            WordToPdfConverter wordToPdfConverter = new WordToPdfConverter();

            // Set whether a table of contents is automatically generated from headings
            wordToPdfConverter.PdfDocumentOptions.GenerateTableOfContents = model.GenerateToc;

            //  Set whether to use page settings (size, margins) from the Word document or the custom settings
            wordToPdfConverter.PdfDocumentOptions.UsePageSettingsFromWord = model.PdfPageSettingsMode == "FromWordDocument";

            // Set whether page break marks from Word documents should be processed
            wordToPdfConverter.ProcessPageBreakMarks = model.ProcessWordPageBreakMarks;

            if (!wordToPdfConverter.PdfDocumentOptions.UsePageSettingsFromWord)
            {
                // Set PDF page size which can be a predefined size like A4 or a custom size in points 
                // Leave it not set to have a default A4 PDF page
                wordToPdfConverter.PdfDocumentOptions.PdfPageSize = SelectedPdfPageSize(model.PdfPageSize);

                // Set PDF page orientation to Portrait or Landscape
                // Leave it not set to have a default Portrait orientation for PDF page
                wordToPdfConverter.PdfDocumentOptions.PdfPageOrientation = SelectedPdfPageOrientation(model.PdfPageOrientation);

                // Set PDF page margins in points or leave them not set to have a PDF page without margins
                wordToPdfConverter.PdfDocumentOptions.LeftMargin = model.LeftMargin;
                wordToPdfConverter.PdfDocumentOptions.RightMargin = model.RightMargin;
                wordToPdfConverter.PdfDocumentOptions.TopMargin = model.TopMargin;
                wordToPdfConverter.PdfDocumentOptions.BottomMargin = model.BottomMargin;

                // Set the Word viewer zoom percentage
                wordToPdfConverter.PdfDocumentOptions.Zoom = model.WordViewerZoom;
            }

            // Set PDF header and footer
            SetHeader(wordToPdfConverter, model);
            SetFooter(wordToPdfConverter, model);

            byte[] inputWordBytes = null;

            // If an uploaded file exists, use it with priority
            if (model.WordFile != null && model.WordFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await model.WordFile.CopyToAsync(ms);
                    inputWordBytes = ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded Word file", ex);
                }
            }
            else
            {
                // Otherwise, fall back to the URL
                string wordUrl = model.WordFileUrl?.Trim();
                if (string.IsNullOrWhiteSpace(wordUrl))
                    throw new Exception("No Word file provided: upload a file or specify a URL");

                try
                {
                    if (wordUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                    {
                        string localPath = new Uri(wordUrl).LocalPath;
                        inputWordBytes = await System.IO.File.ReadAllBytesAsync(localPath);
                    }
                    else
                    {
                        using var httpClient = new System.Net.Http.HttpClient();
                        inputWordBytes = await httpClient.GetByteArrayAsync(wordUrl);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not download the Word file from URL", ex);
                }
            }

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = wordToPdfConverter.ConvertToPdf(inputWordBytes);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            // send as attachment
            fileResult.FileDownloadName = "Word_to_Pdf.pdf";

            return fileResult;
        }

        private void SetHeader(WordToPdfConverter wordToPdfConverter, Word_to_PDF_ViewModel model)
        {
            bool headerEnabled = model.HeaderEnabled;
            if (!headerEnabled)
                return;

            // Set the header HTML from a URL or from an HTML string
            bool headerHtmlFromUrl = model.HeaderHtmlSource == "Url";
            if (headerHtmlFromUrl)
            {
                string headerUrl = model.HeaderUrlTextBox;

                wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlSourceUrl = headerUrl;
            }
            else
            {
                string headerHtml = model.HeaderHtmlTextBox;
                string headerHtmlBaseUrl = model.HeaderHtmlBaseUrlTextBox;

                wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Html = headerHtml;
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlBaseUrl = headerHtmlBaseUrl;
            }

            // Enable automatic height adjustment based on header HTML content
            bool autoSizeHeaderContentHeight = model.AutoSizeHeaderContentHeight;
            wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoSizeContentHeight = autoSizeHeaderContentHeight;

            // Set the minimum and maximum content height used when AutoSizeContentHeight is enabled
            wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.MinContentHeight = model.HeaderMinContentHeight;
            wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.MaxContentHeight = model.HeaderMaxContentHeight;

            // Set a fixed height for the header if AutoResizeHeight is disabled
            if (model.HeaderHeight.HasValue)
            {
                int headerHeight = model.HeaderHeight.Value;
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Height = headerHeight;
            }

            // If AutoResizeHeight is enabled and both Height and FitHeight are set,
            // the content may be scaled down to fit the specified height
            bool fitHeaderHeight = model.FitHeaderHeight;
            wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.FitHeight = fitHeaderHeight;

            // Enable automatic top margin adjustment in the PDF based on the header
            bool autoResizeTopMargin = model.AutoResizeTopMargin;
            wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoResizePdfMargins = autoResizeTopMargin;

            // Set header visibility on specific PDF pages: first page, odd-numbered pages and even-numbered pages
            wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInFirstPage = model.ShowHeaderInFirstPage;
            wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInOddPages = model.ShowHeaderInOddPages;
            wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInEvenPages = model.ShowHeaderInEvenPages;

            // Reserve space for the header on all pages, regardless of visibility
            // If false, the document will be rendered using print styles instead of screen styles
            wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ReserveSpaceAlways = model.ReserveHeaderSpace;

            // Optimize the header rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
            wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.SkipVariablesParsing = model.SkipHeaderVariablesParsing;

            // Optionally set additional time to wait for the asynchronous header HTML content before rendering
            if (model.HeaderConversionDelay.HasValue)
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ConversionDelay = model.HeaderConversionDelay.Value;
        }

        private void SetFooter(WordToPdfConverter wordToPdfConverter, Word_to_PDF_ViewModel model)
        {
            bool footerEnabled = model.FooterEnabled;
            if (footerEnabled)
            {
                // Set the footer HTML from a URL or from an HTML string
                bool footerHtmlFromUrl = model.FooterHtmlSource == "Url";
                if (footerHtmlFromUrl)
                {
                    string footerUrl = model.FooterUrlTextBox;

                    wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlSourceUrl = footerUrl;
                }
                else
                {
                    string footerHtml = model.FooterHtmlTextBox;
                    string footerHtmlBaseUrl = model.FooterHtmlBaseUrlTextBox;

                    wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Html = footerHtml;
                    wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlBaseUrl = footerHtmlBaseUrl;
                }

                // Enable automatic height adjustment based on footer HTML content
                bool autoSizeFooterContentHeight = model.AutoSizeFooterContentHeight;
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoSizeContentHeight = autoSizeFooterContentHeight;

                // Set the minimum and maximum content height used when AutoSizeContentHeight is enabled
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.MinContentHeight = model.FooterMinContentHeight;
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.MaxContentHeight = model.FooterMaxContentHeight;

                // Set a fixed height for the footer if AutoResizeHeight is disabled
                if (model.FooterHeight.HasValue)
                {
                    int footerHeight = model.FooterHeight.Value;
                    wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Height = footerHeight;
                }

                // If AutoResizeHeight is enabled and both Height and FitHeight are set,
                // the content may be scaled down to fit the specified height
                bool fitFooterHeight = model.FitFooterHeight;
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.FitHeight = fitFooterHeight;

                // Enable automatic bottom margin adjustment in the PDF based on the footer
                bool autoResizeBottomMargin = model.AutoResizeBottomMargin;
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoResizePdfMargins = autoResizeBottomMargin;

                // Set footer visibility on specific PDF pages: first page, odd-numbered pages and even-numbered pages
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInFirstPage = model.ShowFooterInFirstPage;
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInOddPages = model.ShowFooterInOddPages;
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInEvenPages = model.ShowFooterInEvenPages;

                // Reserve space for the footer on all pages, regardless of visibility
                // If false, the document will be rendered using print styles instead of screen styles
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ReserveSpaceAlways = model.ReserveFooterSpace;

                // Optimize the footer rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.SkipVariablesParsing = model.SkipFooterVariablesParsing;

                // Optionally set additional time to wait for the asynchronous footer HTML content before rendering
                if (model.FooterConversionDelay.HasValue)
                    wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ConversionDelay = model.FooterConversionDelay.Value;
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

        private Word_to_PDF_ViewModel SetViewModel()
        {
            var model = new Word_to_PDF_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Word_To_PDF".Length);

            model.WordFileUrl = rootUrl + "/DemoAppFiles/Input/Word_Files/Word_Document.docx";
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
