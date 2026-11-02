using System;
using System.IO;
using System.Threading.Tasks;
using System.Text;
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
    public class Add_Text_to_Existing_PDFController : Controller
    {
        private const int leftMargin = 36;
        private const int topMargin = 36;
        // The content area of the pages inside the demo margins, from the size of the first page of the loaded document
        private int contentWidth;
        private int contentHeight;

        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Add_Text_to_Existing_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPdf(Add_Text_to_Existing_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

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

            // The pages of an edited document have no margins: the demo keeps its content inside its own margins
            string password = string.IsNullOrEmpty(model.OwnerPassword) ? model.UserPassword : model.OwnerPassword;
            using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, password);

            PdfPageInfo firstPageInfo = pdfEditor.GetPdfPageInfo(1);
            // A rotation of 90 or 270 degrees swaps the width and the height of the page as it is displayed
            bool firstPageTurned = firstPageInfo.Rotation % 180 != 0;
            contentWidth = (int)(firstPageTurned ? firstPageInfo.PageSize.Height : firstPageInfo.PageSize.Width) - 2 * leftMargin;
            contentHeight = (int)(firstPageTurned ? firstPageInfo.PageSize.Width : firstPageInfo.PageSize.Height) - 2 * topMargin;

            string fontsPath = GetDemoFontsPath();
            string fontFilePath = Path.Combine(fontsPath, "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);

            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 18f,
                PdfFontStyle.Bold | PdfFontStyle.Underline, PdfColor.Black);
            PdfFont sectionFont = PdfFontManager.CreateFont(baseFont, 14f,
                PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont bodyFont = PdfFontManager.CreateFont(baseFont, 11f,
                PdfFontStyle.Normal, PdfColor.Black);
            PdfFont smallFont = PdfFontManager.CreateFont(baseFont, 9f,
                PdfFontStyle.Normal, PdfColor.DarkGray);

            const int xLeft = leftMargin;
            const int ySeparator = 10;
            int currentPage = 1;
            int crtYPos = topMargin;

            // ===== Section 1: Title =====
            PdfTextElement titleElement = new PdfTextElement(
                "PDF Text Demo", titleFont)
            {
                X = xLeft,
                Y = crtYPos,
                Alignment = PdfTextAlignment.Center,
                Width = contentWidth
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            PdfTextRenderInfo titleInfo = pdfEditor.AddText(currentPage, titleElement);
            currentPage = titleInfo.LastPageRectangle.PageNumber;
            crtYPos = (int)titleInfo.LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 2: BackgroundColor + BackgroundOpacity =====
            PdfTextElement sectionLabel1 = new PdfTextElement(
                "1. BackgroundColor + BackgroundOpacity", sectionFont)
            { X = xLeft, Y = crtYPos };
            sectionLabel1.Accessibility.StructureType = PdfStructureType.Heading2;
            var sectionLabel1Info = pdfEditor.AddText(currentPage, sectionLabel1);
            currentPage = sectionLabel1Info.LastPageRectangle.PageNumber;
            crtYPos = (int)sectionLabel1Info.LastPageRectangle.Bounds.Bottom + ySeparator;

            PdfTextElement highlighted = new PdfTextElement(
                "This paragraph has a yellow highlight background drawn behind the text. The background " +
                "covers the full column width and the height used by the text.",
                bodyFont)
            {
                X = xLeft,
                Y = crtYPos,
                Width = contentWidth,
                BackgroundColor = PdfColor.Yellow,
                BackgroundOpacity = 0.4f
            };
            var highlightedInfo = pdfEditor.AddText(currentPage, highlighted);
            currentPage = highlightedInfo.LastPageRectangle.PageNumber;
            crtYPos = (int)highlightedInfo.LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 3: OnBeforePageRender + OnAfterPageRender =====
            PdfTextElement sectionLabel2 = new PdfTextElement(
                "2. OnBeforePageRender (under) + OnAfterPageRender (over)", sectionFont)
            { X = xLeft, Y = crtYPos };
            sectionLabel2.Accessibility.StructureType = PdfStructureType.Heading2;
            var sectionLabel2Info = pdfEditor.AddText(currentPage, sectionLabel2);
            currentPage = sectionLabel2Info.LastPageRectangle.PageNumber;
            crtYPos = (int)sectionLabel2Info.LastPageRectangle.Bounds.Bottom + ySeparator;

            PdfTextElement decoratedText = new PdfTextElement(
                "Below text is drawn an under-layer rectangle in the OnBeforePageRender callback, " +
                "while the OnAfterPageRender callback adds a thin border around the rendered area. " +
                "Both callbacks receive the same PdfTextPageRenderInfo type: the rectangle in " +
                "OnBeforePageRender is the area the text is about to use and in OnAfterPageRender " +
                "it is the area the text used.",
                bodyFont)
            {
                X = xLeft,
                Y = crtYPos,
                Width = contentWidth
            };

            // Under-layer painted before the text is drawn
            decoratedText.OnBeforePageRender = preInfo =>
            {
                var col = preInfo.RenderedRectangle.Bounds;
                pdfEditor.AddRectangle(preInfo.RenderedRectangle.PageNumber, new PdfRectangleElement(
                    col.X - 2, col.Y - 2, col.Width + 4, col.Height + 4)
                {
                    FillColor = PdfColor.LightBlue,
                    FillOpacity = 0.35f,
                    BorderColor = null
                });
            };

            // Post-render hook for the border that follows the actually used area
            decoratedText.OnAfterPageRender = postInfo =>
            {
                var rect = postInfo.RenderedRectangle.Bounds;
                pdfEditor.AddRectangle(postInfo.RenderedRectangle.PageNumber, new PdfRectangleElement(
                    rect.X - 2, rect.Y - 2, rect.Width + 4, rect.Height + 4)
                {
                    FillColor = null,
                    BorderColor = PdfColor.Blue,
                    Border = new PdfLineStyle { LineWidth = 1f }
                });
            };

            var decoratedTextInfo = pdfEditor.AddText(currentPage, decoratedText);
            currentPage = decoratedTextInfo.LastPageRectangle.PageNumber;
            crtYPos = (int)decoratedTextInfo.LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 4: Rotated text + QuadPoints outline =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 180, pdfEditor, contentHeight, topMargin);

            PdfTextElement sectionLabel3 = new PdfTextElement(
                "3. Rotated text with QuadPoints-based outline", sectionFont)
            { X = xLeft, Y = crtYPos };
            sectionLabel3.Accessibility.StructureType = PdfStructureType.Heading2;
            var sectionLabel3Info = pdfEditor.AddText(currentPage, sectionLabel3);
            currentPage = sectionLabel3Info.LastPageRectangle.PageNumber;
            crtYPos = (int)sectionLabel3Info.LastPageRectangle.Bounds.Bottom + ySeparator;

            PdfTextElement rotated = new PdfTextElement(
                "Rotated 25 degrees around the top-left corner. The polygon below follows the " +
                "rotation using the four-corner QuadPoints returned by PdfTextPageRenderInfo.",
                bodyFont)
            {
                X = xLeft + 60,
                Y = crtYPos + 10,
                Width = 350,
                RotationDegrees = -25,
                RotationPivot = PdfRotationPivot.TopLeft,
                BackgroundColor = PdfColor.LightYellow,
                BackgroundOpacity = 0.5f
            };

            // Trace a polygon along the rotated quad after rendering.
            rotated.OnAfterPageRender = postInfo =>
            {
                var quad = postInfo.RenderedRectangle.QuadPoints;
                pdfEditor.AddPolygon(postInfo.RenderedRectangle.PageNumber, new PdfPolygonElement(
                    quad.TopLeft, quad.TopRight, quad.BottomRight, quad.BottomLeft)
                {
                    FillColor = null,
                    BorderColor = PdfColor.Red,
                    Border = new PdfLineStyle { LineWidth = 1.2f, DashStyle = PdfLineDashStyle.Dashed }
                });
            };

            PdfTextRenderInfo rotatedInfo = pdfEditor.AddText(currentPage, rotated);

            currentPage = rotatedInfo.LastPageRectangle.PageNumber;
            // Advance Y past the rotated area's axis-aligned bounding box.
            crtYPos = (int)rotatedInfo.LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 4: Multi-page continuation + per-page RenderedText =====
            currentPage = pdfEditor.AddPage();
            crtYPos = topMargin;

            PdfTextElement sectionLabel4 = new PdfTextElement(
                "4. Multi-page continuation + per-page RenderedText", sectionFont)
            { X = xLeft, Y = crtYPos };
            sectionLabel4.Accessibility.StructureType = PdfStructureType.Heading2;
            var sectionLabel4Info = pdfEditor.AddText(currentPage, sectionLabel4);
            currentPage = sectionLabel4Info.LastPageRectangle.PageNumber;
            crtYPos = (int)sectionLabel4Info.LastPageRectangle.Bounds.Bottom + ySeparator;

            // A page added after the pages of the document: the text continues over it, or the pages where the
            // text continues are inserted before it and it keeps its content, as the option asks
            int pageAfterText = pdfEditor.AddPage();
            pdfEditor.AddText(pageAfterText, new PdfTextElement("The page that followed the text", smallFont)
            {
                X = xLeft,
                Y = topMargin
            });

            // A long text that takes several pages
            string longTextSource = LoadAlphabetText();
            StringBuilder longBuilder = new StringBuilder();
            for (int i = 0; i < 8; i++) longBuilder.AppendLine(longTextSource);
            string longText = longBuilder.ToString();

            // The pages of an edited document have no margins, so the continuation margins give the area of the
            // pages where the text continues
            PdfTextElement multipage = new PdfTextElement(longText, bodyFont)
            {
                X = xLeft,
                Y = crtYPos,
                Width = contentWidth,
                Alignment = PdfTextAlignment.Left,
                ContinueOnNextPage = true,
                InsertContinuationPages = model.InsertContinuationPages,
                ContinuationTopMargin = topMargin,
                ContinuationBottomMargin = topMargin,
                BackgroundColor = PdfColor.WhiteSmoke,
                BackgroundOpacity = 1f
            };

            // Per-page hook: draw a small page-number badge using OnAfterPageRender
            multipage.OnAfterPageRender = info =>
            {
                var r = info.RenderedRectangle.Bounds;
                string badge = $"page {info.RenderedRectangle.PageNumber} - {info.RenderedText.Length} chars";

                // Place the badge to the right of the rendered text top edge
                var badgeText = new PdfTextElement(badge, smallFont)
                {
                    X = (float)r.Right - 110,
                    Y = (float)r.Y - 12,
                    Width = 110,
                    Alignment = PdfTextAlignment.Right
                };
                badgeText.Accessibility.StructureType = PdfStructureType.Artifact;
                pdfEditor.AddText(info.RenderedRectangle.PageNumber, badgeText);
            };

            PdfTextRenderInfo multipageInfo = pdfEditor.AddText(currentPage, multipage);

            currentPage = multipageInfo.LastPageRectangle.PageNumber;

            // ===== Section 5: Summary of pages rendered =====
            currentPage = pdfEditor.AddPage();
            crtYPos = topMargin;

            PdfTextElement summaryLabel = new PdfTextElement(
                "5. Summary: text rendered per page (from Pages list)", sectionFont)
            { X = xLeft, Y = crtYPos };
            summaryLabel.Accessibility.StructureType = PdfStructureType.Heading2;
            var summaryLabelInfo = pdfEditor.AddText(currentPage, summaryLabel);
            currentPage = summaryLabelInfo.LastPageRectangle.PageNumber;
            crtYPos = (int)summaryLabelInfo.LastPageRectangle.Bounds.Bottom + ySeparator;

            // Walk the Pages list returned by the multipage render and report
            // the first 80 characters of each page
            for (int i = 0; i < multipageInfo.Pages.Count; i++)
            {
                var page = multipageInfo.Pages[i];
                string preview = page.RenderedText.Length > 80
                    ? page.RenderedText.Substring(0, 80).Replace("\n", " ").Replace("\r", "") + "..."
                    : page.RenderedText.Replace("\n", " ").Replace("\r", "");

                string entry = $"Page {page.RenderedRectangle.PageNumber}: " +
                               $"{page.RenderedText.Length} chars, " +
                               $"bounds=({page.RenderedRectangle.Bounds.X:F0}, " +
                               $"{page.RenderedRectangle.Bounds.Y:F0}, " +
                               $"{page.RenderedRectangle.Bounds.Width:F0}x" +
                               $"{page.RenderedRectangle.Bounds.Height:F0})  -  " +
                               $"\"{preview}\"";

                PdfTextElement entryElement = new PdfTextElement(entry, smallFont)
                {
                    X = xLeft,
                    Y = crtYPos,
                    Width = contentWidth
                };
                var entryElementInfo = pdfEditor.AddText(currentPage, entryElement);
                currentPage = entryElementInfo.LastPageRectangle.PageNumber;
                crtYPos = (int)entryElementInfo.LastPageRectangle.Bounds.Bottom + 4;

                EnsureSpaceOnPage(ref crtYPos, ref currentPage, 30, pdfEditor, contentHeight, topMargin);
            }

            byte[] outPdfBuffer = pdfEditor.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfTextEditDemo.pdf";
            return fileResult;
        }

        private string LoadAlphabetText()
        {
            string textsPath = GetDemoTextsPath();
            string alphabetFilePath = Path.Combine(textsPath, "Alphabet.txt");
            return System.IO.File.ReadAllText(alphabetFilePath);
        }

        private void EnsureSpaceOnPage(ref int crtYPos, ref int currentPage, int requestedHeight, PdfEditor pdfEditor, int contentHeight, int topMargin)
        {
            if (crtYPos + requestedHeight > contentHeight + topMargin)
            {
                // The content continues on the following page of the document, or on a new page after the last one
                currentPage = currentPage < pdfEditor.PageCount ? currentPage + 1 : pdfEditor.AddPage();
                crtYPos = topMargin;
            }
        }

        private Add_Text_to_Existing_PDF_ViewModel SetViewModel()
        {
            var model = new Add_Text_to_Existing_PDF_ViewModel();

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
                0, currentPageUrl.Length - "Add_Text_to_Existing_PDF".Length);

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
