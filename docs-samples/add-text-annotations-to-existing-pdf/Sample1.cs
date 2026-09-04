// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-text-annotations-to-existing-pdf.htm
// Documentation page: Add Text Annotations to Existing PDF

using System;
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
    public class Add_Text_Annotations_to_Existing_PDFController : Controller
    {
        private const int leftMargin = 36;
        private const int topMargin = 36;
        private const int contentWidth = 595 - 72;
        private const int contentHeight = 842 - 72;

        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Add_Text_Annotations_to_Existing_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPdf(Add_Text_Annotations_to_Existing_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
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

            // Open the loaded PDF for editing.  PdfEditor inherits the
            // standard (PDF/A, PDF/UA etc.) and Language from the source
            // document, so no PdfDocumentCreateSettings is needed.  Page
            // size and margins are also taken from the existing pages;
            // we apply our own margins manually below via leftMargin /
            // topMargin since PdfEditor draws at absolute page coordinates
            string password = string.IsNullOrEmpty(model.OwnerPassword) ? model.UserPassword : model.OwnerPassword;
            using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, password);
            pdfEditor.PdfDocumentInfo.Title = "PDF Text Annotations Demo";

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

            const int xLeft = leftMargin;
            const int ySeparator = 10;
            const int pageNumber = 1;
            int currentPage = 1;
            int crtYPos = topMargin;

            // ===== Title =====
            PdfTextElement titleElement = new PdfTextElement(
                "PDF Text Annotations Demo", titleFont)
            {
                X = xLeft,
                Y = crtYPos,
                Alignment = PdfTextAlignment.Center,
                Width = contentWidth
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            var titleElementInfo = pdfEditor.AddText(currentPage, titleElement);
            currentPage = titleElementInfo.LastPageRectangle.PageNumber;
            crtYPos = (int)titleElementInfo.LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 1: Icon variants =====
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "1. Icon variants (PdfTextAnnotationIcon)",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfEditor, ref currentPage, bodyFont,
                "PdfTextAnnotation.Icon selects the marker drawn on the page. Click any marker to open its popup",
                xLeft, crtYPos, contentWidth) + ySeparator;

            // Four icons in a row, equally spaced across the content width.
            // Each icon is the default 24x24 sticky-note size; the caption
            // beneath identifies the PdfTextAnnotationIcon enum value.
            const int iconSize = 24;
            int[] iconXs = { leftMargin + 30, leftMargin + 160, leftMargin + 290, leftMargin + 420 };

            AddIconSample(pdfEditor, ref currentPage, iconCaptionFont,
                contents: "PdfTextAnnotationIcon.Note  -  the default sticky-note marker, used for general comments",
                icon: PdfTextAnnotationIcon.Note,
                caption: "Note (default)",
                x: iconXs[0], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            AddIconSample(pdfEditor, ref currentPage, iconCaptionFont,
                contents: "PdfTextAnnotationIcon.Comment  -  used for review feedback in collaborative workflows",
                icon: PdfTextAnnotationIcon.Comment,
                caption: "Comment",
                x: iconXs[1], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            AddIconSample(pdfEditor, ref currentPage, iconCaptionFont,
                contents: "PdfTextAnnotationIcon.Help  -  used to flag content that needs clarification",
                icon: PdfTextAnnotationIcon.Help,
                caption: "Help",
                x: iconXs[2], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            AddIconSample(pdfEditor, ref currentPage, iconCaptionFont,
                contents: "PdfTextAnnotationIcon.Insert  -  used to suggest text insertions during editing",
                icon: PdfTextAnnotationIcon.Insert,
                caption: "Insert",
                x: iconXs[3], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            crtYPos += iconSize + 25 + ySeparator;

            // ===== Section 2: Custom dimensions =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 110, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "2. Custom marker dimensions",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfEditor, ref currentPage, bodyFont,
                "Width and Height in points override the default 24x24 marker size",
                xLeft, crtYPos, contentWidth) + ySeparator;

            // Three sizes laid out side by side: small, default, large.
            // The Y of each icon is adjusted so the bottoms align, which
            // makes the size difference easier to compare visually.
            const int baselineY = 0;
            const int sizeSmall = 16;
            const int sizeDefault = 24;
            const int sizeLarge = 48;

            AddSizedSample(pdfEditor, ref currentPage, iconCaptionFont,
                contents: "Small marker  -  Width = 16, Height = 16",
                size: sizeSmall,
                caption: "16 x 16",
                x: leftMargin + 30, baseY: crtYPos + sizeLarge - sizeSmall + baselineY,
                pageNumber: pageNumber, captionY: crtYPos + sizeLarge + 5);

            AddSizedSample(pdfEditor, ref currentPage, iconCaptionFont,
                contents: "Default marker  -  Width = 24, Height = 24",
                size: sizeDefault,
                caption: "24 x 24 (default)",
                x: leftMargin + 130, baseY: crtYPos + sizeLarge - sizeDefault + baselineY,
                pageNumber: pageNumber, captionY: crtYPos + sizeLarge + 5);

            AddSizedSample(pdfEditor, ref currentPage, iconCaptionFont,
                contents: "Large marker  -  Width = 48, Height = 48",
                size: sizeLarge,
                caption: "48 x 48",
                x: leftMargin + 270, baseY: crtYPos + baselineY,
                pageNumber: pageNumber, captionY: crtYPos + sizeLarge + 5);

            crtYPos += sizeLarge + 25 + ySeparator;

            // ===== Section 3: Author identification =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 120, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "3. Author identification",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfEditor, ref currentPage, bodyFont,
                "Author appears in the popup title and in the viewer's Comments panel, useful for tracking review feedback",
                xLeft, crtYPos, contentWidth) + ySeparator;

            // A body paragraph with a sticky note anchored at the end of a
            // specific phrase that the reviewer is commenting on.  The note
            // popup carries the reviewer's name so multiple reviewers can
            // be told apart in the Comments panel.
            const int paragraphX = leftMargin;
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
            var paraInfo = pdfEditor.AddText(currentPage, paragraph);
            currentPage = paraInfo.LastPageRectangle.PageNumber;
            float paraBottom = paraInfo.LastPageRectangle.Bounds.Bottom;

            // Place the sticky note at the right edge of the paragraph,
            // vertically aligned with its first line.
            AddTextAnnotationFull(pdfEditor, ref currentPage,
                contents: "Consider also setting CreationDate when persisting reviewer history. " +
                          "Some viewers sort the Comments panel chronologically.",
                author: "Jane Reviewer",
                icon: PdfTextAnnotationIcon.Comment,
                x: paragraphX + paragraphWidth + 10, y: crtYPos,
                pageNumber: pageNumber);

            crtYPos = (int)paraBottom + ySeparator * 2;

            // ===== Section 4: Initially expanded popup =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 80, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "4. Initially expanded popup (Open = true)",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfEditor, ref currentPage, bodyFont,
                "When Open is true the viewer shows the popup expanded as soon as the page is rendered, " +
                "without requiring the reader to click the icon",
                xLeft, crtYPos, contentWidth) + ySeparator;

            // Anchor the open-by-default note so the popup naturally
            // expands into empty space below.
            AddTextAnnotationFull(pdfEditor, ref currentPage,
                contents: "This popup is shown expanded on page load because Open = true was set on the annotation. " +
                          "Click the icon to collapse it",
                author: "EVO PDF Demo",
                icon: PdfTextAnnotationIcon.Note,
                x: leftMargin + 30, y: crtYPos,
                pageNumber: pageNumber, open: true);

            AddCaption(pdfEditor, ref currentPage, labelFont,
                "Marker on the left  -  its popup is visible immediately when the PDF is opened",
                leftMargin + 60, crtYPos + 6, 480);

            byte[] outPdfBuffer = pdfEditor.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfTextAnnotationsEditDemo.pdf";
            return fileResult;
        }

        // === Helpers ===

        // Adds a sticky-note annotation with the supplied icon at (x, y)
        // plus a bold caption beneath it identifying the icon name.
        // Used by Section 1 to show all four PdfTextAnnotationIcon values
        // in a horizontal row
        private void AddIconSample(PdfEditor editor, ref int currentPage, PdfFont captionFont,
            string contents, PdfTextAnnotationIcon icon, string caption,
            int x, int y, int pageNumber, int iconSize) {
            PdfTextAnnotation ann = PdfTextAnnotation.Create(contents, pageNumber, x, y);
            ann.Icon = icon;
            ann.Width = iconSize;
            ann.Height = iconSize;
            editor.AddTextAnnotation(ann);

            PdfTextElement label = new PdfTextElement(caption, captionFont)
            {
                X = x - 20, Y = y + iconSize + 4, Width = 100,
                Alignment = PdfTextAlignment.Left
            };
            editor.AddText(currentPage, label);
        }

        // Adds a sticky-note annotation sized via Width/Height plus a caption
        // beneath it identifying the dimensions.  baseY positions the icon's
        // top-left so the bottoms of differently sized icons can be aligned
        private void AddSizedSample(PdfEditor editor, ref int currentPage, PdfFont captionFont,
            string contents, int size, string caption,
            int x, int baseY, int pageNumber, int captionY) {
            PdfTextAnnotation ann = PdfTextAnnotation.Create(contents, pageNumber, x, baseY);
            ann.Icon = PdfTextAnnotationIcon.Note;
            ann.Width = size;
            ann.Height = size;
            editor.AddTextAnnotation(ann);

            PdfTextElement label = new PdfTextElement(caption, captionFont)
            {
                X = x - 10, Y = captionY, Width = 120
            };
            editor.AddText(currentPage, label);
        }

        // Adds a sticky-note annotation with the full set of properties.
        // Used by sections 3 and 4 where Author and Open matter
        private void AddTextAnnotationFull(PdfEditor editor, ref int currentPage,
            string contents, string author, PdfTextAnnotationIcon icon,
            int x, int y, int pageNumber, bool open = false) {
            PdfTextAnnotation ann = PdfTextAnnotation.Create(contents, pageNumber, x, y);
            ann.Icon = icon;
            ann.Author = author;
            ann.Open = open;
            editor.AddTextAnnotation(ann);
        }

        private int AddSectionLabel(PdfEditor editor, ref int currentPage, PdfFont sectionFont,
            string label, int x, int y, int separator) {
            PdfTextElement section = new PdfTextElement(label, sectionFont)
            { X = x, Y = y };
            section.Accessibility.StructureType = PdfStructureType.Heading2;
            var info = editor.AddText(currentPage, section);
            currentPage = info.LastPageRectangle.PageNumber;
            return (int)info.LastPageRectangle.Bounds.Bottom + separator;
        }

        private int AddCaption(PdfEditor editor, ref int currentPage, PdfFont labelFont,
            string caption, int x, int y, int width) {
            PdfTextElement t = new PdfTextElement(caption, labelFont)
            { X = x, Y = y, Width = width };
            t.Accessibility.StructureType = PdfStructureType.Artifact;
            var info = editor.AddText(currentPage, t);
            currentPage = info.LastPageRectangle.PageNumber;
            return (int)info.LastPageRectangle.Bounds.Bottom;
        }

        private void EnsureSpaceOnPage(ref int crtYPos, ref int currentPage, int requestedHeight, PdfEditor pdfEditor, int contentHeight, int topMargin)
        {
            if (crtYPos + requestedHeight > contentHeight + topMargin)
            {
                currentPage = pdfEditor.AddPage();
                crtYPos = topMargin;
            }
        }

        private Add_Text_Annotations_to_Existing_PDF_ViewModel SetViewModel()
        {
            var model = new Add_Text_Annotations_to_Existing_PDF_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(
                0, currentPageUrl.Length - "Add_Text_Annotations_to_Existing_PDF".Length);

            // Default input is empty.pdf so this demo edits a fresh
            // blank A4 page.  The user can upload another PDF or paste
            // a different URL
            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/empty.pdf";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
