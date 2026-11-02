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
    public class Add_Word_to_PDF_DocumentsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Add_Word_to_PDF_DocumentsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePdf(Add_Word_to_PDF_Documents_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            byte[] inputWordBytes = await ReadWordFile(model.WordFile, model.WordFileUrl);

            PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
            {
                PageSize = PdfPageSize.A4,
                PageOrientation = PdfPageOrientation.Portrait,
                Margins = new PdfMargins(36, 36, 36, 36),
                PdfStandard = model.PdfStandard,
                Language = "en-US"
            };

            using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);
            pdfDocument.PdfDocumentInfo.Title = "Word Content in a PDF Document";

            string fontFilePath = Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 16f, PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont textFont = PdfFontManager.CreateFont(baseFont, 10f, PdfFontStyle.Normal, PdfColor.Black);

            // Elements created with the PDF creator before the document content
            PdfTextElement titleElement = new PdfTextElement("Word Content in a PDF Document", titleFont)
            {
                X = 0,
                Y = 0,
                Width = pdfDocument.ContentWidth,
                Alignment = PdfTextAlignment.Center
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            float crtYPos = pdfDocument.AddText(titleElement).LastPageRectangle.Bounds.Bottom + 10;

            PdfTextElement introElement = new PdfTextElement(
                "The text above and this paragraph are text elements of the PDF document. The content of the Word " +
                "document starts below them and continues on new pages when it does not fit in this one. " +
                "The elements added after the Word content go to its last page, below it.", textFont)
            {
                X = 0,
                Y = crtYPos,
                Width = pdfDocument.ContentWidth
            };
            crtYPos = pdfDocument.AddText(introElement).LastPageRectangle.Bounds.Bottom + 15;

            // The Word document rendered in the PDF document, from the given position of the current
            // page and inside the page margins. The X position is kept on the next pages
            PdfWordElement wordElement = new PdfWordElement(inputWordBytes)
            {
                X = model.ElementX,
                Y = crtYPos,
                // 0 extends the content to the right margin of the page
                Width = model.ElementWidth,
                Zoom = model.ElementZoom,
                // The whole content is scaled down to show entirely a table wider than the content area only when asked to
                FitWideContentToPage = model.FitWideContentToPage,
                // The headings of the Word document join the outline of the PDF document
                GenerateDocumentOutline = model.GenerateDocumentOutline,
                // The fonts stored in the Word document are used; FontFiles and FallbackFontFiles can give
                // more fonts to the element, as for the converter
                UseEmbeddedFonts = model.UseEmbeddedFonts
            };

            PdfContentRenderInfo wordRenderInfo = pdfDocument.AddWord(wordElement);

            // The rendered area reported for the last page places the elements after the content
            crtYPos = wordRenderInfo.LastPageRectangle.Bounds.Bottom + 15;

            PdfRectangleElement separator = new PdfRectangleElement(0, crtYPos, pdfDocument.ContentWidth, 1)
            {
                FillColor = PdfColor.DarkBlue,
                BorderColor = null
            };
            pdfDocument.AddRectangle(separator);

            PdfTextElement closingElement = new PdfTextElement(
                string.Format("The Word content was rendered on {0} page(s), from page {1} to page {2}. This text was added after it.",
                    wordRenderInfo.PagesCount, wordRenderInfo.FirstPageNumber, wordRenderInfo.LastPageNumber), textFont)
            {
                X = 0,
                Y = crtYPos + 8,
                Width = pdfDocument.ContentWidth
            };
            pdfDocument.AddText(closingElement);

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = pdfDocument.Save();

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Word_in_Pdf_Document.pdf";

            return fileResult;
        }

        private async Task<byte[]> ReadWordFile(IFormFile wordFile, string wordFileUrl)
        {
            // If an uploaded file exists, use it with priority
            if (wordFile != null && wordFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await wordFile.CopyToAsync(ms);
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded Word file", ex);
                }
            }

            // Otherwise, fall back to the URL
            string wordUrl = wordFileUrl?.Trim();
            if (string.IsNullOrWhiteSpace(wordUrl))
                throw new Exception("No Word file provided: upload a file or specify a URL");

            try
            {
                if (wordUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    string localPath = new Uri(wordUrl).LocalPath;
                    return await System.IO.File.ReadAllBytesAsync(localPath);
                }

                using var httpClient = new System.Net.Http.HttpClient();
                return await httpClient.GetByteArrayAsync(wordUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not download the Word file from URL", ex);
            }
        }

        private Add_Word_to_PDF_Documents_ViewModel SetViewModel()
        {
            var model = new Add_Word_to_PDF_Documents_ViewModel();

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Add_Word_to_PDF_Documents".Length);

            model.WordFileUrl = rootUrl + "/DemoAppFiles/Input/Word_Files/Word_Document.docx";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
