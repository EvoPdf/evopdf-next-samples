// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-html-stamp-to-generated-pdf.htm
// Documentation page: Add HTML Stamp with Page Numbering to Generated PDF

using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Stamp_with_HTML_Generated_PDFController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Stamp_with_HTML_Generated_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Stamp_with_HTML_Generated_PDF_ViewModel model)
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

            bool stampEnabled = model.StampEnabled;
            if (stampEnabled)
            {
                PdfHtmlTemplate stamp = null;

                // Get stamp width
                int stampWidth = model.StampWidth;

                // Get stamp height
                int stampHeight = 0;
                if (model.StampHeight.HasValue)
                    stampHeight = model.StampHeight.Value;

                // Get stamp top left corner X and Y position
                int x = model.StampXPosition;
                int y = model.StampYPosition;

                // Get horizontal alignment, which takes precedence over the X coordinate when specified
                PdfTemplateHorizontalAlign horizontalAlign = model.HorizontalAlign;

                // Get vertical alignment, which takes precedence over the Y coordinate when specified
                PdfTemplateVerticalAlign verticalAlign = model.VerticalAlign;

                // Set the stamp HTML from an URL or from a HTML string
                bool stampHtmlFromUrl = model.StampHtmlSource == "Url";
                if (stampHtmlFromUrl)
                {
                    string stampUrl = model.StampUrlTextBox;

                    if (stampHeight > 0)
                    {
                        // The stamp has a specified height
                        stamp = htmlToPdfConverter.PdfDocumentOptions.AddHtmlTemplate(x, y, stampWidth, stampHeight,
                            horizontalAlign, verticalAlign, stampUrl);
                    }
                    else
                    {
                        // The stamp size is automatically determined from the HTML content when AutoSizeContentHeight is true
                        stamp = htmlToPdfConverter.PdfDocumentOptions.AddHtmlTemplate(x, y, stampWidth, stampHeight,
                           horizontalAlign, verticalAlign, stampUrl);
                    }
                }
                else
                {
                    string stampHtml = model.StampHtmlTextBox;
                    string stampHtmlBaseUrl = model.StampHtmlBaseUrlTextBox;

                    if (stampHeight > 0)
                    {
                        // The stamp has a specified height
                        stamp = htmlToPdfConverter.PdfDocumentOptions.AddHtmlTemplate(x, y, stampWidth, stampHeight,
                            horizontalAlign, verticalAlign, stampHtml, stampHtmlBaseUrl);
                    }
                    else
                    {
                        // The stamp size is automatically determined from the HTML content when AutoSizeContentHeight is true
                        stamp = htmlToPdfConverter.PdfDocumentOptions.AddHtmlTemplate(x, y, stampWidth, stampHeight,
                            horizontalAlign, verticalAlign, stampHtml, stampHtmlBaseUrl);
                    }
                }

                // Set stamp rotation degrees. Positive values rotate counter-clockwise
                stamp.RotationDegrees = model.RotationDirection == RotationDirection.CounterClockwise
                        ? model.RotationDegrees
                        : -model.RotationDegrees;

                // Set stamp rotation pivot
                stamp.RotationPivot = model.RotationPivot;

                // If AutoSizeContentHeight is true, the Height property is positive and FitHeight is true,
                // the content may be scaled down to fit the specified height
                bool fitStampHeight = model.FitStampHeight;
                stamp.FitHeight = fitStampHeight;

                // Set the auto resize stamp option to allow the stamp height adjust based on HTML content
                bool autoSizeStampContentHeight = model.AutoSizeStampContentHeight;
                stamp.AutoSizeContentHeight = autoSizeStampContentHeight;

                // Set Min and Max content height used when the AutoSizeContentHeight property is true
                stamp.MinContentHeight = model.StampMinContentHeight;
                stamp.MaxContentHeight = model.StampMaxContentHeight;

                // Set stamp visibility in PDF for the first page, odd pages, and even pages
                stamp.ShowInFirstPage = model.ShowStampInFirstPage;
                stamp.ShowInOddPages = model.ShowStampInOddPages;
                stamp.ShowInEvenPages = model.ShowStampInEvenPages;

                // Sets the opacity for the entire stamp content, including text.
                // To apply opacity only to the background, use the alpha channel
                // in the CSS background-color property of the body element,
                // e.g., background-color: rgba(255, 255, 255, 0.75);
                int opacity = model.StampOpacity;
                stamp.Opacity = ((float)opacity) / 100;

                // Optimize the stamp rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
                stamp.SkipVariablesParsing = model.SkipVariablesParsing;

                // Optionally set additional time to wait for the asynchronous stamp HTML content before rendering
                if (model.ConversionDelay.HasValue)
                    stamp.ConversionDelay = model.ConversionDelay.Value;
            }

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(model.Url);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Stamp_with_HTML_Generated_PDF.pdf";

            return fileResult;
        }

        private Stamp_with_HTML_Generated_PDF_ViewModel SetViewModel()
        {
            var model = new Stamp_with_HTML_Generated_PDF_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Stamp_with_HTML_Generated_PDF".Length);

            model.StampHtmlTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Stamp_HTML.html"));
            model.StampHtmlBaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.StampUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/Stamp_HTML.html";

            return model;
        }
    }
}
