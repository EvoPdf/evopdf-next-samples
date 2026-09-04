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

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

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