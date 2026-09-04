using System;
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
    public class Retrieve_HTML_Element_Positions_in_PDFController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Retrieve_HTML_Element_Positions_in_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Retrieve_HTML_Element_Positions_in_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            bool enableElementsInfoSelector = model.EnableElementsInfoSelector;
            if (enableElementsInfoSelector)
            {
                // The CSS selector used to identify HTML elements for metadata collection.
                // The retrieved information will be available in the HtmlElementsInfo object,
                // which is exposed by htmlToPdfConverter after the conversion
                htmlToPdfConverter.HtmlElementsInfoSelector = model.RetrieveElementsInfoSelector;
            }

            // Optionally generate a table of contents for the PDF document
            htmlToPdfConverter.PdfDocumentOptions.GenerateTableOfContents = model.GenerateToc;

            // Optionally set a conversion delay to allow asynchronous content (e.g., JavaScript)
            // to load before rendering
            if (model.ConversionDelay.HasValue)
                htmlToPdfConverter.ConversionDelay = model.ConversionDelay.Value;

            byte[] outPdfBuffer = null;

            if (model.HtmlPageSource == "Html")
            {
                string htmlWithForm = model.HtmlString;
                string baseUrl = model.BaseUrl;

                // Convert a HTML string to a PDF document
                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlWithForm, baseUrl);
            }
            else
            {
                string url = model.Url;

                // Convert the HTML page to a PDF document
                outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
            }

            // Process the retrieved HTML elements info
            if (htmlToPdfConverter.HtmlElementsInfo != null)
            {
                // Load the generated PDF document into a PDF editor
                using PdfEditor pdfEditor = new PdfEditor(outPdfBuffer);

                // Highlight each retrieved HTML element in the PDF with a colored rectangle
                // based on its tag type. By default, GetHighlightColor() assigns specific colors
                // to h1, h2, h3 and h4 tags while other tags are highlighted in red
                foreach (HtmlElementInfo elemInfo in htmlToPdfConverter.HtmlElementsInfo.Elements)
                {
                    // An HTML element may be rendered across multiple PDF pages,
                    // resulting in multiple PdfRenderedRectangle instances
                    foreach (PdfRenderedRectangle renderedRectangle in elemInfo.RenderedRectangles)
                    {
                        int pageNumber = renderedRectangle.PageNumber;
                        PdfRectangleF bounds = renderedRectangle.Bounds;

                        PdfColor borderColor = GetHighlightColor(elemInfo.TagName);
                        var rectangleElement = new PdfRectangleElement(
                            bounds.X, bounds.Y, bounds.Width, bounds.Height)
                        {
                            BorderColor = borderColor
                        };

                        pdfEditor.AddRectangle(pageNumber, rectangleElement);
                    }
                }

                // Save the PDF with highlighted elements into the output buffer
                outPdfBuffer = pdfEditor.Save();
            }

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Retrieve_HTML_Element_Positions_in_PDF.pdf";

            return fileResult;
        }

        private PdfColor GetHighlightColor(string tagName)
        {
            PdfColor color = PdfColor.Red;
            switch (tagName)
            {
                case "h1":
                    color = PdfColor.Green;
                    break;
                case "h2":
                    color = PdfColor.Blue;
                    break;
                case "h3":
                    color = PdfColor.Purple;
                    break;
                case "h4":
                    color = PdfColor.Yellow;
                    break;
                default:
                    color = PdfColor.Red;
                    break;
            }

            return color;
        }

        private Retrieve_HTML_Element_Positions_in_PDF_ViewModel SetViewModel()
        {
            var model = new Retrieve_HTML_Element_Positions_in_PDF_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Retrieve_HTML_Element_Positions_in_PDF".Length);

            model.HtmlString = System.IO.File.ReadAllText(System.IO.Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/HTML_Element_Positions.html"));
            model.BaseUrl = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.Url = rootUrl + "DemoAppFiles/Input/HTML_Files/HTML_Element_Positions.html";

            return model;
        }
    }
}
