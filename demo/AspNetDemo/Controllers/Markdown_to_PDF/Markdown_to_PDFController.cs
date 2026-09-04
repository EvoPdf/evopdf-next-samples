using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.Markdown_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.Markdown_to_PDF
{
    public class Markdown_to_PDFController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Markdown_to_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConvertMarkdownToPdf(Markdown_to_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create a Markdown to PDF converter object with default settings
            MarkdownToPdfConverter markdownToPdfConverter = new MarkdownToPdfConverter();

            // Set custom styling rules using CSS syntax
            markdownToPdfConverter.PdfDocumentOptions.StyleSheet = model.StyleSheet;

            // Set whether a table of contents is automatically generated from headings
            markdownToPdfConverter.PdfDocumentOptions.GenerateTableOfContents = model.GenerateToc;

            // Set PDF page size which can be a predefined size like A4 or a custom size in points 
            // Leave it not set to have a default A4 PDF page
            markdownToPdfConverter.PdfDocumentOptions.PdfPageSize = SelectedPdfPageSize(model.PdfPageSize);

            // Set PDF page orientation to Portrait or Landscape
            // Leave it not set to have a default Portrait orientation for PDF page
            markdownToPdfConverter.PdfDocumentOptions.PdfPageOrientation = SelectedPdfPageOrientation(model.PdfPageOrientation);

            // Set PDF page margins in points or leave them not set to have a PDF page without margins
            markdownToPdfConverter.PdfDocumentOptions.LeftMargin = model.LeftMargin;
            markdownToPdfConverter.PdfDocumentOptions.RightMargin = model.RightMargin;
            markdownToPdfConverter.PdfDocumentOptions.TopMargin = model.TopMargin;
            markdownToPdfConverter.PdfDocumentOptions.BottomMargin = model.BottomMargin;

            // Set the Markdown viewer zoom percentage
            markdownToPdfConverter.PdfDocumentOptions.Zoom = model.MarkdownViewerZoom;

            // Set PDF header and footer
            SetHeader(markdownToPdfConverter, model);
            SetFooter(markdownToPdfConverter, model);

            string markdownString = null;
            string baseUrl = null;

            if (model.MarkdownSource == "Url")
            {
                // Obtain the Markdown string and base URL from a Markdown file using UTF-8 encoding
                string markdownUrl = model.MarkdownFileUrl?.Trim();
                if (string.IsNullOrWhiteSpace(markdownUrl))
                    throw new Exception("No Markdown file provided: upload a file or specify a URL");

                byte[] inputMarkdownBytes = null;

                try
                {
                    if (markdownUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                    {
                        string localPath = new Uri(markdownUrl).LocalPath;
                        inputMarkdownBytes = await System.IO.File.ReadAllBytesAsync(localPath);
                    }
                    else
                    {
                        using var httpClient = new System.Net.Http.HttpClient();
                        inputMarkdownBytes = await httpClient.GetByteArrayAsync(markdownUrl);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not download the Markdown file from URL", ex);
                }

                markdownString = Encoding.UTF8.GetString(inputMarkdownBytes);
                baseUrl = markdownUrl;
            }
            else
            {
                // The Markdown string and base URL are provided directly
                markdownString = model.MarkdownString;
                baseUrl = model.BaseUrl;
            }

            // Convert the Markdown string to a PDF document in a memory buffer
            byte[] outPdfBuffer = markdownToPdfConverter.ConvertStringToPdf(markdownString, baseUrl);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            // send as attachment
            fileResult.FileDownloadName = "Markdown_to_Pdf.pdf";

            return fileResult;
        }

        private void SetHeader(MarkdownToPdfConverter markdownToPdfConverter, Markdown_to_PDF_ViewModel model)
        {
            bool headerEnabled = model.HeaderEnabled;
            if (!headerEnabled)
                return;

            // Set the header HTML from a URL or from an HTML string
            bool headerHtmlFromUrl = model.HeaderHtmlSource == "Url";
            if (headerHtmlFromUrl)
            {
                string headerUrl = model.HeaderUrlTextBox;

                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlSourceUrl = headerUrl;
            }
            else
            {
                string headerHtml = model.HeaderHtmlTextBox;
                string headerHtmlBaseUrl = model.HeaderHtmlBaseUrlTextBox;

                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Html = headerHtml;
                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlBaseUrl = headerHtmlBaseUrl;
            }

            // Enable automatic height adjustment based on header HTML content
            bool autoSizeHeaderContentHeight = model.AutoSizeHeaderContentHeight;
            markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoSizeContentHeight = autoSizeHeaderContentHeight;

            // Set the minimum and maximum content height used when AutoSizeContentHeight is enabled
            markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.MinContentHeight = model.HeaderMinContentHeight;
            markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.MaxContentHeight = model.HeaderMaxContentHeight;

            // Set a fixed height for the header if AutoResizeHeight is disabled
            if (model.HeaderHeight.HasValue)
            {
                int headerHeight = model.HeaderHeight.Value;
                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Height = headerHeight;
            }

            // If AutoResizeHeight is enabled and both Height and FitHeight are set,
            // the content may be scaled down to fit the specified height
            bool fitHeaderHeight = model.FitHeaderHeight;
            markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.FitHeight = fitHeaderHeight;

            // Enable automatic top margin adjustment in the PDF based on the header
            bool autoResizeTopMargin = model.AutoResizeTopMargin;
            markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoResizePdfMargins = autoResizeTopMargin;

            // Set header visibility on specific PDF pages: first page, odd-numbered pages and even-numbered pages
            markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInFirstPage = model.ShowHeaderInFirstPage;
            markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInOddPages = model.ShowHeaderInOddPages;
            markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInEvenPages = model.ShowHeaderInEvenPages;

            // Reserve space for the header on all pages, regardless of visibility
            // If false, the document will be rendered using print styles instead of screen styles
            markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ReserveSpaceAlways = model.ReserveHeaderSpace;

            // Optimize the header rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
            markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.SkipVariablesParsing = model.SkipHeaderVariablesParsing;

            // Optionally set additional time to wait for the asynchronous header HTML content before rendering
            if (model.HeaderConversionDelay.HasValue)
                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ConversionDelay = model.HeaderConversionDelay.Value;
        }

        private void SetFooter(MarkdownToPdfConverter markdownToPdfConverter, Markdown_to_PDF_ViewModel model)
        {
            bool footerEnabled = model.FooterEnabled;
            if (footerEnabled)
            {
                // Set the footer HTML from a URL or from an HTML string
                bool footerHtmlFromUrl = model.FooterHtmlSource == "Url";
                if (footerHtmlFromUrl)
                {
                    string footerUrl = model.FooterUrlTextBox;

                    markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlSourceUrl = footerUrl;
                }
                else
                {
                    string footerHtml = model.FooterHtmlTextBox;
                    string footerHtmlBaseUrl = model.FooterHtmlBaseUrlTextBox;

                    markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Html = footerHtml;
                    markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlBaseUrl = footerHtmlBaseUrl;
                }

                // Enable automatic height adjustment based on footer HTML content
                bool autoSizeFooterContentHeight = model.AutoSizeFooterContentHeight;
                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoSizeContentHeight = autoSizeFooterContentHeight;

                // Set the minimum and maximum content height used when AutoSizeContentHeight is enabled
                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.MinContentHeight = model.FooterMinContentHeight;
                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.MaxContentHeight = model.FooterMaxContentHeight;

                // Set a fixed height for the footer if AutoResizeHeight is disabled
                if (model.FooterHeight.HasValue)
                {
                    int footerHeight = model.FooterHeight.Value;
                    markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Height = footerHeight;
                }

                // If AutoResizeHeight is enabled and both Height and FitHeight are set,
                // the content may be scaled down to fit the specified height
                bool fitFooterHeight = model.FitFooterHeight;
                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.FitHeight = fitFooterHeight;

                // Enable automatic bottom margin adjustment in the PDF based on the footer
                bool autoResizeBottomMargin = model.AutoResizeBottomMargin;
                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoResizePdfMargins = autoResizeBottomMargin;

                // Set footer visibility on specific PDF pages: first page, odd-numbered pages and even-numbered pages
                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInFirstPage = model.ShowFooterInFirstPage;
                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInOddPages = model.ShowFooterInOddPages;
                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInEvenPages = model.ShowFooterInEvenPages;

                // Reserve space for the footer on all pages, regardless of visibility
                // If false, the document will be rendered using print styles instead of screen styles
                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ReserveSpaceAlways = model.ReserveFooterSpace;

                // Optimize the footer rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
                markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.SkipVariablesParsing = model.SkipFooterVariablesParsing;

                // Optionally set additional time to wait for the asynchronous footer HTML content before rendering
                if (model.FooterConversionDelay.HasValue)
                    markdownToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ConversionDelay = model.FooterConversionDelay.Value;
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

        private Markdown_to_PDF_ViewModel SetViewModel()
        {
            var model = new Markdown_to_PDF_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Markdown_To_PDF".Length);

            model.MarkdownFileUrl = rootUrl + "/DemoAppFiles/Input/Markdown_Files/Markdown_Document.md";
            model.MarkdownString = System.IO.File.ReadAllText(System.IO.Path.Combine(contentRootPath, "DemoAppFiles/Input/Markdown_Files/Markdown_Document.md"));
            model.BaseUrl = rootUrl + "/DemoAppFiles/Input/Markdown_Files/";
            model.StyleSheet = System.IO.File.ReadAllText(System.IO.Path.Combine(contentRootPath, "DemoAppFiles/Input/Markdown_Files/Markdown_Style.css"));
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
