// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/create-pdf-documents-with-text-annotations.htm
// Documentation page: Create PDF Documents with Text Annotations

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
    public class Create_PDF_Documents_with_Text_AnnotationsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Create_PDF_Documents_with_Text_AnnotationsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = new Create_PDF_Documents_with_Text_Annotations_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePdf(Create_PDF_Documents_with_Text_Annotations_ViewModel model)
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
            pdfDocument.PdfDocumentInfo.Title = "PDF Text Annotations Demo";

            string fontsPath = GetDemoFontsPath();
            string fontFilePath = Path.Combine(fontsPath, "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);

            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 18f,
                PdfFontStyle.Bold | PdfFontStyle.Underline, PdfColor.Black);
            PdfFont sectionFont = PdfFontManager.CreateFont(baseFont, 14f,
                PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont bodyFont = PdfFontManager.CreateFont(baseFont, 11f,
                PdfFontStyle.Normal, PdfColor.Black);
            PdfFont labelFont = PdfFontManager.CreateFont(baseFont, 9f,
                PdfFontStyle.Normal, PdfColor.DarkGray);
            PdfFont iconCaptionFont = PdfFontManager.CreateFont(baseFont, 10f,
                PdfFontStyle.Bold, PdfColor.DarkSlateGray);

            const int xLeft = 0;
            const int ySeparator = 10;
            const int pageNumber = 1;
            int crtYPos = 0;

            // ===== Title =====
            PdfTextElement titleElement = new PdfTextElement(
                "PDF Text Annotations Demo", titleFont)
            {
                X = xLeft,
                Y = crtYPos,
                Alignment = PdfTextAlignment.Center,
                Width = pdfDocument.ContentWidth
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            crtYPos = (int)pdfDocument.AddText(titleElement).LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 1: Icon variants =====
            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "1. Icon variants (PdfTextAnnotationIcon)",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfDocument, bodyFont,
                "PdfTextAnnotation.Icon selects the marker drawn on the page. Click any marker to open its popup",
                xLeft, crtYPos, 540) + ySeparator;

            // Four icons in a row, equally spaced across the content width.
            // Each icon is the default 24x24 sticky-note size; the caption
            // beneath identifies the PdfTextAnnotationIcon enum value.
            const int iconSize = 24;
            int[] iconXs = { 30, 160, 290, 420 };

            AddIconSample(pdfDocument, iconCaptionFont,
                contents: "PdfTextAnnotationIcon.Note  -  the default sticky-note marker, used for general comments",
                icon: PdfTextAnnotationIcon.Note,
                caption: "Note (default)",
                x: iconXs[0], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            AddIconSample(pdfDocument, iconCaptionFont,
                contents: "PdfTextAnnotationIcon.Comment  -  used for review feedback in collaborative workflows",
                icon: PdfTextAnnotationIcon.Comment,
                caption: "Comment",
                x: iconXs[1], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            AddIconSample(pdfDocument, iconCaptionFont,
                contents: "PdfTextAnnotationIcon.Help  -  used to flag content that needs clarification",
                icon: PdfTextAnnotationIcon.Help,
                caption: "Help",
                x: iconXs[2], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            AddIconSample(pdfDocument, iconCaptionFont,
                contents: "PdfTextAnnotationIcon.Insert  -  used to suggest text insertions during editing",
                icon: PdfTextAnnotationIcon.Insert,
                caption: "Insert",
                x: iconXs[3], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            crtYPos += iconSize + 25 + ySeparator;

            // ===== Section 2: Author identification =====
            EnsureSpaceOnPage(ref crtYPos, 120, pdfDocument);
            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "2. Author identification",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfDocument, bodyFont,
                "Author appears in the popup title and in the viewer's Comments panel, useful for tracking review feedback",
                xLeft, crtYPos, 540) + ySeparator;

            // A body paragraph with a sticky note anchored at the end of a
            // specific phrase that the reviewer is commenting on.  The note
            // popup carries the reviewer's name so multiple reviewers can
            // be told apart in the Comments panel.
            const int paragraphX = 0;
            const int paragraphWidth = 500;
            PdfTextElement paragraph = new PdfTextElement(
                "The PdfTextAnnotation API supports an optional Author property " +
                "that is written to the annotation dictionary as /T. PDF viewers " +
                "show this value as the popup title and group comments by author " +
                "in the Comments panel.",
                bodyFont)
            {
                X = paragraphX, Y = crtYPos, Width = paragraphWidth
            };
            var paraInfo = pdfDocument.AddText(paragraph);
            float paraBottom = paraInfo.LastPageRectangle.Bounds.Bottom;

            // Place the sticky note at the right edge of the paragraph,
            // vertically aligned with its first line.
            AddTextAnnotationFull(pdfDocument,
                contents: "Consider also setting CreationDate when persisting reviewer history. " +
                          "Some viewers sort the Comments panel chronologically.",
                author: "Jane Reviewer",
                icon: PdfTextAnnotationIcon.Comment,
                x: paragraphWidth + 10, y: crtYPos,
                pageNumber: pageNumber);

            crtYPos = (int)paraBottom + ySeparator * 2;

            // ===== Section 3: Initially expanded popup =====
            EnsureSpaceOnPage(ref crtYPos, 80, pdfDocument);
            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "3. Initially expanded popup (Open = true)",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfDocument, bodyFont,
                "When Open is true the viewer shows the popup expanded as soon as the page is rendered, " +
                "without requiring the reader to click the icon",
                xLeft, crtYPos, 540) + ySeparator;

            // Anchor the open-by-default note so the popup naturally
            // expands into empty space below.
            AddTextAnnotationFull(pdfDocument,
                contents: "This popup is shown expanded on page load because Open = true was set on the annotation. " +
                          "Click the icon to collapse it",
                author: "EVO PDF Demo",
                icon: PdfTextAnnotationIcon.Note,
                x: 30, y: crtYPos,
                pageNumber: pageNumber, open: true);

            AddCaption(pdfDocument, labelFont,
                "Marker on the left  -  its popup is visible immediately when the PDF is opened",
                60, crtYPos + 6, 480);

            byte[] outPdfBuffer = pdfDocument.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfTextAnnotationsDemo.pdf";
            return fileResult;
        }

        // === Helpers ===

        // Adds a sticky-note annotation with the supplied icon at (x, y)
        // plus a bold caption beneath it identifying the icon name.
        // Used by Section 1 to show all four PdfTextAnnotationIcon values
        // in a horizontal row
        private void AddIconSample(
            PdfDocument doc, PdfFont captionFont,
            string contents, PdfTextAnnotationIcon icon, string caption,
            int x, int y, int pageNumber, int iconSize)
        {
            PdfTextAnnotation ann = PdfTextAnnotation.Create(contents, pageNumber, x, y);
            ann.Icon = icon;
            ann.Width = iconSize;
            ann.Height = iconSize;
            doc.AddTextAnnotation(ann);

            PdfTextElement label = new PdfTextElement(caption, captionFont)
            {
                X = x - 20, Y = y + iconSize + 4, Width = 100,
                Alignment = PdfTextAlignment.Left
            };
            doc.AddText(label);
        }

        // Adds a sticky-note annotation with the full set of properties.
        // Used by sections 2 and 3 where Author and Open matter
        private void AddTextAnnotationFull(
            PdfDocument doc,
            string contents, string author, PdfTextAnnotationIcon icon,
            int x, int y, int pageNumber, bool open = false)
        {
            PdfTextAnnotation ann = PdfTextAnnotation.Create(contents, pageNumber, x, y);
            ann.Icon = icon;
            ann.Author = author;
            ann.Open = open;
            doc.AddTextAnnotation(ann);
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
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
