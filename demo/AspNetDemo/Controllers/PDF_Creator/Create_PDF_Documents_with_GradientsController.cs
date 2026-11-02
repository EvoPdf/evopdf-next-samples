using System;
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
    public class Create_PDF_Documents_with_GradientsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Create_PDF_Documents_with_GradientsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = new Create_PDF_Documents_with_Gradients_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePdf(Create_PDF_Documents_with_Gradients_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
            {
                PageSize = PdfPageSize.A4,
                PageOrientation = PdfPageOrientation.Portrait,
                Margins = new PdfMargins(36, 36, 36, 36),
                PdfStandard = model.PdfStandard,
                Language = "en-US"
            };

            using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);
            pdfDocument.PdfDocumentInfo.Title = "PDF Gradient Fills Demo";

            string fontFilePath = Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);

            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 18f, PdfFontStyle.Bold | PdfFontStyle.Underline, PdfColor.Black);
            PdfFont sectionFont = PdfFontManager.CreateFont(baseFont, 14f, PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont labelFont = PdfFontManager.CreateFont(baseFont, 9f, PdfFontStyle.Normal, PdfColor.DarkGray);
            PdfFont badgeFont = PdfFontManager.CreateFont(baseFont, 11f, PdfFontStyle.Bold, PdfColor.White);

            // The opacity applies to the gradient fill the same way it applies to a solid fill
            float fillOpacity = Math.Max(0f, Math.Min(1f, model.FillOpacity));

            const int xLeft = 0;
            const int ySeparator = 10;
            int crtYPos = 0;

            // ===== Title =====
            PdfTextElement titleElement = new PdfTextElement("Gradient Fills Demo", titleFont)
            {
                X = xLeft,
                Y = crtYPos,
                Alignment = PdfTextAlignment.Center,
                Width = pdfDocument.ContentWidth
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            crtYPos = (int)pdfDocument.AddText(titleElement).LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 1: the four gradient directions on rectangles =====
            crtYPos = AddSectionLabel(pdfDocument, sectionFont, "1. PdfGradientFill directions on PdfRectangleElement", xLeft, crtYPos, ySeparator);

            PdfGradientDirection[] directions =
            {
                PdfGradientDirection.Vertical, PdfGradientDirection.Horizontal,
                PdfGradientDirection.ForwardDiagonal, PdfGradientDirection.BackwardDiagonal
            };
            int rowBottom = crtYPos;
            for (int i = 0; i < directions.Length; i++)
            {
                int boxX = xLeft + i * 130;

                // A gradient fill takes the place of the solid fill color: the shape is painted
                // from the start color to the end color along the given direction
                var rectangle = new PdfRectangleElement(boxX, crtYPos, 110, 60)
                {
                    GradientFill = new PdfGradientFill(PdfColor.LightSkyBlue, PdfColor.DarkBlue, directions[i]),
                    FillOpacity = fillOpacity,
                    BorderColor = PdfColor.DarkBlue,
                    Border = new PdfLineStyle { LineWidth = 1f }
                };
                var rectangleInfo = pdfDocument.AddRectangle(rectangle);
                int captionBottom = AddCaption(pdfDocument, labelFont, directions[i].ToString(), boxX,
                    (int)rectangleInfo.LastPageRectangle.Bounds.Bottom + 5, 110);
                rowBottom = Math.Max(rowBottom, captionBottom);
            }
            crtYPos = rowBottom + ySeparator;

            // ===== Section 2: rounded rectangles, circles and ellipses =====
            crtYPos = AddSectionLabel(pdfDocument, sectionFont, "2. Gradients on rounded rectangles, circles and ellipses", xLeft, crtYPos, ySeparator);

            var roundedInfo = pdfDocument.AddRoundedRectangle(new PdfRoundedRectangleElement(xLeft, crtYPos, 140, 70, 16f)
            {
                GradientFill = new PdfGradientFill(PdfColor.Gold, PdfColor.DarkOrange, PdfGradientDirection.ForwardDiagonal),
                FillOpacity = fillOpacity,
                BorderColor = PdfColor.DarkOrange,
                Border = new PdfLineStyle { LineWidth = 1f }
            });
            int roundedCaptionBottom = AddCaption(pdfDocument, labelFont, "rounded rectangle, forward diagonal", xLeft,
                (int)roundedInfo.LastPageRectangle.Bounds.Bottom + 5, 160);

            var circleInfo = pdfDocument.AddCircle(new PdfCircleElement(xLeft + 215, crtYPos + 35, 35)
            {
                GradientFill = new PdfGradientFill(PdfColor.White, PdfColor.Crimson, PdfGradientDirection.Vertical),
                FillOpacity = fillOpacity,
                BorderColor = PdfColor.Crimson,
                Border = new PdfLineStyle { LineWidth = 1f }
            });
            int circleCaptionBottom = AddCaption(pdfDocument, labelFont, "circle, vertical", xLeft + 180,
                (int)circleInfo.LastPageRectangle.Bounds.Bottom + 5, 100);

            var ellipseInfo = pdfDocument.AddEllipse(new PdfEllipseElement(xLeft + 300, crtYPos, 150, 70)
            {
                GradientFill = new PdfGradientFill(PdfColor.LightGreen, PdfColor.DarkGreen, PdfGradientDirection.Horizontal),
                FillOpacity = fillOpacity,
                BorderColor = PdfColor.DarkGreen,
                Border = new PdfLineStyle { LineWidth = 1f },
                RotationDegrees = 20,
                RotationPivot = PdfRotationPivot.Center
            });
            int ellipseCaptionBottom = AddCaption(pdfDocument, labelFont, "rotated ellipse, horizontal", xLeft + 300,
                (int)ellipseInfo.LastPageRectangle.Bounds.Bottom + 5, 150);

            crtYPos = Math.Max(Math.Max(roundedCaptionBottom, circleCaptionBottom), ellipseCaptionBottom) + ySeparator;

            // ===== Section 3: arcs, polygons and paths =====
            crtYPos = AddSectionLabel(pdfDocument, sectionFont, "3. Gradients on closed arcs, polygons and paths", xLeft, crtYPos, ySeparator);

            // Only a closed arc (chord or pie) has an area to fill
            var pieInfo = pdfDocument.AddArc(new PdfArcElement(xLeft, crtYPos, 120, 90, startAngleDegrees: 30, sweepAngleDegrees: 240)
            {
                Closure = PdfArcClosureType.Pie,
                GradientFill = new PdfGradientFill(PdfColor.Plum, PdfColor.Purple, PdfGradientDirection.BackwardDiagonal),
                FillOpacity = fillOpacity,
                BorderColor = PdfColor.Purple,
                Border = new PdfLineStyle { LineWidth = 1f }
            });
            int pieCaptionBottom = AddCaption(pdfDocument, labelFont, "pie arc, backward diagonal", xLeft,
                (int)pieInfo.LastPageRectangle.Bounds.Bottom + 5, 130);

            float hexagonCenterX = xLeft + 220, hexagonCenterY = crtYPos + 45, hexagonRadius = 45;
            var hexagonPoints = new PdfPointF[6];
            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i;
                hexagonPoints[i] = new PdfPointF(hexagonCenterX + (float)(hexagonRadius * Math.Cos(angle)),
                    hexagonCenterY + (float)(hexagonRadius * Math.Sin(angle)));
            }
            var hexagonInfo = pdfDocument.AddPolygon(new PdfPolygonElement(hexagonPoints)
            {
                GradientFill = new PdfGradientFill(PdfColor.LightCyan, PdfColor.Teal, PdfGradientDirection.Vertical),
                FillOpacity = fillOpacity,
                BorderColor = PdfColor.Teal,
                Border = new PdfLineStyle { LineWidth = 1f }
            });
            int hexagonCaptionBottom = AddCaption(pdfDocument, labelFont, "hexagon polygon, vertical", xLeft + 175,
                (int)hexagonInfo.LastPageRectangle.Bounds.Bottom + 5, 120);

            // A heart drawn with two cubic curves; the gradient fills the closed path
            float cx = xLeft + 400, cy = crtYPos + 50, s = 55;
            PdfPathElement heart = new PdfPathElement
            {
                GradientFill = new PdfGradientFill(PdfColor.LightPink, PdfColor.DarkRed, PdfGradientDirection.Vertical),
                FillOpacity = fillOpacity,
                BorderColor = PdfColor.DarkRed,
                Border = new PdfLineStyle { LineWidth = 1f, LineJoin = PdfLineJoinStyle.Round }
            };
            heart.MoveTo(cx, cy + s * 0.25f)
                .CurveTo(cx - s * 0.55f, cy + s * 0.55f, cx - s * 1.10f, cy - s * 0.10f, cx, cy - s * 0.35f)
                .CurveTo(cx + s * 1.10f, cy - s * 0.10f, cx + s * 0.55f, cy + s * 0.55f, cx, cy + s * 0.25f)
                .Close();
            var heartInfo = pdfDocument.AddPath(heart);
            int heartCaptionBottom = AddCaption(pdfDocument, labelFont, "heart path, vertical", xLeft + 345,
                (int)heartInfo.LastPageRectangle.Bounds.Bottom + 5, 120);

            crtYPos = Math.Max(Math.Max(pieCaptionBottom, hexagonCaptionBottom), heartCaptionBottom) + ySeparator;

            // ===== Section 4: badges sized with the measured text width =====
            crtYPos = AddSectionLabel(pdfDocument, sectionFont, "4. Badges sized with PdfFont.GetTextWidth", xLeft, crtYPos, ySeparator);

            string[] badgeTexts = { "NEW", "GRADIENT FILL", "PdfFont.GetTextWidth", "PDF" };
            PdfColor[] badgeStartColors = { PdfColor.LightGreen, PdfColor.LightSkyBlue, PdfColor.Gold, PdfColor.Plum };
            PdfColor[] badgeEndColors = { PdfColor.DarkGreen, PdfColor.DarkBlue, PdfColor.DarkOrange, PdfColor.Purple };
            float badgeX = xLeft;
            const float badgeHeight = 24f, badgePadding = 10f;
            for (int i = 0; i < badgeTexts.Length; i++)
            {
                // The width of the text written with the font gives the width of its badge
                float textWidth = badgeFont.GetTextWidth(badgeTexts[i]);
                float badgeWidth = textWidth + 2 * badgePadding;

                pdfDocument.AddRoundedRectangle(new PdfRoundedRectangleElement(badgeX, crtYPos, badgeWidth, badgeHeight, badgeHeight / 2)
                {
                    GradientFill = new PdfGradientFill(badgeStartColors[i], badgeEndColors[i], PdfGradientDirection.Vertical),
                    FillOpacity = fillOpacity,
                    BorderColor = badgeEndColors[i],
                    Border = new PdfLineStyle { LineWidth = 0.5f }
                });

                // The text is centered in the badge from the measured width
                PdfTextElement badgeText = new PdfTextElement(badgeTexts[i], badgeFont)
                {
                    X = badgeX + badgePadding,
                    Y = crtYPos + (badgeHeight - badgeFont.Size) / 2 - 1,
                    Width = textWidth + 2
                };
                pdfDocument.AddText(badgeText);

                badgeX += badgeWidth + 12;
            }
            crtYPos += (int)badgeHeight + 5;
            crtYPos = AddCaption(pdfDocument, labelFont, "each badge is as wide as its text plus the padding", xLeft, crtYPos, 300) + ySeparator;

            byte[] outPdfBuffer = pdfDocument.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfGradientsDemo.pdf";
            return fileResult;
        }

        private int AddSectionLabel(PdfDocument pdfDocument, PdfFont sectionFont, string label, int x, int y, int separator)
        {
            PdfTextElement section = new PdfTextElement(label, sectionFont) { X = x, Y = y };
            section.Accessibility.StructureType = PdfStructureType.Heading2;
            return (int)pdfDocument.AddText(section).LastPageRectangle.Bounds.Bottom + separator;
        }

        private int AddCaption(PdfDocument pdfDocument, PdfFont labelFont, string caption, int x, int y, int width)
        {
            PdfTextElement text = new PdfTextElement(caption, labelFont) { X = x, Y = y, Width = width };
            text.Accessibility.StructureType = PdfStructureType.Artifact;
            return (int)pdfDocument.AddText(text).LastPageRectangle.Bounds.Bottom;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
