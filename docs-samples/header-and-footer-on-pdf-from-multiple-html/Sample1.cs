// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/header-and-footer-on-pdf-from-multiple-html.htm
// Documentation page: Add Header and Footer to PDF from Multiple HTML

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Header_Footer_on_PDF_from_Multiple_HTMLController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Header_Footer_on_PDF_from_Multiple_HTMLController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult MergePdf(Header_Footer_on_PDF_from_Multiple_HTML_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create the first HTML to PDF converter instance
            HtmlToPdfConverter firstHtmlToPdfConverter = new HtmlToPdfConverter();

            // Delay header and footer rendering until the merged PDF is saved
            firstHtmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.DelayContentRendering = true;
            firstHtmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.DelayContentRendering = true;

            // Set header and footer visibility for the first converter
            SetHeaderFooterVisibility(firstHtmlToPdfConverter, 0, model);

            // Set common options for the first converter
            SetHtmlToPdfConverterOptions(firstHtmlToPdfConverter, model);

            // Convert the first HTML to PDF
            byte[] firstPdfBytes = firstHtmlToPdfConverter.ConvertUrl(model.FirstUrl);

            // Get header and footer size based on their rendered HTML content
            System.Drawing.Size headerSize = firstHtmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.DestinationSize;
            System.Drawing.Size footerSize = firstHtmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.DestinationSize;

            // Create the PDF merger
            using PdfMerge pdfMerge = new PdfMerge();

            // Set merge options including header and footer dimensions
            SetPdfMergeOptions(pdfMerge, headerSize, footerSize, model);

            // Add the first PDF to the merger
            int firstPdfPageCount = pdfMerge.AddPdf(firstPdfBytes);

            // Prepare the list of additional URLs to convert and merge
            List<string> urlsToConvert = new List<string>() {
                model.SecondUrl
            };

            foreach (string url in urlsToConvert)
            {
                // Create a new converter for each HTML source
                HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

                // Delay header and footer rendering until merge is saved
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.DelayContentRendering = true;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.DelayContentRendering = true;

                // Calculate current page count to determine visibility logic
                int totalPdfPageCount = pdfMerge.PdfMergeInfo.TotalPagesProduced;

                // Set header and footer visibility based on current position
                SetHeaderFooterVisibility(htmlToPdfConverter, totalPdfPageCount, model);

                // Apply general converter options
                SetHtmlToPdfConverterOptions(htmlToPdfConverter, model);

                // Convert the HTML to PDF
                byte[] pdfBytes = htmlToPdfConverter.ConvertUrl(url);

                // Add the result to the merger
                int pdfPageCount = pdfMerge.AddPdf(pdfBytes);
            }

            // Merge all PDFs into a single document, applying the header and footer
            byte[] mergedPdf = pdfMerge.Save();

            // Send the resulting PDF to the browser
            FileResult fileResult = new FileContentResult(mergedPdf, "application/pdf");
            fileResult.FileDownloadName = "Header_Footer_on_PDF_from_Multiple_HTML.pdf";

            return fileResult;
        }

        private void SetHeaderFooterVisibility(HtmlToPdfConverter htmlToPdfConverter, int totalPagesBefore, Header_Footer_on_PDF_from_Multiple_HTML_ViewModel model)
        {
            // Set header visibility
            SetHeaderFooterVisibility(htmlToPdfConverter, totalPagesBefore, true, model);

            // Set footer visibility
            SetHeaderFooterVisibility(htmlToPdfConverter, totalPagesBefore, false, model);
        }

        private void SetHeaderFooterVisibility(HtmlToPdfConverter htmlToPdfConverter, int totalPagesBefore, bool isHeader, Header_Footer_on_PDF_from_Multiple_HTML_ViewModel model)
        {
            PdfHtmlHeaderFooter pdfHtmlTemplate = null;

            bool showInFirstPage, showInEvenPages, showInOddPages;

            if (isHeader)
            {
                pdfHtmlTemplate = htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader;

                showInFirstPage = model.ShowHeaderInFirstPage;
                showInOddPages = model.ShowHeaderInOddPages;
                showInEvenPages = model.ShowHeaderInEvenPages;
            }
            else
            {
                pdfHtmlTemplate = htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter;

                showInFirstPage = model.ShowFooterInFirstPage;
                showInOddPages = model.ShowFooterInOddPages;
                showInEvenPages = model.ShowFooterInEvenPages;
            }

            pdfHtmlTemplate.ShowInFirstPage = showInFirstPage;
            pdfHtmlTemplate.ShowInEvenPages = showInEvenPages;
            pdfHtmlTemplate.ShowInOddPages = showInOddPages;

            if (totalPagesBefore > 0)
            {
                if (totalPagesBefore % 2 == 1)
                {
                    // First page is even in whole document
                    pdfHtmlTemplate.ShowInFirstPage = showInEvenPages;
                    pdfHtmlTemplate.ShowInOddPages = showInEvenPages;
                    pdfHtmlTemplate.ShowInEvenPages = showInOddPages;
                }
                else
                {
                    // First page is odd in whole document
                    pdfHtmlTemplate.ShowInFirstPage = showInOddPages;
                    pdfHtmlTemplate.ShowInOddPages = showInOddPages;
                    pdfHtmlTemplate.ShowInEvenPages = showInEvenPages;
                }
            }
        }

        private void SetHtmlToPdfConverterOptions(HtmlToPdfConverter htmlToPdfConverter, Header_Footer_on_PDF_from_Multiple_HTML_ViewModel model)
        {
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

                // Set minimum and maximum content height when AutoSizeContentHeight is enabled
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.MinContentHeight = model.HeaderMinContentHeight;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.MaxContentHeight = model.HeaderMaxContentHeight;

                // Set a fixed header height when AutoResizeHeight is disabled
                if (model.HeaderHeight.HasValue)
                {
                    int headerHeight = model.HeaderHeight.Value;
                    htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Height = headerHeight;
                }

                // If AutoResizeHeight and FitHeight are enabled, the content may be scaled down to fit the specified height
                bool fitHeaderHeight = model.FitHeaderHeight;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.FitHeight = fitHeaderHeight;

                // Automatically adjust the top page margin based on the header height
                bool autoResizeTopMargin = model.AutoResizeTopMargin;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoResizePdfMargins = autoResizeTopMargin;

                // Reserve space for the header on all pages, regardless of visibility
                // If false, print styles are used instead of screen styles
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

                // Set minimum and maximum content height when AutoSizeContentHeight is enabled
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.MinContentHeight = model.FooterMinContentHeight;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.MaxContentHeight = model.FooterMaxContentHeight;

                // Set a fixed footer height when AutoResizeHeight is disabled
                if (model.FooterHeight.HasValue)
                {
                    int footerHeight = model.FooterHeight.Value;
                    htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Height = footerHeight;
                }

                // If AutoResizeHeight and FitHeight are enabled, the content may be scaled down to fit the specified height
                bool fitFooterHeight = model.FitFooterHeight;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.FitHeight = fitFooterHeight;

                // Automatically adjust the bottom page margin based on the footer height
                bool autoResizeBottomMargin = model.AutoResizeBottomMargin;
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoResizePdfMargins = autoResizeBottomMargin;

                // Reserve space for the footer on all pages, regardless of visibility
                // If false, print styles are used instead of screen styles
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ReserveSpaceAlways = model.ReserveFooterSpace;

                // Optimize the footer rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
                htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.SkipVariablesParsing = model.SkipFooterVariablesParsing;

                // Optionally set additional time to wait for the asynchronous footer HTML content before rendering
                if (model.FooterConversionDelay.HasValue)
                    htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ConversionDelay = model.FooterConversionDelay.Value;
            }
        }

        private void SetPdfMergeOptions(PdfMerge pdfMerge, System.Drawing.Size headerSize, System.Drawing.Size footerSize, Header_Footer_on_PDF_from_Multiple_HTML_ViewModel model)
        {
            bool headerEnabled = model.HeaderEnabled;
            if (headerEnabled)
            {
                PdfHtmlTemplate pdfHeaderTemplate = null;

                // Set the header HTML from a URL or from an HTML string
                bool headerHtmlFromUrl = model.HeaderHtmlSource == "Url";
                if (headerHtmlFromUrl)
                {
                    string headerUrl = model.HeaderUrlTextBox;

                    pdfHeaderTemplate = pdfMerge.AddHtmlTemplate(0, 0, headerSize.Width, headerSize.Height, 
                            PdfTemplateHorizontalAlign.Left, PdfTemplateVerticalAlign.Top, headerUrl);
                }
                else
                {
                    string headerHtml = model.HeaderHtmlTextBox;
                    string headerHtmlBaseUrl = model.HeaderHtmlBaseUrlTextBox;

                    pdfHeaderTemplate = pdfMerge.AddHtmlTemplate(0, 0, headerSize.Width, headerSize.Height, 
                            PdfTemplateHorizontalAlign.Left, PdfTemplateVerticalAlign.Top, headerHtml, headerHtmlBaseUrl);
                }

                // Enable automatic height adjustment based on header HTML content
                bool autoSizeHeaderContentHeight = model.AutoSizeHeaderContentHeight;
                pdfHeaderTemplate.AutoSizeContentHeight = autoSizeHeaderContentHeight;

                // Set minimum and maximum content height when AutoSizeContentHeight is enabled
                pdfHeaderTemplate.MinContentHeight = model.HeaderMinContentHeight;
                pdfHeaderTemplate.MaxContentHeight = model.HeaderMaxContentHeight;

                // Set a fixed header height when AutoResizeHeight is disabled
                if (model.HeaderHeight.HasValue)
                {
                    int headerHeight = model.HeaderHeight.Value;
                    pdfHeaderTemplate.Height = headerHeight;
                }

                // If AutoResizeHeight and FitHeight are enabled, the content may be scaled down to fit the specified height
                bool fitHeaderHeight = model.FitHeaderHeight;
                pdfHeaderTemplate.FitHeight = fitHeaderHeight;

                // Set header visibility on specific PDF pages: first page, odd-numbered pages and even-numbered pages
                pdfHeaderTemplate.ShowInFirstPage = model.ShowHeaderInFirstPage;
                pdfHeaderTemplate.ShowInOddPages = model.ShowHeaderInOddPages;
                pdfHeaderTemplate.ShowInEvenPages = model.ShowHeaderInEvenPages;

                // Optimize the header rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
                pdfHeaderTemplate.SkipVariablesParsing = model.SkipHeaderVariablesParsing;

                // Optionally set additional time to wait for the asynchronous header HTML content before rendering
                if (model.HeaderConversionDelay.HasValue)
                    pdfHeaderTemplate.ConversionDelay = model.HeaderConversionDelay.Value;
            }

            bool footerEnabled = model.FooterEnabled;
            if (footerEnabled)
            {
                PdfHtmlTemplate pdfFooterTemplate = null;

                // Set the footer HTML from a URL or from an HTML string
                bool footerHtmlFromUrl = model.FooterHtmlSource == "Url";
                if (footerHtmlFromUrl)
                {
                    string footerUrl = model.FooterUrlTextBox;
                    pdfFooterTemplate = pdfMerge.AddHtmlTemplate(0, 0, footerSize.Width, footerSize.Height, 
                            PdfTemplateHorizontalAlign.Left, PdfTemplateVerticalAlign.Bottom, footerUrl);
                }
                else
                {
                    string footerHtml = model.FooterHtmlTextBox;
                    string footerHtmlBaseUrl = model.FooterHtmlBaseUrlTextBox;

                    pdfFooterTemplate = pdfMerge.AddHtmlTemplate(0, 0, footerSize.Width, footerSize.Height, 
                            PdfTemplateHorizontalAlign.Left, PdfTemplateVerticalAlign.Bottom, footerHtml, footerHtmlBaseUrl);
                }

                // Enable automatic height adjustment based on footer HTML content
                bool autoSizeFooterContentHeight = model.AutoSizeFooterContentHeight;
                pdfFooterTemplate.AutoSizeContentHeight = autoSizeFooterContentHeight;

                // Set minimum and maximum content height when AutoSizeContentHeight is enabled
                pdfFooterTemplate.MinContentHeight = model.FooterMinContentHeight;
                pdfFooterTemplate.MaxContentHeight = model.FooterMaxContentHeight;

                // Set a fixed footer height when AutoResizeHeight is disabled
                if (model.FooterHeight.HasValue)
                {
                    int footerHeight = model.FooterHeight.Value;
                    pdfFooterTemplate.Height = footerHeight;
                }

                // If AutoResizeHeight and FitHeight are enabled, the content may be scaled down to fit the specified height
                bool fitFooterHeight = model.FitFooterHeight;
                pdfFooterTemplate.FitHeight = fitFooterHeight;

                // Set footer visibility on specific PDF pages: first page, odd-numbered pages and even-numbered pages
                pdfFooterTemplate.ShowInFirstPage = model.ShowFooterInFirstPage;
                pdfFooterTemplate.ShowInOddPages = model.ShowFooterInOddPages;
                pdfFooterTemplate.ShowInEvenPages = model.ShowFooterInEvenPages;

                // Optimize the footer rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
                pdfFooterTemplate.SkipVariablesParsing = model.SkipFooterVariablesParsing;

                // Optionally set additional time to wait for the asynchronous footer HTML content before rendering
                if (model.FooterConversionDelay.HasValue)
                    pdfFooterTemplate.ConversionDelay = model.FooterConversionDelay.Value;
            }
        }

        private Header_Footer_on_PDF_from_Multiple_HTML_ViewModel SetViewModel()
        {
            var model = new Header_Footer_on_PDF_from_Multiple_HTML_ViewModel();

            var contentRootPath = m_hostingEnvironment.ContentRootPath + "/wwwroot";

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Header_Footer_on_PDF_from_Multiple_HTML".Length);

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
