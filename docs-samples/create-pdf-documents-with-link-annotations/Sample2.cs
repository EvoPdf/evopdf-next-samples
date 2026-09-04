// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/create-pdf-documents-with-link-annotations.htm
// Documentation page: Create PDF Documents with Link Annotations

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
    public class Create_PDF_Documents_with_Link_AnnotationsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Create_PDF_Documents_with_Link_AnnotationsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = new Create_PDF_Documents_with_Link_Annotations_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePdf(Create_PDF_Documents_with_Link_Annotations_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
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
            pdfDocument.PdfDocumentInfo.Title = "PDF Link Annotations Demo";

            string fontsPath = GetDemoFontsPath();
            string fontFilePath = Path.Combine(fontsPath, "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);

            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 18f,
                PdfFontStyle.Bold | PdfFontStyle.Underline, PdfColor.Black);
            PdfFont sectionFont = PdfFontManager.CreateFont(baseFont, 14f,
                PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont linkFont = PdfFontManager.CreateFont(baseFont, 11f,
                PdfFontStyle.Underline, PdfColor.MediumBlue);
            PdfFont labelFont = PdfFontManager.CreateFont(baseFont, 9f,
                PdfFontStyle.Normal, PdfColor.DarkGray);
            PdfFont anchorFont = PdfFontManager.CreateFont(baseFont, 11f,
                PdfFontStyle.Bold, PdfColor.DarkRed);

            const int xLeft = 0;
            const int ySeparator = 10;
            const int sourcePageNumber = 1;
            const int targetPageNumber = 2;
            int crtYPos = 0;

            // ===== Title =====
            PdfTextElement titleElement = new PdfTextElement(
                "PDF Link Annotations Demo", titleFont)
            {
                X = xLeft,
                Y = crtYPos,
                Alignment = PdfTextAlignment.Center,
                Width = pdfDocument.ContentWidth
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            crtYPos = (int)pdfDocument.AddText(titleElement).LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 1: PdfLinkAnnotation.FromUrl =====
            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "1. PdfLinkAnnotation.FromUrl (external URLs with border styles)",
                xLeft, crtYPos, ySeparator);

            // Text-style hyperlink - no border, the underlying underlined
            // blue text indicates the clickable area to the reader
            crtYPos = AddUrlLink(pdfDocument, linkFont, labelFont,
                visibleText: "Visit evopdf.com",
                url: "https://www.evopdf.com",
                description: "EvoPdf homepage",
                borderStyle: PdfLinkBorderStyle.None,
                captionText: "BorderStyle = None (typical text hyperlink)",
                x: xLeft, y: crtYPos, pageNumber: sourcePageNumber, separator: ySeparator);

            // Solid border - viewer renders a solid rectangle outline
            // around the hotspot (button-like appearance)
            crtYPos = AddUrlLink(pdfDocument, linkFont, labelFont,
                visibleText: "Visit github.com",
                url: "https://github.com",
                description: "GitHub homepage",
                borderStyle: PdfLinkBorderStyle.Solid,
                captionText: "BorderStyle = Solid, BorderWidth = 1",
                x: xLeft, y: crtYPos, pageNumber: sourcePageNumber, separator: ySeparator);

            // Dashed border - viewer renders a dashed rectangle outline
            crtYPos = AddUrlLink(pdfDocument, linkFont, labelFont,
                visibleText: "Visit wikipedia.org",
                url: "https://www.wikipedia.org",
                description: "Wikipedia main page",
                borderStyle: PdfLinkBorderStyle.Dashed,
                captionText: "BorderStyle = Dashed, BorderWidth = 1",
                x: xLeft, y: crtYPos, pageNumber: sourcePageNumber, separator: ySeparator);

            crtYPos += ySeparator;

            // ===== Section 2: PdfLinkAnnotation.ToPage =====
            EnsureSpaceOnPage(ref crtYPos, 80, pdfDocument);
            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "2. PdfLinkAnnotation.ToPage (jump to whole page)",
                xLeft, crtYPos, ySeparator);

            // Bordered link box pointing at page 2 with the implicit /Fit
            // destination produced by ToPage
            crtYPos = AddIntraDocLink(pdfDocument, linkFont, labelFont,
                visibleText: $"Jump to page {targetPageNumber} (whole page Fit)",
                description: $"Navigate to page {targetPageNumber}",
                linkFactory: (lx, ly, lw, lh) => PdfLinkAnnotation.ToPage(
                    targetPageNumber: targetPageNumber,
                    pageNumber: sourcePageNumber,
                    x: lx, y: ly, width: lw, height: lh),
                captionText: "Navigates to page 2 with the whole page fitted in the view",
                x: xLeft, y: crtYPos, separator: ySeparator);

            crtYPos += ySeparator;

            // ===== Section 3: PdfLinkAnnotation.ToPageLocation =====
            EnsureSpaceOnPage(ref crtYPos, 220, pdfDocument);
            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "3. PdfLinkAnnotation.ToPageLocation (Fit modes and explicit zoom)",
                xLeft, crtYPos, ySeparator);

            // FitPage() -- same effect as ToPage(), exposed through
            // PdfLinkPageLocation so the call site is symmetric with
            // the other fit variants
            crtYPos = AddPageLocationLink(pdfDocument, linkFont, labelFont,
                visibleText: "PdfLinkPageLocation.FitPage()",
                description: "Fit whole page in viewport (/Fit destination)",
                location: PdfLinkPageLocation.FitPage(),
                captionText: "Fits the whole target page in the viewport (equivalent to ToPage)",
                x: xLeft, y: crtYPos,
                pageNumber: sourcePageNumber, targetPageNumber: targetPageNumber,
                separator: ySeparator);

            // FitWidth(top=400) -- fits the page width in the viewport
            // and scrolls vertically so y=400 (from page top) is at the
            // top of the viewport
            crtYPos = AddPageLocationLink(pdfDocument, linkFont, labelFont,
                visibleText: "PdfLinkPageLocation.FitWidth(top = 400)",
                description: "Fit page width, scroll so y=400 is at top (fit width and scroll vertically)",
                location: PdfLinkPageLocation.FitWidth(top: 400f),
                captionText: "Fits the page WIDTH, scrolled so y=400 from page top is at the top of the viewport",
                x: xLeft, y: crtYPos,
                pageNumber: sourcePageNumber, targetPageNumber: targetPageNumber,
                separator: ySeparator);

            // FitHeight(left=200) -- fits the page height in the viewport
            // and scrolls horizontally so x=200 (from page left) is at
            // the left of the viewport
            crtYPos = AddPageLocationLink(pdfDocument, linkFont, labelFont,
                visibleText: "PdfLinkPageLocation.FitHeight(left = 200)",
                description: "Fit page height, scroll so x=200 is at left (fit height and scroll horizontally)",
                location: PdfLinkPageLocation.FitHeight(left: 200f),
                captionText: "Fits the page HEIGHT, scrolled so x=200 from page left is at the left of the viewport",
                x: xLeft, y: crtYPos,
                pageNumber: sourcePageNumber, targetPageNumber: targetPageNumber,
                separator: ySeparator);

            // AtCoordinates(50, 300, zoom=1.5) -- explicit position with
            // custom zoom factor.  The (50, 300) point on the target
            // page is placed at the top-left of the viewport and the
            // viewer applies a 150% zoom
            crtYPos = AddPageLocationLink(pdfDocument, linkFont, labelFont,
                visibleText: "PdfLinkPageLocation.AtCoordinates(50, 300, zoom = 1.5)",
                description: "Position (50,300) at viewport top-left with 150% zoom (explicit position with optional zoom)",
                location: PdfLinkPageLocation.AtCoordinates(left: 50f, top: 300f, zoom: 1.5f),
                captionText: "Positions (50,300) at the top-left of the viewport with explicit zoom = 1.5 (150%)",
                x: xLeft, y: crtYPos,
                pageNumber: sourcePageNumber, targetPageNumber: targetPageNumber,
                separator: ySeparator);

            // ===== TARGET PAGE -- landing points for intra-document links =====
            pdfDocument.AddPage();

            PdfTextElement targetTitle = new PdfTextElement(
                "Target Page  -  landing points for intra-document links", titleFont)
            {
                X = xLeft, Y = 0,
                Alignment = PdfTextAlignment.Center,
                Width = pdfDocument.ContentWidth
            };
            targetTitle.Accessibility.StructureType = PdfStructureType.Heading1;
            pdfDocument.AddText(targetTitle);

            // Top marker: landing point for FitPage() and ToPage()
            PdfTextElement topMarker = new PdfTextElement(
                "\u2191 Top of page \u2014 landing point for ToPage() and FitPage()", anchorFont)
            {
                X = xLeft, Y = 50, Width = pdfDocument.ContentWidth
            };
            pdfDocument.AddText(topMarker);

            // Horizontal reference line at y=400 for FitWidth(top=400)
            pdfDocument.AddLine(new PdfLineElement(
                xLeft, 400, xLeft + pdfDocument.ContentWidth, 400)
            {
                LineColor = PdfColor.DarkRed,
                LineStyle = new PdfLineStyle
                {
                    LineWidth = 0.8f,
                    DashStyle = PdfLineDashStyle.Dashed
                }
            });
            PdfTextElement fitWidthMarker = new PdfTextElement(
                "\u2192 y=400 \u2014 top of viewport when FitWidth(top = 400) is followed", anchorFont)
            {
                X = xLeft, Y = 405, Width = pdfDocument.ContentWidth
            };
            pdfDocument.AddText(fitWidthMarker);

            // Vertical reference line at x=200 for FitHeight(left=200)
            pdfDocument.AddLine(new PdfLineElement(
                200, 90, 200, pdfDocument.ContentHeight - 30)
            {
                LineColor = PdfColor.DarkRed,
                LineStyle = new PdfLineStyle
                {
                    LineWidth = 0.8f,
                    DashStyle = PdfLineDashStyle.Dashed
                }
            });
            PdfTextElement fitHeightMarker = new PdfTextElement(
                "\u2193 x=200 \u2014 left of viewport when FitHeight(left = 200) is followed", anchorFont)
            {
                X = 205, Y = 95, Width = 350
            };
            pdfDocument.AddText(fitHeightMarker);

            // Crosshair at (50, 300) for AtCoordinates(50, 300, zoom=1.5)
            pdfDocument.AddLine(new PdfLineElement(40, 300, 60, 300)
            {
                LineColor = PdfColor.DarkRed,
                LineStyle = new PdfLineStyle { LineWidth = 1.2f }
            });
            pdfDocument.AddLine(new PdfLineElement(50, 290, 50, 310)
            {
                LineColor = PdfColor.DarkRed,
                LineStyle = new PdfLineStyle { LineWidth = 1.2f }
            });
            PdfTextElement xyzMarker = new PdfTextElement(
                "+ (50, 300) \u2014 top-left of viewport when AtCoordinates(50, 300, zoom = 1.5) is followed",
                anchorFont)
            {
                X = 65, Y = 293, Width = 470
            };
            pdfDocument.AddText(xyzMarker);

            byte[] outPdfBuffer = pdfDocument.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfLinkAnnotationsDemo.pdf";
            return fileResult;
        }

        // === Helpers ===

        // Renders an external-URL link as underlined-blue text and attaches a
        // PdfLinkAnnotation that covers the rendered text bounds
        private int AddUrlLink(
            PdfDocument doc, PdfFont linkFont, PdfFont labelFont,
            string visibleText, string url, string description,
            PdfLinkBorderStyle borderStyle, string captionText,
            int x, int y, int pageNumber, int separator)
        {
            PdfTextElement t = new PdfTextElement(visibleText, linkFont) { X = x, Y = y };
            var info = doc.AddText(t);
            var tb = info.LastPageRectangle.Bounds;

            PdfLinkAnnotation link = PdfLinkAnnotation.FromUrl(
                url: url, pageNumber: pageNumber,
                x: tb.X, y: tb.Y, width: tb.Width, height: tb.Height);
            link.Description = description;
            link.BorderStyle = borderStyle;
            doc.AddLinkAnnotation(link);

            int capBottom = AddCaption(doc, labelFont, captionText,
                x, (int)tb.Bottom + 2, 450);
            return capBottom + separator;
        }

        // Renders an intra-document link as a bordered button with
        // underlined-blue text inside
        private int AddIntraDocLink(
            PdfDocument doc, PdfFont linkFont, PdfFont labelFont,
            string visibleText, string description,
            Func<float, float, float, float, PdfLinkAnnotation> linkFactory,
            string captionText,
            int x, int y, int separator)
        {
            const int linkW = 360;
            const int linkH = 22;

            // Visible bordered hotspot drawn explicitly so the click region
            // is obvious regardless of how the viewer renders /Border.
            doc.AddRectangle(new PdfRectangleElement(x, y, linkW, linkH)
            {
                BorderColor = PdfColor.SteelBlue,
                Border = new PdfLineStyle { LineWidth = 0.6f }
            });
            PdfTextElement t = new PdfTextElement(visibleText, linkFont)
            {
                X = x + 4, Y = y + 4, Width = linkW - 8
            };
            doc.AddText(t);

            PdfLinkAnnotation link = linkFactory(x, y, linkW, linkH);
            link.Description = description;
            doc.AddLinkAnnotation(link);

            int capBottom = AddCaption(doc, labelFont, captionText,
                x, y + linkH + 3, 540);
            return capBottom + separator;
        }

        // Convenience wrapper for ToPageLocation links -- builds the link
        // annotation from the supplied target page and PdfLinkPageLocation
        // then delegates to AddIntraDocLink for layout
        private int AddPageLocationLink(
            PdfDocument doc, PdfFont linkFont, PdfFont labelFont,
            string visibleText, string description,
            PdfLinkPageLocation location, string captionText,
            int x, int y, int pageNumber, int targetPageNumber, int separator)
        {
            return AddIntraDocLink(doc, linkFont, labelFont,
                visibleText: visibleText, description: description,
                linkFactory: (lx, ly, lw, lh) => PdfLinkAnnotation.ToPageLocation(
                    targetPageNumber: targetPageNumber, location: location,
                    pageNumber: pageNumber,
                    x: lx, y: ly, width: lw, height: lh),
                captionText: captionText,
                x: x, y: y, separator: separator);
        }

        private int AddSectionLabel(PdfDocument doc, PdfFont sectionFont,
            string label, int x, int y, int separator)
        {
            PdfTextElement section = new PdfTextElement(label, sectionFont)
            { X = x, Y = y };
            section.Accessibility.StructureType = PdfStructureType.Heading2;
            var info = doc.AddText(section);
            return (int)info.LastPageRectangle.Bounds.Bottom + separator;
        }

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

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoImagesPath() => Path.Combine(GetDemoFilesPath(), "Image_Files");
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
        private string GetDemoTextsPath() => Path.Combine(GetDemoFilesPath(), "Text_Files");
    }
}
