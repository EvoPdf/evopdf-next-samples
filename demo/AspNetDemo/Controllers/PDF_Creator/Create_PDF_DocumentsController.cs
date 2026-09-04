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
    public class Create_PDF_DocumentsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Create_PDF_DocumentsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = new Create_PDF_Documents_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePdf(Create_PDF_Documents_ViewModel model)
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
                Margins = new PdfMargins(36, 36, 36, 36)
            };

            // Create a new PDF document with the specified settings
            using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);

            const int xLeft = 0;
            const int ySeparator = 15;
            int crtYPos = 0;

            string imagesPath = GetDemoImagesPath();
            string fontsPath = GetDemoFontsPath();
            string textsPath = GetDemoTextsPath();

            // Each section title in this demo uses a different standard font so
            // the showcase exercises multiple font families/styles/colors in the
            // same document
            PdfFont fontHelveticaBoldUnderlineBlack = PdfFontManager.CreateStandardFont(
                PdfStandardFont.Helvetica, 16f, PdfFontStyle.Bold | PdfFontStyle.Underline, PdfColor.Black);
            PdfFont fontCourierBoldItalicGreen = PdfFontManager.CreateStandardFont(
                PdfStandardFont.Courier, 16f, PdfFontStyle.Bold | PdfFontStyle.Italic, PdfColor.Green);
            PdfFont fontCourierBoldBlue = PdfFontManager.CreateStandardFont(
                PdfStandardFont.Courier, 16f, PdfFontStyle.Bold, PdfColor.Blue);
            PdfFont fontCourierNormalPurple = PdfFontManager.CreateStandardFont(
                PdfStandardFont.Courier, 16f, PdfFontStyle.Normal, PdfColor.Purple);

            // ===== Section 1: Transparent PNG with custom width =====
            PdfTextElement pdfTitle1 = new PdfTextElement(
                "Transparent PNG Image with Custom Width", fontHelveticaBoldUnderlineBlack)
            {
                X = xLeft,
                Y = crtYPos
            };
            crtYPos = (int)pdfDocument.AddText(pdfTitle1).LastPageRectangle.Bounds.Bottom + ySeparator;

            PdfImageElement pdfPngImage = new PdfImageElement(Path.Combine(imagesPath, "transparent.png"))
            {
                X = xLeft,
                Y = crtYPos,
                Width = 150
            };
            crtYPos = (int)pdfDocument.AddImage(pdfPngImage).BoundingBox.Bottom + ySeparator;

            // ===== Section 2: JPEG with custom height =====
            PdfTextElement pdfTitle2 = new PdfTextElement(
                "JPEG Image with Custom Height", fontCourierBoldItalicGreen)
            {
                X = xLeft,
                Y = crtYPos
            };
            crtYPos = (int)pdfDocument.AddText(pdfTitle2).LastPageRectangle.Bounds.Bottom + ySeparator;

            // Ensure there is enough vertical space on the current page for the image.
            // Add a new page and reset Y position if needed
            EnsureSpaceOnPage(ref crtYPos, 200, pdfDocument);

            PdfImageElement pdfJpgImage = new PdfImageElement(Path.Combine(imagesPath, "image.jpg"))
            {
                X = xLeft,
                Y = crtYPos,
                Height = 150
            };
            crtYPos = (int)pdfDocument.AddImage(pdfJpgImage).BoundingBox.Bottom + ySeparator;

            // ===== Section 3: Multi-page Unicode text with per-page blue border =====
            PdfTextElement pdfTitle3 = new PdfTextElement(
                "Multi Page Unicode Text with Custom Font", fontCourierBoldBlue)
            {
                Y = crtYPos,
                Alignment = PdfTextAlignment.Center
            };
            crtYPos = (int)pdfDocument.AddText(pdfTitle3).LastPageRectangle.Bounds.Bottom + ySeparator;

            string alphabetFilePath = Path.Combine(textsPath, "Alphabet.txt");
            string alfabetString = System.IO.File.ReadAllText(alphabetFilePath);

            // Load the Unicode TrueType font used for the long alphabet body text
            string fontFilePath = Path.Combine(fontsPath, "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
            PdfFont trueTypeFont = PdfFontManager.CreateFont(baseFont, 16f,
                PdfFontStyle.Normal, PdfColor.Black);

            // Long Unicode text using the TrueType font, allowing continuation on next pages.
            // The OnAfterPageRender callback draws a blue rectangle around the rendered area on
            // each page the text spans
            PdfTextElement pdfText1 = new PdfTextElement(alfabetString, trueTypeFont)
            {
                X = xLeft,
                Y = crtYPos,
                Alignment = PdfTextAlignment.Left,
                ContinueOnNextPage = true
            };

            pdfText1.OnAfterPageRender = info =>
            {
                var bounds = info.RenderedRectangle.Bounds;
                PdfRectangleElement border = new PdfRectangleElement(bounds.X, bounds.Y,
                    bounds.Width, bounds.Height + 5)
                {
                    BorderColor = PdfColor.Blue,
                };
                pdfDocument.AddRectangle(border);
            };

            pdfDocument.AddText(pdfText1);

            // ===== Section 4: Same text on landscape pages, centered, per-page purple border =====
            pdfDocument.SetPageSize(PdfPageSize.A4, PdfPageOrientation.Landscape);
            pdfDocument.AddPage();
            crtYPos = 0;

            PdfTextElement pdfText2 = new PdfTextElement(alfabetString, trueTypeFont)
            {
                X = xLeft,
                Y = crtYPos,
                Alignment = PdfTextAlignment.Center,
                ContinueOnNextPage = true
            };

            pdfText2.OnAfterPageRender = info =>
            {
                var bounds = info.RenderedRectangle.Bounds;
                PdfRectangleElement border = new PdfRectangleElement(bounds.X, bounds.Y,
                    bounds.Width, bounds.Height + 5)
                {
                    BorderColor = PdfColor.Purple,
                };
                pdfDocument.AddRectangle(border);
            };

            pdfDocument.AddText(pdfText2);

            // ===== Section 5: Right-to-left text =====
            pdfDocument.AddPage();
            crtYPos = 0;

            PdfTextElement rtlTitle = new PdfTextElement(
                "Add Right to Left Text", fontCourierNormalPurple)
            {
                X = xLeft,
                Y = crtYPos
            };
            crtYPos = (int)pdfDocument.AddText(rtlTitle).LastPageRectangle.Bounds.Bottom + ySeparator;

            string rtlFilePath = Path.Combine(textsPath, "RightToLeft.txt");
            string rtlString = System.IO.File.ReadAllText(rtlFilePath);

            string rtlFontFilePath = Path.Combine(fontsPath, "NotoSansArabic-Regular.ttf");
            PdfFont rtlTrueTypeFont = PdfFontManager.CreateFont(rtlFontFilePath, 16f,
                PdfFontStyle.Normal, PdfColor.Black);

            PdfTextElement pdfTextRtl = new PdfTextElement(rtlString, rtlTrueTypeFont)
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
            fileResult.FileDownloadName = "PdfDocument.pdf";

            return fileResult;
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
