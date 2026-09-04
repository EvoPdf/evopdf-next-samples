using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
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
    public class Add_Paths_and_Polygons_to_Existing_PDFController : Controller
    {
        private const int leftMargin = 36;
        private const int topMargin = 36;
        private const int contentWidth = 595 - 72;
        private const int contentHeight = 842 - 72;

        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Add_Paths_and_Polygons_to_Existing_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPdf(Add_Paths_and_Polygons_to_Existing_PDF_ViewModel model)
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
            pdfEditor.PdfDocumentInfo.Title = "PDF Polyline, Polygon and Path Demo";

            string fontsPath = GetDemoFontsPath();
            string fontFilePath = Path.Combine(fontsPath, "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);

            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 18f,
                PdfFontStyle.Bold | PdfFontStyle.Underline, PdfColor.Black);
            PdfFont sectionFont = PdfFontManager.CreateFont(baseFont, 14f,
                PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont labelFont = PdfFontManager.CreateFont(baseFont, 9f,
                PdfFontStyle.Normal, PdfColor.DarkGray);

            const int xLeft = leftMargin;
            const int ySeparator = 10;
            int currentPage = 1;
            int crtYPos = topMargin;

            // ===== Title =====
            PdfTextElement titleElement = new PdfTextElement(
                "PDF Polyline, Polygon and Path Demo", titleFont)
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

            // ===== Section 1: PdfPolylineElement (open, with caps) =====
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "1. PdfPolylineElement (zigzag, round joins)", xLeft, crtYPos, ySeparator);

            List<PdfPointF> zigzag = new List<PdfPointF>();
            for (int i = 0; i <= 12; i++)
            {
                float x = xLeft + i * 30;
                float y = crtYPos + (i % 2 == 0 ? 0 : 40);
                zigzag.Add(new PdfPointF(x, y));
            }

            var zigzagInfo = pdfEditor.AddPolyline(currentPage, new PdfPolylineElement(zigzag)
            {
                LineColor = PdfColor.DarkOrange,
                LineStyle = new PdfLineStyle
                {
                    LineWidth = 3f,
                    LineCap = PdfLineCapStyle.Round,
                    LineJoin = PdfLineJoinStyle.Round
                }
            });
            int zigzagCapBottom = AddCaption(pdfEditor, ref currentPage, labelFont,
                "12-segment zigzag, LineWidth=3, Round caps and joins",
                xLeft, (int)zigzagInfo.LastPageRectangle.Bounds.Bottom + 5, 400);
            crtYPos = zigzagCapBottom + ySeparator * 2;

            // Same path rotated
            List<PdfPointF> zigzag2 = new List<PdfPointF>();
            for (int i = 0; i <= 12; i++)
            {
                float x = xLeft + i * 30;
                float y = crtYPos + (i % 2 == 0 ? 0 : 30);
                zigzag2.Add(new PdfPointF(x, y));
            }
            var zigzag2Info = pdfEditor.AddPolyline(currentPage, new PdfPolylineElement(zigzag2)
            {
                LineColor = PdfColor.SteelBlue,
                LineStyle = new PdfLineStyle
                {
                    LineWidth = 2f,
                    DashStyle = PdfLineDashStyle.Dashed,
                    LineJoin = PdfLineJoinStyle.Miter
                },
                RotationDegrees = -6,
                RotationPivot = PdfRotationPivot.TopLeft
            });
            int zigzag2CapBottom = AddCaption(pdfEditor, ref currentPage, labelFont,
                "Same zigzag, dashed, rotated 6 deg clockwise",
                xLeft, (int)zigzag2Info.LastPageRectangle.Bounds.Bottom + 5, 400);
            crtYPos = zigzag2CapBottom + ySeparator * 2;

            // ===== Section 2: PdfPolygonElement (triangle, hexagon, star) =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 200, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "2. PdfPolygonElement (triangle, regular hexagon, 5-point star)",
                xLeft, crtYPos, ySeparator);

            // Triangle
            var triangleInfo = pdfEditor.AddPolygon(currentPage, new PdfPolygonElement(new PdfPointF(xLeft + 60, crtYPos),
                new PdfPointF(xLeft + 110, crtYPos + 90),
                new PdfPointF(xLeft + 10, crtYPos + 90))
            {
                FillColor = PdfColor.LightSalmon,
                FillOpacity = 0.7f,
                BorderColor = PdfColor.DarkRed,
                Border = new PdfLineStyle { LineWidth = 1.5f }
            });
            int triangleCapBottom = AddCaption(pdfEditor, ref currentPage, labelFont, "triangle (filled)", xLeft + 10,
                (int)triangleInfo.LastPageRectangle.Bounds.Bottom + 5, 110);

            // Regular hexagon
            var hexagonInfo = pdfEditor.AddPolygon(currentPage, new PdfPolygonElement(
                BuildRegularPolygon(xLeft + 200, crtYPos + 45, 50, 6, startAngleDeg: -90))
            {
                FillColor = PdfColor.LightGreen,
                FillOpacity = 0.6f,
                BorderColor = PdfColor.DarkGreen,
                Border = new PdfLineStyle
                {
                    LineWidth = 1.5f,
                    LineJoin = PdfLineJoinStyle.Round
                }
            });
            int hexagonCapBottom = AddCaption(pdfEditor, ref currentPage, labelFont, "regular hexagon", xLeft + 150,
                (int)hexagonInfo.LastPageRectangle.Bounds.Bottom + 5, 110);

            // 5-point star
            var starInfo = pdfEditor.AddPolygon(currentPage, new PdfPolygonElement(
                BuildStar(xLeft + 340, crtYPos + 45, outerRadius: 50, innerRadius: 22, points: 5))
            {
                FillColor = PdfColor.Gold,
                FillOpacity = 0.85f,
                BorderColor = PdfColor.DarkGoldenRod,
                Border = new PdfLineStyle { LineWidth = 1.2f, LineJoin = PdfLineJoinStyle.Miter, MiterLimit = 4f }
            });
            int starCapBottom = AddCaption(pdfEditor, ref currentPage, labelFont, "5-point star", xLeft + 295,
                (int)starInfo.LastPageRectangle.Bounds.Bottom + 5, 110);

            crtYPos = Math.Max(Math.Max(triangleCapBottom, hexagonCapBottom), starCapBottom) + ySeparator * 2;

            // ===== Section 3: PdfPathElement (heart via fluent API) =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 200, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "3. PdfPathElement (fluent MoveTo / LineTo / CurveTo / Close)",
                xLeft, crtYPos, ySeparator);

            // Heart shape built around (cx, cy) with size s.
            float cx = xLeft + 80;
            float cy = crtYPos + 80;
            float s = 60f;

            PdfPathElement heart = new PdfPathElement
            {
                FillColor = PdfColor.Crimson,
                FillOpacity = 0.8f,
                LineColor = PdfColor.DarkRed,
                LineStyle = new PdfLineStyle { LineWidth = 1.5f, LineJoin = PdfLineJoinStyle.Round }};

            heart
                .MoveTo(cx, cy + s * 0.25f)
                // Left (cubic Bezier upward to top center)
                .CurveTo(
                    cx - s * 0.55f, cy + s * 0.55f,
                    cx - s * 1.10f, cy - s * 0.10f,
                    cx, cy - s * 0.35f)
                // Right (cubic Bezier from top center back down)
                .CurveTo(
                    cx + s * 1.10f, cy - s * 0.10f,
                    cx + s * 0.55f, cy + s * 0.55f,
                    cx, cy + s * 0.25f)
                .Close();

            var heartInfo = pdfEditor.AddPath(currentPage, heart);
            int heartCapBottom = AddCaption(pdfEditor, ref currentPage, labelFont,
                "heart (two cubic Beziers + Close, FillColor set)",
                xLeft, (int)heartInfo.LastPageRectangle.Bounds.Bottom + 5, 250);

            // Wave
            PdfPathElement wave = new PdfPathElement
            {
                LineColor = PdfColor.SteelBlue,
                LineStyle = new PdfLineStyle { LineWidth = 2f, LineCap = PdfLineCapStyle.Round }};

            float waveStartX = xLeft + 200;
            float waveY = crtYPos + 70;
            wave.MoveTo(waveStartX, waveY);
            for (int i = 0; i < 3; i++)
            {
                float x0 = waveStartX + i * 80;
                wave.CurveTo(
                    x0 + 20, waveY - 30,
                    x0 + 60, waveY + 30,
                    x0 + 80, waveY);
            }

            var waveInfo = pdfEditor.AddPath(currentPage, wave);
            int waveCapBottom = AddCaption(pdfEditor, ref currentPage, labelFont,
                "stroke-only wave (3 cubic segments)",
                xLeft + 200, (int)waveInfo.LastPageRectangle.Bounds.Bottom + 5, 250);

            // Third triangle built via the params PdfPathOperation[] constructor
            float triX = xLeft + 460;
            float triY = crtYPos + 30;
            var staticTriangle = new PdfPathElement(
                PdfPathOperation.MoveTo(triX + 40, triY),
                PdfPathOperation.LineTo(triX + 80, triY + 70),
                PdfPathOperation.LineTo(triX, triY + 70),
                PdfPathOperation.Close())
            {
                FillColor = PdfColor.LightSeaGreen,
                FillOpacity = 0.6f,
                LineColor = PdfColor.Teal,
                LineStyle = new PdfLineStyle { LineWidth = 1.2f }
            };
            var triangleInfo3 = pdfEditor.AddPath(currentPage, staticTriangle);
            int triangle3CapBottom = AddCaption(pdfEditor, ref currentPage, labelFont,
                "static triangle (params PdfPathOperation[])",
                (int)(triX - 10), (int)triangleInfo3.LastPageRectangle.Bounds.Bottom + 5, 110);

            crtYPos = Math.Max(Math.Max(heartCapBottom, waveCapBottom), triangle3CapBottom) + ySeparator * 2;

            // ===== Section 4: Path with rotation =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 220, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "4. PdfPathElement (arrow shape rotated 0, 45, 90, 135 degrees)",
                xLeft, crtYPos, ySeparator);

            float[] angles = { 0, 45, 90, 135 };
            int arrowMaxBottom = crtYPos;
            for (int i = 0; i < angles.Length; i++)
            {
                int cellX = xLeft + i * 120;
                int cellY = crtYPos + 30;

                PdfPathElement arrow = new PdfPathElement
                {
                    FillColor = PdfColor.MediumPurple,
                    FillOpacity = 0.7f,
                    LineColor = PdfColor.Indigo,
                    LineStyle = new PdfLineStyle { LineWidth = 1f },
                    RotationDegrees = angles[i],
                    RotationPivot = PdfRotationPivot.Center
                };

                // Right-pointing arrow
                arrow
                    .MoveTo(cellX, cellY + 10)
                    .LineTo(cellX + 50, cellY + 10)
                    .LineTo(cellX + 50, cellY)
                    .LineTo(cellX + 80, cellY + 20)
                    .LineTo(cellX + 50, cellY + 40)
                    .LineTo(cellX + 50, cellY + 30)
                    .LineTo(cellX, cellY + 30)
                    .Close();

                var arrowInfo = pdfEditor.AddPath(currentPage, arrow);
                int capBottom = AddCaption(pdfEditor, ref currentPage, labelFont, $"{angles[i]} deg", cellX,
                    (int)arrowInfo.LastPageRectangle.Bounds.Bottom + 5, 80);
                arrowMaxBottom = Math.Max(arrowMaxBottom, capBottom);
            }
            crtYPos = arrowMaxBottom + ySeparator;

            byte[] outPdfBuffer = pdfEditor.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfPathsPolygonsEditDemo.pdf";
            return fileResult;
        }

        // Builds a regular N-sided polygon centered at (cx, cy) with circumscribed radius r
        private static List<PdfPointF> BuildRegularPolygon(float cx, float cy, float r, int sides, float startAngleDeg)
        {
            var pts = new List<PdfPointF>(sides);
            for (int i = 0; i < sides; i++)
            {
                double a = (startAngleDeg + i * 360.0 / sides) * Math.PI / 180.0;
                pts.Add(new PdfPointF(
                    (float)(cx + r * Math.Cos(a)),
                    (float)(cy + r * Math.Sin(a))));
            }
            return pts;
        }

        // Builds an N-point star centered at (cx, cy)
        private static List<PdfPointF> BuildStar(float cx, float cy, float outerRadius, float innerRadius, int points)
        {
            int total = points * 2;
            var pts = new List<PdfPointF>(total);
            for (int i = 0; i < total; i++)
            {
                float r = (i % 2 == 0) ? outerRadius : innerRadius;
                double a = (-90 + i * 360.0 / total) * Math.PI / 180.0;
                pts.Add(new PdfPointF(
                    (float)(cx + r * Math.Cos(a)),
                    (float)(cy + r * Math.Sin(a))));
            }
            return pts;
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

        private Add_Paths_and_Polygons_to_Existing_PDF_ViewModel SetViewModel()
        {
            var model = new Add_Paths_and_Polygons_to_Existing_PDF_ViewModel();

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
                0, currentPageUrl.Length - "Add_Paths_and_Polygons_to_Existing_PDF".Length);

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
