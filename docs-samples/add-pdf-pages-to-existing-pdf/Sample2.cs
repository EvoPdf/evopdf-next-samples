using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_Editor;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_Editor
{
    public class Add_PDF_Pages_to_Existing_PDFController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Add_PDF_Pages_to_Existing_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPdf(Add_PDF_Pages_to_Existing_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            byte[] inputPdfBytes = await ReadInputPdf(model.PdfFile, model.PdfFileUrl);
            byte[] sourcePdfBytes = await ReadInputPdf(model.SourcePdfFile, model.SourcePdfFileUrl);
            string password = !string.IsNullOrEmpty(model.OwnerPassword) ? model.OwnerPassword :
                !string.IsNullOrEmpty(model.UserPassword) ? model.UserPassword : null;
            string sourcePassword = string.IsNullOrEmpty(model.SourcePassword) ? null : model.SourcePassword;

            using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, password);
            int pageCount = pdfEditor.PageCount;

            List<int> targetPages = new List<int>();
            switch (model.TargetPages)
            {
                case "First":
                    targetPages.Add(1);
                    break;
                case "Last":
                    targetPages.Add(pageCount);
                    break;
                case "Odd":
                    for (int i = 1; i <= pageCount; i += 2)
                        targetPages.Add(i);
                    break;
                case "Even":
                    for (int i = 2; i <= pageCount; i += 2)
                        targetPages.Add(i);
                    break;
                default:
                    for (int i = 1; i <= pageCount; i++)
                        targetPages.Add(i);
                    break;
            }

            PdfFont captionFont = null;
            if (model.AddCaption)
                captionFont = PdfFontManager.CreateFont(Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf"), 8f, PdfFontStyle.Normal, PdfColor.DarkGray);

            foreach (int pageNumber in targetPages)
            {
                // The source page drawn in the target page. The position is relative to the top left corner of the page;
                // the page is scaled to fit the given width and height when they are set, otherwise it keeps its size
                PdfPageElement pageElement = new PdfPageElement(sourcePdfBytes, model.SourcePageNumber)
                {
                    Password = sourcePassword,
                    X = model.ElementX,
                    Y = model.ElementY,
                    Width = model.ElementWidth,
                    Height = model.ElementHeight,
                    Opacity = model.Opacity,
                    RotationDegrees = model.RotationDegrees,
                    RotationPivot = model.RotationPivot,
                    // The page goes under the existing content, as a letterhead or a watermark, or over it, as a stamp
                    RenderUnderContent = model.RenderUnderContent,
                    // The links and form fields of the source page are added to the target page
                    IncludeAnnotations = model.IncludeAnnotations
                };
                PdfElementRenderInfo renderInfo = pdfEditor.AddPdfPage(pageNumber, pageElement);

                if (model.AddCaption)
                {
                    PdfRectangleF bounds = renderInfo.LastPageRectangle.Bounds;
                    pdfEditor.AddText(pageNumber, new PdfTextElement(
                        string.Format("Page {0} of the source document drawn at ({1:0}, {2:0}), {3:0} x {4:0} points",
                            model.SourcePageNumber, bounds.X, bounds.Y, bounds.Width, bounds.Height), captionFont)
                    {
                        X = 20,
                        Y = pdfEditor.GetPdfPageInfo(pageNumber).PageSize.Height - 16
                    });
                }
            }

            byte[] outPdfBuffer = pdfEditor.Save();

            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Add_PDF_Pages_to_Existing_PDF.pdf";
            return fileResult;
        }

        private async Task<byte[]> ReadInputPdf(IFormFile pdfFile, string pdfFileUrl)
        {
            // If an uploaded file exists, use it with priority
            if (pdfFile != null && pdfFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await pdfFile.CopyToAsync(ms);
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded PDF file", ex);
                }
            }

            // Otherwise, fall back to the URL
            string pdfUrl = pdfFileUrl?.Trim();
            if (string.IsNullOrWhiteSpace(pdfUrl))
                throw new Exception("No PDF file provided: upload a file or specify a URL");

            try
            {
                if (pdfUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    string localPath = new Uri(pdfUrl).LocalPath;
                    return await System.IO.File.ReadAllBytesAsync(localPath);
                }

                using var httpClient = new System.Net.Http.HttpClient();
                return await httpClient.GetByteArrayAsync(pdfUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not download the PDF file from URL", ex);
            }
        }

        private Add_PDF_Pages_to_Existing_PDF_ViewModel SetViewModel()
        {
            var model = new Add_PDF_Pages_to_Existing_PDF_ViewModel();

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Path = request.PathBase.ToString() + request.Path.ToString(),
                Query = request.QueryString.ToString()
            };
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Add_PDF_Pages_to_Existing_PDF".Length);

            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/Multi_Page_Document.pdf";
            model.SourcePdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/Letterhead.pdf";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
