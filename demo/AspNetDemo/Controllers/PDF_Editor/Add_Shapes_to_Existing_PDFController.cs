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
    public class Add_Shapes_to_Existing_PDFController : Controller
    {
        private const int leftMargin = 36;
        private const int topMargin = 36;
        private const int contentWidth = 595 - 72;
        private const int contentHeight = 842 - 72;

        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Add_Shapes_to_Existing_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPdf(Add_Shapes_to_Existing_PDF_ViewModel model)
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
            pdfEditor.PdfDocumentInfo.Title = "PDF Geometric Shapes Demo";

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
                "Geometric Shape Elements Demo", titleFont)
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

            // ===== Section 1: PdfRectangleElement =====
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "1. PdfRectangleElement (basic, dashed, rotated)", xLeft, crtYPos, ySeparator);

            // Filled basic
            var basicInfo = pdfEditor.AddRectangle(currentPage, new PdfRectangleElement(xLeft, crtYPos, 100, 60)
            {
                FillColor = PdfColor.LightSkyBlue,
                BorderColor = PdfColor.SteelBlue,
                Border = new PdfLineStyle { LineWidth = 1.5f }
            });
            int basicCaptionBottom = AddCaption(pdfEditor, ref currentPage, labelFont,
                "filled + border", xLeft,
                (int)basicInfo.LastPageRectangle.Bounds.Bottom + 5, 100);

            // Dashed
            var dashedInfo = pdfEditor.AddRectangle(currentPage, new PdfRectangleElement(xLeft + 120, crtYPos, 100, 60)
            {
                FillColor = null,
                BorderColor = PdfColor.DarkOrange,
                Border = new PdfLineStyle
                {
                    LineWidth = 1.5f,
                    DashStyle = PdfLineDashStyle.Dashed
                }
            });
            int dashedCaptionBottom = AddCaption(pdfEditor, ref currentPage, labelFont,
                "dashed outline", xLeft + 120,
                (int)dashedInfo.LastPageRectangle.Bounds.Bottom + 5, 100);

            // Rotated 30 degrees
            var rotatedInfo = pdfEditor.AddRectangle(currentPage, new PdfRectangleElement(xLeft + 320, crtYPos + 5, 100, 60)
            {
                FillColor = PdfColor.LightPink,
                FillOpacity = 0.6f,
                BorderColor = PdfColor.Crimson,
                Border = new PdfLineStyle { LineWidth = 1.2f },
                RotationDegrees = 30,
                RotationPivot = PdfRotationPivot.TopLeft
            });
            int rotatedCaptionBottom = AddCaption(pdfEditor, ref currentPage, labelFont,
                "rotated 30 deg (TopLeft pivot)", xLeft + 320,
                (int)rotatedInfo.LastPageRectangle.Bounds.Bottom + 5, 150);

            // Advance past the deepest caption in this row
            crtYPos = Math.Max(
                Math.Max(basicCaptionBottom, dashedCaptionBottom),
                rotatedCaptionBottom) + ySeparator;

            // ===== Section 2: PdfRoundedRectangleElement =====
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "2. PdfRoundedRectangleElement (varying corner radius)", xLeft, crtYPos, ySeparator);

            float[] radii = { 4f, 12f, 24f, 30f };
            int section2MaxBottom = crtYPos;
            for (int i = 0; i < radii.Length; i++)
            {
                int boxX = xLeft + i * 130;
                var rrInfo = pdfEditor.AddRoundedRectangle(currentPage, new PdfRoundedRectangleElement(boxX, crtYPos, 110, 60, radii[i])
                {
                    FillColor = PdfColor.LightGreen,
                    FillOpacity = 0.5f,
                    BorderColor = PdfColor.DarkGreen,
                    Border = new PdfLineStyle { LineWidth = 1f }
                });
                int capBottom = AddCaption(pdfEditor, ref currentPage, labelFont, $"radius={radii[i]}", boxX,
                    (int)rrInfo.LastPageRectangle.Bounds.Bottom + 5, 110);
                section2MaxBottom = Math.Max(section2MaxBottom, capBottom);
            }
            crtYPos = section2MaxBottom + ySeparator;

            // ===== Section 3: PdfLineElement (LineCap variations) =====
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "3. PdfLineElement (LineCap: Butt, Round, ProjectingSquare)", xLeft, crtYPos, ySeparator);

            PdfLineCapStyle[] caps = { PdfLineCapStyle.Butt, PdfLineCapStyle.Round, PdfLineCapStyle.ProjectingSquare };
            string[] capLabels = { "Butt", "Round", "ProjectingSquare" };
            int section3RowTop = crtYPos;
            for (int i = 0; i < caps.Length; i++)
            {
                int lineY = section3RowTop + 8;
                var lineInfo = pdfEditor.AddLine(currentPage, new PdfLineElement(xLeft + 60, lineY, xLeft + 260, lineY)
                {
                    LineColor = PdfColor.DarkBlue,
                    LineStyle = new PdfLineStyle
                    {
                        LineWidth = 8f,
                        LineCap = caps[i]
                    }
                });
                // Thin reference line to highlight the cap extension past the endpoints
                pdfEditor.AddLine(currentPage, new PdfLineElement(xLeft + 60, lineY, xLeft + 260, lineY)
                {
                    LineColor = PdfColor.White,
                    LineStyle = new PdfLineStyle { LineWidth = 0.5f }
                });
                int capBottom = AddCaption(pdfEditor, ref currentPage, labelFont, capLabels[i], xLeft + 270, lineY - 5, 150);
                int lineBottom = (int)lineInfo.LastPageRectangle.Bounds.Bottom;
                section3RowTop = Math.Max(lineBottom, capBottom) + 5;
            }
            crtYPos = section3RowTop + ySeparator;

            // Dashed line using CustomDashPattern
            var dashLineInfo = pdfEditor.AddLine(currentPage, new PdfLineElement(xLeft, crtYPos, xLeft + 380, crtYPos)
            {
                LineColor = PdfColor.DarkRed,
                LineStyle = new PdfLineStyle
                {
                    LineWidth = 2f,
                    CustomDashPattern = new float[] { 10f, 4f, 2f, 4f },
                    DashPhase = 0f
                }
            });
            int dashCapBottom = AddCaption(pdfEditor, ref currentPage, labelFont, "CustomDashPattern [10, 4, 2, 4]",
                xLeft, (int)dashLineInfo.LastPageRectangle.Bounds.Bottom + 5, 250);
            crtYPos = dashCapBottom + ySeparator;

            // ===== Section 4: PdfCircleElement =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 160, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "4. PdfCircleElement (filled, stroked, opacity)", xLeft, crtYPos, ySeparator);

            var circle1Info = pdfEditor.AddCircle(currentPage, new PdfCircleElement(xLeft + 40, crtYPos + 40, 30)
            {
                FillColor = PdfColor.Tomato,
                BorderColor = PdfColor.DarkRed,
                Border = new PdfLineStyle { LineWidth = 1.5f }
            });
            int circle1CapBottom = AddCaption(pdfEditor, ref currentPage, labelFont, "filled + border", xLeft + 10,
                (int)circle1Info.LastPageRectangle.Bounds.Bottom + 5, 90);

            var circle2Info = pdfEditor.AddCircle(currentPage, new PdfCircleElement(xLeft + 140, crtYPos + 40, 30)
            {
                FillColor = null,
                BorderColor = PdfColor.DarkBlue,
                Border = new PdfLineStyle { LineWidth = 2f }
            });
            int circle2CapBottom = AddCaption(pdfEditor, ref currentPage, labelFont, "stroke only", xLeft + 110,
                (int)circle2Info.LastPageRectangle.Bounds.Bottom + 5, 90);

            var circle3Info = pdfEditor.AddCircle(currentPage, new PdfCircleElement(xLeft + 240, crtYPos + 40, 30)
            {
                FillColor = PdfColor.Purple,
                FillOpacity = 0.4f,
                BorderColor = PdfColor.Purple,
                BorderOpacity = 0.7f,
                Border = new PdfLineStyle { LineWidth = 1f }
            });
            int circle3CapBottom = AddCaption(pdfEditor, ref currentPage, labelFont, "translucent", xLeft + 210,
                (int)circle3Info.LastPageRectangle.Bounds.Bottom + 5, 90);

            crtYPos = Math.Max(Math.Max(circle1CapBottom, circle2CapBottom), circle3CapBottom) + ySeparator;

            // ===== Section 5: PdfEllipseElement (rotation, axis-aligned vs QuadPoints) =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 180, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "5. PdfEllipseElement rotation - Bounds (axis-aligned) vs QuadPoints (rotated)", xLeft, crtYPos, ySeparator);

            // Axis-aligned ellipse
            var axisEllipseInfo = pdfEditor.AddEllipse(currentPage, new PdfEllipseElement(xLeft, crtYPos + 10, 120, 70)
            {
                FillColor = PdfColor.LightYellow,
                BorderColor = PdfColor.GoldenRod,
                Border = new PdfLineStyle { LineWidth = 1f }
            });
            int axisCapBottom = AddCaption(pdfEditor, ref currentPage, labelFont, "axis-aligned", xLeft,
                (int)axisEllipseInfo.LastPageRectangle.Bounds.Bottom + 5, 120);

            // Rotated ellipse + outline its tight Bounds (red) and its QuadPoints (blue).
            var rotEllipse = new PdfEllipseElement(xLeft + 180, crtYPos + 25, 120, 70)
            {
                FillColor = PdfColor.LightCyan,
                BorderColor = PdfColor.Teal,
                Border = new PdfLineStyle { LineWidth = 1f },
                RotationDegrees = 35,
                RotationPivot = PdfRotationPivot.Center
            };
            var rotEllipseInfo = pdfEditor.AddEllipse(currentPage, rotEllipse);

            // axis-aligned bounding box of the rotated ellipse (red dashed)
            var eb = rotEllipseInfo.LastPageRectangle.Bounds;
            pdfEditor.AddRectangle(currentPage, new PdfRectangleElement(eb.X, eb.Y, eb.Width, eb.Height)
            {
                FillColor = null,
                BorderColor = PdfColor.Red,
                Border = new PdfLineStyle { LineWidth = 0.5f, DashStyle = PdfLineDashStyle.Dotted }
            });
            int rotCapBottom = AddCaption(pdfEditor, ref currentPage, labelFont, "rotated 35 deg (red axis-aligned bounding box)",
                xLeft + 150, (int)eb.Bottom + 5, 250);

            crtYPos = Math.Max(axisCapBottom, rotCapBottom) + ySeparator;

            // ===== Section 6: PdfArcElement (3 closure types) =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 180, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "6. PdfArcElement (Open, Chord, Pie closures)", xLeft, crtYPos, ySeparator);

            PdfArcClosureType[] closures = { PdfArcClosureType.Open, PdfArcClosureType.Chord, PdfArcClosureType.Pie };
            string[] closureLabels = { "Open (stroke only)", "Chord", "Pie" };
            int arcMaxBottom = crtYPos;
            for (int i = 0; i < closures.Length; i++)
            {
                int arcX = xLeft + i * 170;
                var arcInfo = pdfEditor.AddArc(currentPage, new PdfArcElement(arcX, crtYPos, 130, 90,
                    startAngleDegrees: 20,
                    sweepAngleDegrees: 200)
                {
                    Closure = closures[i],
                    FillColor = PdfColor.LightSkyBlue,
                    FillOpacity = 0.4f,
                    LineColor = PdfColor.MediumBlue,
                    LineStyle = new PdfLineStyle { LineWidth = 1.5f }
                });
                int capBottom = AddCaption(pdfEditor, ref currentPage, labelFont, closureLabels[i], arcX,
                    (int)arcInfo.LastPageRectangle.Bounds.Bottom + 5, 140);
                arcMaxBottom = Math.Max(arcMaxBottom, capBottom);
            }
            crtYPos = arcMaxBottom + ySeparator;

            byte[] outPdfBuffer = pdfEditor.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfShapesEditDemo.pdf";
            return fileResult;
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

        private Add_Shapes_to_Existing_PDF_ViewModel SetViewModel()
        {
            var model = new Add_Shapes_to_Existing_PDF_ViewModel();

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
                0, currentPageUrl.Length - "Add_Shapes_to_Existing_PDF".Length);

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
