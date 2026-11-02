using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_Creator;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_Creator
{
    public class Create_PDF_Documents_with_PDF_PagesController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Create_PDF_Documents_with_PDF_PagesController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePdf(Create_PDF_Documents_with_PDF_Pages_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            byte[] sourcePdfBytes = await ReadInputPdf(model.PdfFile, model.PdfFileUrl);
            string sourcePassword = string.IsNullOrEmpty(model.PdfPassword) ? null : model.PdfPassword;

            // The top and bottom margins leave room for the band and the line of the letterhead page
            PdfMargins margins = new PdfMargins(40, 40, 90, 70);
            PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
            {
                PageSize = PdfPageSize.A4,
                PageOrientation = PdfPageOrientation.Portrait,
                Margins = margins,
                PdfStandard = model.PdfStandard,
                Language = "en-US"
            };

            using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);
            pdfDocument.PdfDocumentInfo.Title = "PDF Pages in a PDF Document";

            string fontFilePath = Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 16f, PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont textFont = PdfFontManager.CreateFont(baseFont, 10f, PdfFontStyle.Normal, PdfColor.Black);
            PdfFont captionFont = PdfFontManager.CreateFont(baseFont, 8f, PdfFontStyle.Normal, PdfColor.DarkGray);

            string letterheadPath = Path.Combine(GetDemoFilesPath(), "PDF_Files", "Letterhead.pdf");

            if (model.UseLetterhead)
                AddLetterhead(pdfDocument, letterheadPath, margins, model);

            PdfTextElement titleElement = new PdfTextElement("PDF Pages in a PDF Document", titleFont)
            {
                X = 0,
                Y = 0,
                Width = pdfDocument.ContentWidth
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            float crtYPos = pdfDocument.AddText(titleElement).LastPageRectangle.Bounds.Bottom + 10;

            PdfTextElement introElement = new PdfTextElement(
                "The page of the letterhead document is drawn under the content of this page and the pages of another PDF " +
                "document are drawn below as scaled copies, with their links and form fields when the annotations are included. " +
                "A drawn page keeps its fonts, images and vector content.", textFont)
            {
                X = 0,
                Y = crtYPos,
                Width = pdfDocument.ContentWidth
            };
            crtYPos = pdfDocument.AddText(introElement).LastPageRectangle.Bounds.Bottom + 20;

            if (model.DrawPages)
            {
                int[] pageNumbers = ParsePageNumbers(model.DrawnPages);
                using PdfEditor sourceInfo = new PdfEditor(sourcePdfBytes, sourcePassword);
                int sourcePageCount = sourceInfo.PageCount;

                float cellWidth = model.PageWidth + 20;
                float cellHeight = model.PageHeight + 30;
                int columns = Math.Max(1, (int)(pdfDocument.ContentWidth / cellWidth));
                int index = 0;

                foreach (int pageNumber in pageNumbers)
                {
                    if (pageNumber < 1 || pageNumber > sourcePageCount)
                        throw new Exception("The page numbers to draw must be between 1 and " + sourcePageCount);

                    int column = index % columns;
                    if (column == 0 && index > 0)
                        crtYPos += cellHeight;
                    if (crtYPos + cellHeight > pdfDocument.ContentHeight)
                    {
                        // The next row of pages goes to a new page
                        pdfDocument.AddPage();
                        if (model.UseLetterhead)
                            AddLetterhead(pdfDocument, letterheadPath, margins, model);
                        crtYPos = 0;
                    }

                    float x = column * cellWidth;

                    // A page of the source document scaled to fit the given width and height, keeping its proportions
                    PdfPageElement pageElement = new PdfPageElement(sourcePdfBytes, pageNumber)
                    {
                        Password = sourcePassword,
                        X = x,
                        Y = crtYPos,
                        Width = model.PageWidth,
                        Height = model.PageHeight,
                        Opacity = model.PageOpacity,
                        RotationDegrees = model.RotationDegrees,
                        RotationPivot = model.RotationPivot,
                        IncludeAnnotations = model.IncludeAnnotations
                    };
                    PdfElementRenderInfo renderInfo = pdfDocument.AddPdfPage(pageElement);

                    // The rendered rectangle gives the actual size of the scaled page
                    PdfRectangleF bounds = renderInfo.LastPageRectangle.Bounds;
                    pdfDocument.AddRectangle(new PdfRectangleElement(bounds.X, bounds.Y, bounds.Width, bounds.Height)
                    {
                        BorderColor = PdfColor.Gray,
                        FillColor = null
                    });
                    pdfDocument.AddText(new PdfTextElement("Page " + pageNumber, captionFont)
                    {
                        X = x,
                        Y = crtYPos + model.PageHeight + 4,
                        Width = model.PageWidth,
                        Alignment = PdfTextAlignment.Center
                    });

                    index++;
                }
                crtYPos += cellHeight;
            }

            if (model.DrawFullSizePage)
            {
                // The first page of the source document drawn at its own size on a new page, with the margins of the document
                pdfDocument.AddPage();
                if (model.UseLetterhead)
                    AddLetterhead(pdfDocument, letterheadPath, margins, model);

                pdfDocument.AddText(new PdfTextElement("The first page of the source document at its own size, inside the margins of this page:", textFont)
                {
                    X = 0,
                    Y = 0,
                    Width = pdfDocument.ContentWidth
                });

                PdfPageElement fullPageElement = new PdfPageElement(sourcePdfBytes, 1)
                {
                    Password = sourcePassword,
                    X = 0,
                    Y = 30,
                    Opacity = model.PageOpacity,
                    IncludeAnnotations = model.IncludeAnnotations
                };
                pdfDocument.AddPdfPage(fullPageElement);
            }

            byte[] outPdfBuffer = pdfDocument.Save();

            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PDF_Pages_in_Pdf_Document.pdf";
            return fileResult;
        }

        // The letterhead page covers the whole current page, from the top left corner of the page, under the content added later
        private static void AddLetterhead(PdfDocument pdfDocument, string letterheadPath, PdfMargins margins,
            Create_PDF_Documents_with_PDF_Pages_ViewModel model)
        {
            PdfPageElement letterhead = new PdfPageElement(letterheadPath, 1)
            {
                X = -margins.Left,
                Y = -margins.Top,
                Opacity = model.LetterheadOpacity,
                RenderUnderContent = model.RenderUnderContent
            };
            pdfDocument.AddPdfPage(letterhead);
        }

        private static int[] ParsePageNumbers(string pageNumbers)
        {
            return (pageNumbers ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => int.Parse(p.Trim())).ToArray();
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

        private Create_PDF_Documents_with_PDF_Pages_ViewModel SetViewModel()
        {
            var model = new Create_PDF_Documents_with_PDF_Pages_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Create_PDF_Documents_with_PDF_Pages".Length);

            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/Multi_Page_Document.pdf";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
