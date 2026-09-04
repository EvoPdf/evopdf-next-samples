using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_Creator;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_Creator
{
    public class Create_PDF_Documents_with_StandardsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Create_PDF_Documents_with_StandardsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = new Create_PDF_Documents_with_Standards_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePdf(Create_PDF_Documents_with_Standards_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the library in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
            {
                PageSize = PdfPageSize.A4,
                PageOrientation = PdfPageOrientation.Portrait,
                Margins = new PdfMargins(36, 36, 36, 36),
                PdfStandard = model.PdfStandard,
                Language = "en-US"
            };

            using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);

            pdfDocument.PdfDocumentInfo.Title = "Standards Compliance Demo";

            const int xLeft = 0;
            const int ySeparator = 15;
            int crtYPos = 0;

            string fontsPath = GetDemoFontsPath();
            string imagesPath = GetDemoImagesPath();
            string textsPath = GetDemoTextsPath();

            string fontFilePath = Path.Combine(fontsPath, "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);

            // Font roles: page title, section labels, body, captions.
            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 18f,
                PdfFontStyle.Bold | PdfFontStyle.Underline, PdfColor.Black);
            PdfFont sectionFont = PdfFontManager.CreateFont(baseFont, 14f,
                PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont bodyFont = PdfFontManager.CreateFont(baseFont, 14f,
                PdfFontStyle.Normal, PdfColor.Black);
            PdfFont labelFont = PdfFontManager.CreateFont(baseFont, 9f,
                PdfFontStyle.Normal, PdfColor.DimGray);

            // ===== Title =====
            PdfTextElement titleElement = new PdfTextElement(
                "PDF/UA and PDF/A Standards Compliance Demo", titleFont)
            {
                X = xLeft,
                Y = crtYPos,
                Alignment = PdfTextAlignment.Center,
                Width = pdfDocument.ContentWidth
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            crtYPos = (int)pdfDocument.AddText(titleElement).LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 1: Transparent PNG with custom width =====
            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "1. Transparent PNG with custom width", xLeft, crtYPos, ySeparator);

            PdfImageElement pdfPngImage = new PdfImageElement(Path.Combine(imagesPath, "transparent.png"))
            {
                X = xLeft,
                Y = crtYPos,
                Width = 150,
                RotationPivot = PdfRotationPivot.TopCenter,
                Opacity = 0.75f
            };
            pdfPngImage.Accessibility.AlternateText = "Glass globe with fish";

            PdfImageRenderInfo pngInfo = pdfDocument.AddImage(pdfPngImage);
            crtYPos = (int)pngInfo.BoundingBox.Bottom + ySeparator;

            crtYPos = AddCaption(pdfDocument, labelFont,
                "150pt wide PNG with transparency, Opacity=0.75, accessible alt text",
                xLeft, crtYPos, 400) + ySeparator;

            // ===== Section 2: JPEG with custom height =====
            EnsureSpaceOnPage(ref crtYPos, 220, pdfDocument);
            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "2. JPEG with custom height", xLeft, crtYPos, ySeparator);

            PdfImageElement pdfJpgImage = new PdfImageElement(Path.Combine(imagesPath, "image.jpg"))
            {
                X = xLeft,
                Y = crtYPos,
                Height = 150
            };
            pdfJpgImage.Accessibility.AlternateText = "Wooden house on a river";

            PdfImageRenderInfo jpgInfo = pdfDocument.AddImage(pdfJpgImage);
            crtYPos = (int)jpgInfo.BoundingBox.Bottom + ySeparator;

            crtYPos = AddCaption(pdfDocument, labelFont,
                "150pt tall JPEG, accessible alt text",
                xLeft, crtYPos, 400) + ySeparator;

            // ===== Section 3: Multi-page Unicode text + per-page border via OnAfterPageRender =====
            EnsureSpaceOnPage(ref crtYPos, 100, pdfDocument);
            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "3. Multi-page Unicode text with per-page blue border", xLeft, crtYPos, ySeparator);

            string alphabetFilePath = Path.Combine(textsPath, "Alphabet.txt");
            string alphabetString = System.IO.File.ReadAllText(alphabetFilePath);

            PdfTextElement pdfText1 = new PdfTextElement(alphabetString, bodyFont)
            {
                X = xLeft,
                Y = crtYPos,
                Alignment = PdfTextAlignment.Left,
                ContinueOnNextPage = true,
                RotationDegrees = 0,
                RotationPivot = PdfRotationPivot.TopLeft
            };

            // Draw a blue border around the rendered text area on each page.
            // OnAfterPageRender receives a PdfTextPageRenderInfo carrying both the
            // geometry (bounds, rotated quad, visible portion, page dimensions)
            // and the substring rendered on that page.
            pdfText1.OnAfterPageRender = info =>
            {
                var bounds = info.RenderedRectangle.Bounds;
                PdfRectangleElement border = new PdfRectangleElement(
                    bounds.X, bounds.Y, bounds.Width, bounds.Height + 5)
                {
                    FillColor = null,
                    BorderColor = PdfColor.Blue
                };
                pdfDocument.AddRectangle(border);
            };

            pdfDocument.AddText(pdfText1);

            // ===== Section 4: Same text on landscape pages, centered =====
            pdfDocument.SetPageSize(PdfPageSize.A4, PdfPageOrientation.Landscape);
            pdfDocument.AddPage();
            crtYPos = 0;

            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "4. Same text on landscape pages, centered, per-page purple border",
                xLeft, crtYPos, ySeparator);

            PdfTextElement pdfText2 = new PdfTextElement(alphabetString, bodyFont)
            {
                X = xLeft,
                Y = crtYPos,
                Alignment = PdfTextAlignment.Center,
                ContinueOnNextPage = true
            };

            pdfText2.OnAfterPageRender = info =>
            {
                var bounds = info.RenderedRectangle.Bounds;
                PdfRectangleElement border = new PdfRectangleElement(
                    bounds.X, bounds.Y, bounds.Width, bounds.Height + 5)
                {
                    FillColor = null,
                    BorderColor = PdfColor.Purple
                };
                pdfDocument.AddRectangle(border);
            };

            pdfDocument.AddText(pdfText2);

            // ===== Section 5: Right-to-left text =====
            pdfDocument.AddPage();
            crtYPos = 0;

            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "5. Right-to-left text", xLeft, crtYPos, ySeparator);

            string rtlFontFilePath = Path.Combine(fontsPath, "NotoSansArabic-Regular.ttf");
            PdfFont rtlBaseFont = PdfFontManager.CreateFont(rtlFontFilePath, 14f,
                PdfFontStyle.Normal, PdfColor.Black);

            string rtlFilePath = Path.Combine(textsPath, "RightToLeft.txt");
            string rtlString = System.IO.File.ReadAllText(rtlFilePath);

            PdfTextElement pdfTextRtl = new PdfTextElement(rtlString, rtlBaseFont)
            {
                X = xLeft,
                Y = crtYPos,
                Direction = PdfTextDirection.RightToLeft
            };
            pdfDocument.AddText(pdfTextRtl);

            // Save to memory buffer
            byte[] outPdfBuffer = pdfDocument.Save();

            // Send PDF to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfStandardsDemo.pdf";

            return fileResult;
        }

        // Renders a Heading2 section label at the requested position and returns
        // the Y where the next element should start
        private int AddSectionLabel(PdfDocument doc, PdfFont sectionFont,
            string label, int x, int y, int separator)
        {
            PdfTextElement section = new PdfTextElement(label, sectionFont)
            { X = x, Y = y };
            section.Accessibility.StructureType = PdfStructureType.Heading2;
            var info = doc.AddText(section);
            return (int)info.LastPageRectangle.Bounds.Bottom + separator;
        }

        // Renders a small caption as a decorative artifact (excluded from the
        // structure tree). Returns the Y position at the bottom of the caption
        // so callers can chain layout without hardcoding caption heights
        private int AddCaption(PdfDocument doc, PdfFont labelFont,
            string caption, int x, int y, int width)
        {
            PdfTextElement t = new PdfTextElement(caption, labelFont)
            { X = x, Y = y, Width = width };
            t.Accessibility.StructureType = PdfStructureType.Artifact;
            var info = doc.AddText(t);
            return (int)info.LastPageRectangle.Bounds.Bottom;
        }

        private void EnsureSpaceOnPage(ref int crtYPos, int requestedHeight, PdfDocument pdfDocument)
        {
            if (crtYPos + requestedHeight > pdfDocument.ContentHeight)
            {
                pdfDocument.AddPage();
                crtYPos = 0;
            }
        }

        private string GetDemoFilesPath()
        {
            return m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        }

        private string GetDemoImagesPath()
        {
            return Path.Combine(GetDemoFilesPath(), "Image_Files");
        }

        private string GetDemoFontsPath()
        {
            return Path.Combine(GetDemoFilesPath(), "Font_Files");
        }

        private string GetDemoTextsPath()
        {
            return Path.Combine(GetDemoFilesPath(), "Text_Files");
        }
    }
}
