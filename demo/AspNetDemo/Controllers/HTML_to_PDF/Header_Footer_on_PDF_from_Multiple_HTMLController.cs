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

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // The PDF document that receives the HTML documents. The top and bottom margins reserve the space of the
            // header and of the footer on every page
            int topMargin = model.HeaderEnabled ? model.HeaderHeight : 0;
            int bottomMargin = model.FooterEnabled ? model.FooterHeight : 0;
            PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
            {
                PageSize = PdfPageSize.A4,
                PageOrientation = PdfPageOrientation.Portrait,
                Margins = new PdfMargins(0, 0, topMargin, bottomMargin)
            };

            using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);

            // The header and the footer are HTML templates of the document, drawn on every page of it when it is saved,
            // with the page numbers of the whole document
            int pageWidth = pdfDocument.PageSize.Width;

            PdfHtmlTemplate headerTemplate = null;
            PdfHtmlTemplate footerTemplate = null;

            if (model.HeaderEnabled)
            {
                headerTemplate = model.HeaderHtmlSource == "Url"
                    ? pdfDocument.AddHtmlTemplate(0, 0, pageWidth, model.HeaderHeight, model.HeaderUrlTextBox)
                    : pdfDocument.AddHtmlTemplate(0, 0, pageWidth, model.HeaderHeight, model.HeaderHtmlTextBox, model.HeaderHtmlBaseUrlTextBox);
                headerTemplate.VerticalAlign = PdfTemplateVerticalAlign.Top;
                SetTemplateOptions(headerTemplate, model.FitHeaderHeight, model.HeaderZoom, model.SkipHeaderVariablesParsing, model.HeaderConversionDelay,
                    model.ShowHeaderInFirstPage, model.ShowHeaderInOddPages, model.ShowHeaderInEvenPages);
            }

            if (model.FooterEnabled)
            {
                footerTemplate = model.FooterHtmlSource == "Url"
                    ? pdfDocument.AddHtmlTemplate(0, 0, pageWidth, model.FooterHeight, model.FooterUrlTextBox)
                    : pdfDocument.AddHtmlTemplate(0, 0, pageWidth, model.FooterHeight, model.FooterHtmlTextBox, model.FooterHtmlBaseUrlTextBox);
                footerTemplate.VerticalAlign = PdfTemplateVerticalAlign.Bottom;
                SetTemplateOptions(footerTemplate, model.FitFooterHeight, model.FooterZoom, model.SkipFooterVariablesParsing, model.FooterConversionDelay,
                    model.ShowFooterInFirstPage, model.ShowFooterInOddPages, model.ShowFooterInEvenPages);
            }

            bool startOnNewPage = model.SecondDocumentStart == "NewPage";

            // Create the list of URLs to convert
            List<string> urlsToConvert = new List<string>() {
                model.FirstUrl,
                model.SecondUrl
            };

            // The position where the next HTML document starts in the current page
            float crtYPos = 0;

            for (int i = 0; i < urlsToConvert.Count; i++)
            {
                if (i > 0 && (startOnNewPage || crtYPos >= pdfDocument.ContentHeight))
                {
                    // The next HTML document starts at the top of a new page, as asked or because the previous
                    // one ended at the bottom of its last page
                    pdfDocument.AddPage();
                    crtYPos = 0;
                }

                // The HTML document starts at the current position and continues on as many new pages as it needs,
                // inside the margins of the document, below the header and above the footer
                PdfHtmlElement htmlElement = new PdfHtmlElement(urlsToConvert[i]);
                htmlElement.Y = crtYPos;

                // Render the HTML document in the PDF document
                PdfHtmlRenderInfo renderInfo = pdfDocument.AddHtml(htmlElement);

                if (i == 0 && model.TemplateZoomFromDocuments)
                {
                    // The header and the footer are drawn at the zoom of the HTML documents, available after the first
                    // document was rendered, so the fonts have the same sizes in the templates and in the documents
                    double documentsZoom = Math.Max(10, Math.Min(200, htmlElement.Converter.ConversionInfo.PrintZoom));
                    if (headerTemplate != null)
                        headerTemplate.Zoom = documentsZoom;
                    if (footerTemplate != null)
                        footerTemplate.Zoom = documentsZoom;
                }

                // The next HTML document starts below the rendered content of the last page
                crtYPos = renderInfo.LastPageRectangle.Bounds.Bottom;
            }

            // Save the PDF document in a memory buffer. The header and the footer are rendered now, with the final page numbers
            byte[] outPdfBuffer = pdfDocument.Save();

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Header_Footer_on_PDF_from_Multiple_HTML.pdf";
            return fileResult;
        }

        private void SetTemplateOptions(PdfHtmlTemplate template, bool fitHeight, double zoom, bool skipVariablesParsing, int? conversionDelay,
            bool showInFirstPage, bool showInOddPages, bool showInEvenPages)
        {
            // The content keeps its own height up to the height of the template; when it is taller it is scaled down
            // to fit the template if FitHeight is true, or cut at the height of the template otherwise
            template.AutoSizeContentHeight = true;
            template.FitHeight = fitHeight;
            if (!fitHeight)
                template.MaxContentHeight = template.Height;

            // The zoom at which the template HTML is laid out and drawn, at the same template width; replaced by the zoom
            // of the HTML documents when TemplateZoomFromDocuments is set
            template.Zoom = zoom;

            // Set the template visibility on specific PDF pages: first page, odd-numbered pages and even-numbered pages
            template.ShowInFirstPage = showInFirstPage;
            template.ShowInOddPages = showInOddPages;
            template.ShowInEvenPages = showInEvenPages;

            // Optimize the template rendering time by providing a hint if the HTML template contains variables such as {page_number} or {total_pages}
            template.SkipVariablesParsing = skipVariablesParsing;

            // Optionally set additional time to wait for the asynchronous template HTML content before rendering
            if (conversionDelay.HasValue)
                template.ConversionDelay = conversionDelay.Value;
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
