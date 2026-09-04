using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_Editor;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_Editor
{
    public class Stamp_with_HTML_Existing_PDFController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Stamp_with_HTML_Existing_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> StampPdf(Stamp_with_HTML_Existing_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the library in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            byte[] inputPdfBytes = null;

            // If an uploaded file exists, use it with priority
            if (model.PdfFile != null && model.PdfFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await model.PdfFile.CopyToAsync(ms);
                    inputPdfBytes = ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded PDF file", ex);
                }
            }
            else
            {
                // Otherwise, fall back to the URL
                string pdfUrl = model.PdfFileUrl?.Trim();
                if (string.IsNullOrWhiteSpace(pdfUrl))
                    throw new Exception("No PDF file provided: upload a file or specify a URL");

                try
                {
                    if (pdfUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                    {
                        string localPath = new Uri(pdfUrl).LocalPath;
                        inputPdfBytes = await System.IO.File.ReadAllBytesAsync(localPath);
                    }
                    else
                    {
                        using var httpClient = new System.Net.Http.HttpClient();
                        inputPdfBytes = await httpClient.GetByteArrayAsync(pdfUrl);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not download the PDF file from URL", ex);
                }
            }

            // Open the PDF in editor
            string password = string.IsNullOrEmpty(model.OwnerPassword) ? model.UserPassword : model.OwnerPassword;
            using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, password);

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
                    string stampUrl = model.StampUrl;

                    if (stampHeight > 0)
                    {
                        // The stamp has a specified height
                        stamp = pdfEditor.AddHtmlTemplate(x, y, stampWidth, stampHeight,
                            horizontalAlign, verticalAlign, stampUrl);
                    }
                    else
                    {
                        // The stamp size is automatically determined from the HTML content when AutoSizeContentHeight is true
                        stamp = pdfEditor.AddHtmlTemplate(x, y, stampWidth, stampHeight,
                           horizontalAlign, verticalAlign, stampUrl);
                    }
                }
                else
                {
                    string stampHtml = model.StampHtml;
                    string stampHtmlBaseUrl = model.StampHtmlBaseUrl;

                    if (stampHeight > 0)
                    {
                        // The stamp has a specified height
                        stamp = pdfEditor.AddHtmlTemplate(x, y, stampWidth, stampHeight,
                            horizontalAlign, verticalAlign, stampHtml, stampHtmlBaseUrl);
                    }
                    else
                    {
                        // The stamp size is automatically determined from the HTML content when AutoSizeContentHeight is true
                        stamp = pdfEditor.AddHtmlTemplate(x, y, stampWidth, stampHeight,
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
                if (model.ConversionDelay.HasValue && model.ConversionDelay.Value > 0)
                    stamp.ConversionDelay = model.ConversionDelay.Value;
            }

            // Stamp the PDF and save the resulted PDF document in a memory buffer
            byte[] outPdfBuffer = pdfEditor.Save();

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Stamp_with_HTML_Existing_PDF.pdf";

            return fileResult;
        }

        private Stamp_with_HTML_Existing_PDF_ViewModel SetViewModel()
        {
            var model = new Stamp_with_HTML_Existing_PDF_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Stamp_with_HTML_Existing_PDF".Length);

            model.StampHtml = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Stamp_HTML.html"));
            model.StampHtmlBaseUrl = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.StampUrl = rootUrl + "DemoAppFiles/Input/HTML_Files/Stamp_HTML.html";
            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/PDF_Document.pdf";

            return model;
        }
    }
}
