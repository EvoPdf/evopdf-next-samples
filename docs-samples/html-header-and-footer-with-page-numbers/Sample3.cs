using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class HTML_in_Header_FooterController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public HTML_in_Header_FooterController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: HTML_in_Header_Footer
        public ActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(HTML_in_Header_Footer_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // Create an HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            bool headerEnabled = model.HeaderEnabled;
            if (headerEnabled)
            {
                // Set the header HTML from a URL or from an HTML string
                bool headerHtmlFromUrl = model.HeaderHtmlSource == "Url";
                if (headerHtmlFromUrl)
                {
                    string headerUrl = model.HeaderUrlTextBox;

                    htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlSourceUrl = headerUrl;
                }
                else
                {
                    string headerHtml = model.HeaderHtmlTextBox;
                    string headerHtmlBaseUrl = model.HeaderHtmlBaseUrlTextBox;

                    htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Html = headerHtml;
                    htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlBaseUrl = headerHtmlBaseUrl;
                }

                // Enable automatic height adjustment based on header HTML content
                bool autoSizeHeaderContentHeight = model.AutoSizeHeaderContentHeight;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoSizeContentHeight = autoSizeHeaderContentHeight;

                // Set the minimum and maximum content height used when AutoSizeContentHeight is enabled
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.MinContentHeight = model.HeaderMinContentHeight;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.MaxContentHeight = model.HeaderMaxContentHeight;

                // Set a fixed height for the header if AutoResizeHeight is disabled
                if (model.HeaderHeight.HasValue)
                {
                    int headerHeight = model.HeaderHeight.Value;
                    htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Height = headerHeight;
                }

                // If AutoResizeHeight is enabled and both Height and FitHeight are set,
                // the content may be scaled down to fit the specified height
                bool fitHeaderHeight = model.FitHeaderHeight;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.FitHeight = fitHeaderHeight;

                // Enable automatic top margin adjustment in the PDF based on the header
                bool autoResizeTopMargin = model.AutoResizeTopMargin;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoResizePdfMargins = autoResizeTopMargin;

                // Set header visibility on specific PDF pages: first page, odd-numbered pages and even-numbered pages
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInFirstPage = model.ShowHeaderInFirstPage;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInOddPages = model.ShowHeaderInOddPages;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInEvenPages = model.ShowHeaderInEvenPages;

                // Reserve space for the header on all pages, regardless of visibility
                // If false, the document will be rendered using print styles instead of screen styles
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ReserveSpaceAlways = model.ReserveHeaderSpace;

                // Optimize the header rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.SkipVariablesParsing = model.SkipHeaderVariablesParsing;

                // Optionally set additional time to wait for the asynchronous header HTML content before rendering
                if (model.HeaderConversionDelay.HasValue)
                    htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ConversionDelay = model.HeaderConversionDelay.Value;
            }

            bool footerEnabled = model.FooterEnabled;
            if (footerEnabled)
            {
                // Set the footer HTML from a URL or from an HTML string
                bool footerHtmlFromUrl = model.FooterHtmlSource == "Url";
                if (footerHtmlFromUrl)
                {
                    string footerUrl = model.FooterUrlTextBox;

                    htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlSourceUrl = footerUrl;
                }
                else
                {
                    string footerHtml = model.FooterHtmlTextBox;
                    string footerHtmlBaseUrl = model.FooterHtmlBaseUrlTextBox;

                    htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Html = footerHtml;
                    htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlBaseUrl = footerHtmlBaseUrl;
                }

                // Enable automatic height adjustment based on footer HTML content
                bool autoSizeFooterContentHeight = model.AutoSizeFooterContentHeight;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoSizeContentHeight = autoSizeFooterContentHeight;

                // Set the minimum and maximum content height used when AutoSizeContentHeight is enabled
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.MinContentHeight = model.FooterMinContentHeight;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.MaxContentHeight = model.FooterMaxContentHeight;

                // Set a fixed height for the footer if AutoResizeHeight is disabled
                if (model.FooterHeight.HasValue)
                {
                    int footerHeight = model.FooterHeight.Value;
                    htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Height = footerHeight;
                }

                // If AutoResizeHeight is enabled and both Height and FitHeight are set,
                // the content may be scaled down to fit the specified height
                bool fitFooterHeight = model.FitFooterHeight;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.FitHeight = fitFooterHeight;

                // Enable automatic bottom margin adjustment in the PDF based on the footer
                bool autoResizeBottomMargin = model.AutoResizeBottomMargin;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoResizePdfMargins = autoResizeBottomMargin;

                // Set footer visibility on specific PDF pages: first page, odd-numbered pages and even-numbered pages
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInFirstPage = model.ShowFooterInFirstPage;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInOddPages = model.ShowFooterInOddPages;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInEvenPages = model.ShowFooterInEvenPages;

                // Reserve space for the footer on all pages, regardless of visibility
                // If false, the document will be rendered using print styles instead of screen styles
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ReserveSpaceAlways = model.ReserveFooterSpace;

                // Optimize the footer rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.SkipVariablesParsing = model.SkipFooterVariablesParsing;

                // Optionally set additional time to wait for the asynchronous footer HTML content before rendering
                if (model.FooterConversionDelay.HasValue)
                    htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ConversionDelay = model.FooterConversionDelay.Value;
            }

            // The page numbering options apply to the header and to the footer
            PdfHtmlTemplate[] templates = { htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader, htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter };
            foreach (PdfHtmlTemplate template in templates)
            {
                // Offsets added to the page number and to the total number of pages written in place of the
                // {page_number} and {total_pages} placeholders
                template.PageNumberOffset = model.PageNumberOffset;
                template.TotalPagesOffset = model.TotalPagesOffset;

                // A zoom applied to the font of the page number placeholders, from 0.1 to 2
                template.PageCounterFontScale = model.PageCounterFontScale;

                if (model.UsePageNumbersFont)
                {
                    // The font of the text written in place of the placeholders; the bold, italic and bold italic
                    // variants can be given separately and default to this font
                    template.PageNumbersFont = PdfFontManager.CreateBaseFont(Path.Combine(m_hostingEnvironment.ContentRootPath,
                        "wwwroot", "DemoAppFiles", "Input", "Font_Files", "DejaVuSerif.ttf"));
                }

                // The template is rendered only in the pages generated from HTML or in all the pages of the document,
                // including the table of contents and the PDF documents inserted before and after the HTML content
                template.ShowOnlyInHtmlToPdfPages = model.ShowOnlyInHtmlToPdfPages;

                // The pages inserted before and after the HTML content are counted in the page numbers
                template.CountStartPages = model.CountStartPages;
                template.CountEndPages = model.CountEndPages;
                template.CountTocPages = model.CountTocPages;

                if (model.SkipPagesWithCallback)
                {
                    // The callback decides for each page if the template is rendered; it receives the placement of the
                    // template in the page. The template is skipped in the third page here
                    template.OnPageRendering = placement => placement.DocumentPageNumber != 3;
                }
            }

            if (model.AddStartAndEndPdf)
            {
                // PDF documents inserted before and after the HTML content
                string pdfFilesPath = Path.Combine(m_hostingEnvironment.ContentRootPath, "wwwroot", "DemoAppFiles", "Input", "PDF_Files");
                htmlToPdfConverter.PdfDocumentOptions.AddStartPdf(Path.Combine(pdfFilesPath, "Letterhead.pdf"));
                htmlToPdfConverter.PdfDocumentOptions.AddEndPdf(Path.Combine(pdfFilesPath, "Letterhead.pdf"));
            }

            if (model.AddTableOfContents)
            {
                // A table of contents inserted before the HTML content
                htmlToPdfConverter.PdfDocumentOptions.GenerateTableOfContents = true;
            }

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(model.Url);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "HTML_in_Footer_Footer.pdf";

            return fileResult;
        }

        private HTML_in_Header_Footer_ViewModel SetViewModel()
        {
            var model = new HTML_in_Header_Footer_ViewModel();

            var contentRootPath = Path.Combine(m_hostingEnvironment.ContentRootPath, "wwwroot");

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "HTML_in_Header_Footer".Length);

            model.HeaderHtmlTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Header_HTML.html"));
            model.FooterHtmlTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Footer_HTML.html"));
            model.HeaderHtmlBaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.HeaderUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/Header_HTML.html";
            model.FooterHtmlBaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.FooterUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/Footer_HTML.html";

            return model;
        }
    }
}
