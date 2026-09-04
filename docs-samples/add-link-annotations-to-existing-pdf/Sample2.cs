// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-link-annotations-to-existing-pdf.htm
// Documentation page: Add Link Annotations to Existing PDF

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
    public class Add_Link_Annotations_to_Existing_PDFController : Controller
    {
        private const int leftMargin = 36;
        private const int topMargin = 36;
        private const int contentWidth = 595 - 72;
        private const int contentHeight = 842 - 72;

        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Add_Link_Annotations_to_Existing_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPdf(Add_Link_Annotations_to_Existing_PDF_ViewModel model)
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
            pdfEditor.PdfDocumentInfo.Title = "PDF Link Annotations Demo";

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

            const int xLeft = leftMargin;
            const int ySeparator = 10;
            const int sourcePageNumber = 1;
            const int targetPageNumber = 2;
            int currentPage = 1;
            int crtYPos = topMargin;

            // ===== Title =====
            PdfTextElement titleElement = new PdfTextElement(
                "PDF Link Annotations Demo", titleFont)
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

            // ===== Section 1: PdfLinkAnnotation.FromUrl =====
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "1. PdfLinkAnnotation.FromUrl (external URLs with border styles)",
                xLeft, crtYPos, ySeparator);

            // Text-style hyperlink - no border, the underlying underlined
            // blue text indicates the clickable area to the reader
            crtYPos = AddUrlLink(pdfEditor, ref currentPage, linkFont, labelFont,
                visibleText: "Visit evopdf.com",
                url: "https://www.evopdf.com",
                description: "EvoPdf homepage",
                borderStyle: PdfLinkBorderStyle.None,
                captionText: "BorderStyle = None (typical text hyperlink)",
                x: xLeft, y: crtYPos, pageNumber: sourcePageNumber, separator: ySeparator);

            // Solid border - viewer renders a solid rectangle outline
            // around the hotspot (button-like appearance)
            crtYPos = AddUrlLink(pdfEditor, ref currentPage, linkFont, labelFont,
                visibleText: "Visit github.com",
                url: "https://github.com",
                description: "GitHub homepage",
                borderStyle: PdfLinkBorderStyle.Solid,
                captionText: "BorderStyle = Solid, BorderWidth = 1",
                x: xLeft, y: crtYPos, pageNumber: sourcePageNumber, separator: ySeparator);

            // Dashed border - viewer renders a dashed rectangle outline
            crtYPos = AddUrlLink(pdfEditor, ref currentPage, linkFont, labelFont,
                visibleText: "Visit wikipedia.org",
                url: "https://www.wikipedia.org",
                description: "Wikipedia main page",
                borderStyle: PdfLinkBorderStyle.Dashed,
                captionText: "BorderStyle = Dashed, BorderWidth = 1",
                x: xLeft, y: crtYPos, pageNumber: sourcePageNumber, separator: ySeparator);

            crtYPos += ySeparator;

            // ===== Section 2: PdfLinkAnnotation.ToPage =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 80, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "2. PdfLinkAnnotation.ToPage (jump to whole page)",
                xLeft, crtYPos, ySeparator);

            // Bordered link box pointing at page 2 with the implicit /Fit
            // destination produced by ToPage
            crtYPos = AddIntraDocLink(pdfEditor, ref currentPage, linkFont, labelFont,
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
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 220, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "3. PdfLinkAnnotation.ToPageLocation (Fit modes and explicit zoom)",
                xLeft, crtYPos, ySeparator);

            // FitPage() -- same effect as ToPage(), exposed through
            // PdfLinkPageLocation so the call site is symmetric with
            // the other fit variants
            crtYPos = AddPageLocationLink(pdfEditor, ref currentPage, linkFont, labelFont,
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
            crtYPos = AddPageLocationLink(pdfEditor, ref currentPage, linkFont, labelFont,
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
            crtYPos = AddPageLocationLink(pdfEditor, ref currentPage, linkFont, labelFont,
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
            crtYPos = AddPageLocationLink(pdfEditor, ref currentPage, linkFont, labelFont,
                visibleText: "PdfLinkPageLocation.AtCoordinates(50, 300, zoom = 1.5)",
                description: "Position (50,300) at viewport top-left with 150% zoom (explicit position with optional zoom)",
                location: PdfLinkPageLocation.AtCoordinates(left: 50f, top: 300f, zoom: 1.5f),
                captionText: "Positions (50,300) at the top-left of the viewport with explicit zoom = 1.5 (150%)",
                x: xLeft, y: crtYPos,
                pageNumber: sourcePageNumber, targetPageNumber: targetPageNumber,
                separator: ySeparator);

            // ===== TARGET PAGE -- landing points for intra-document links =====
            currentPage = pdfEditor.AddPage();

            PdfTextElement targetTitle = new PdfTextElement(
                "Target Page  -  landing points for intra-document links", titleFont)
            {
                X = xLeft, Y = 0,
                Alignment = PdfTextAlignment.Center,
                Width = contentWidth
            };
            targetTitle.Accessibility.StructureType = PdfStructureType.Heading1;
            pdfEditor.AddText(currentPage, targetTitle);

            // Top marker: landing point for FitPage() and ToPage()
            PdfTextElement topMarker = new PdfTextElement(
                "\u2191 Top of page \u2014 landing point for ToPage() and FitPage()", anchorFont)
            {
                X = xLeft, Y = 50, Width = contentWidth
            };
            pdfEditor.AddText(currentPage, topMarker);

            // Horizontal reference line at y=400 for FitWidth(top=400)
            pdfEditor.AddLine(currentPage, new PdfLineElement(
                xLeft, 400, xLeft + contentWidth, 400)
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
                X = xLeft, Y = 405, Width = contentWidth
            };
            pdfEditor.AddText(currentPage, fitWidthMarker);

            // Vertical reference line at x=200 for FitHeight(left=200)
            pdfEditor.AddLine(currentPage, new PdfLineElement(
                200, 90, 200, contentHeight - 30)
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
            pdfEditor.AddText(currentPage, fitHeightMarker);

            // Crosshair at (50, 300) for AtCoordinates(50, 300, zoom=1.5)
            pdfEditor.AddLine(currentPage, new PdfLineElement(40, 300, 60, 300)
            {
                LineColor = PdfColor.DarkRed,
                LineStyle = new PdfLineStyle { LineWidth = 1.2f }
            });
            pdfEditor.AddLine(currentPage, new PdfLineElement(50, 290, 50, 310)
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
            pdfEditor.AddText(currentPage, xyzMarker);

            byte[] outPdfBuffer = pdfEditor.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfLinkAnnotationsEditDemo.pdf";
            return fileResult;
        }

        // === Helpers ===

        // Renders an external-URL link as underlined-blue text and attaches a
        // PdfLinkAnnotation that covers the rendered text bounds
        private int AddUrlLink(PdfEditor editor, ref int currentPage, PdfFont linkFont, PdfFont labelFont,
            string visibleText, string url, string description,
            PdfLinkBorderStyle borderStyle, string captionText,
            int x, int y, int pageNumber, int separator) {
            PdfTextElement t = new PdfTextElement(visibleText, linkFont) { X = x, Y = y };
            var info = editor.AddText(currentPage, t);
            currentPage = info.LastPageRectangle.PageNumber;
            var tb = info.LastPageRectangle.Bounds;

            PdfLinkAnnotation link = PdfLinkAnnotation.FromUrl(
                url: url, pageNumber: pageNumber,
                x: tb.X, y: tb.Y, width: tb.Width, height: tb.Height);
            link.Description = description;
            link.BorderStyle = borderStyle;
            editor.AddLinkAnnotation(link);

            int capBottom = AddCaption(editor, ref currentPage, labelFont, captionText,
                x, (int)tb.Bottom + 2, 450);
            return capBottom + separator;
        }

        // Renders an intra-document link as a bordered button with
        // underlined-blue text inside
        private int AddIntraDocLink(PdfEditor editor, ref int currentPage, PdfFont linkFont, PdfFont labelFont,
            string visibleText, string description,
            Func<float, float, float, float, PdfLinkAnnotation> linkFactory,
            string captionText,
            int x, int y, int separator) {
            const int linkW = 360;
            const int linkH = 22;

            // Visible bordered hotspot drawn explicitly so the click region
            // is obvious regardless of how the viewer renders /Border.
            editor.AddRectangle(currentPage, new PdfRectangleElement(x, y, linkW, linkH)
            {
                BorderColor = PdfColor.SteelBlue,
                Border = new PdfLineStyle { LineWidth = 0.6f }
            });
            PdfTextElement t = new PdfTextElement(visibleText, linkFont)
            {
                X = x + 4, Y = y + 4, Width = linkW - 8
            };
            editor.AddText(currentPage, t);

            PdfLinkAnnotation link = linkFactory(x, y, linkW, linkH);
            link.Description = description;
            editor.AddLinkAnnotation(link);

            int capBottom = AddCaption(editor, ref currentPage, labelFont, captionText,
                x, y + linkH + 3, contentWidth);
            return capBottom + separator;
        }

        // Convenience wrapper for ToPageLocation links -- builds the link
        // annotation from the supplied target page and PdfLinkPageLocation
        // then delegates to AddIntraDocLink for layout
        private int AddPageLocationLink(PdfEditor editor, ref int currentPage, PdfFont linkFont, PdfFont labelFont,
            string visibleText, string description,
            PdfLinkPageLocation location, string captionText,
            int x, int y, int pageNumber, int targetPageNumber, int separator) {
            return AddIntraDocLink(editor, ref currentPage, linkFont, labelFont,
                visibleText: visibleText, description: description,
                linkFactory: (lx, ly, lw, lh) => PdfLinkAnnotation.ToPageLocation(
                    targetPageNumber: targetPageNumber, location: location,
                    pageNumber: pageNumber,
                    x: lx, y: ly, width: lw, height: lh),
                captionText: captionText,
                x: x, y: y, separator: separator);
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

        private Add_Link_Annotations_to_Existing_PDF_ViewModel SetViewModel()
        {
            var model = new Add_Link_Annotations_to_Existing_PDF_ViewModel();

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
                0, currentPageUrl.Length - "Add_Link_Annotations_to_Existing_PDF".Length);

            // Default input is empty.pdf so this demo edits a fresh
            // blank A4 page.  The user can upload another PDF or paste
            // a different URL
            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/empty.pdf";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoImagesPath() => Path.Combine(GetDemoFilesPath(), "Image_Files");
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
        private string GetDemoTextsPath() => Path.Combine(GetDemoFilesPath(), "Text_Files");
    }
}
