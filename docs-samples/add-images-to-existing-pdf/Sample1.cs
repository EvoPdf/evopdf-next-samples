// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-images-to-existing-pdf.htm
// Documentation page: Add Images to Existing PDF

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
    public class Add_Images_to_Existing_PDFController : Controller
    {
        private const int leftMargin = 36;
        private const int topMargin = 36;
        private const int contentWidth = 595 - 72;
        private const int contentHeight = 842 - 72;

        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Add_Images_to_Existing_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPdf(Add_Images_to_Existing_PDF_ViewModel model)
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
            pdfEditor.PdfDocumentInfo.Title = "PDF Image Demo";

            string fontsPath = GetDemoFontsPath();
            string fontFilePath = Path.Combine(fontsPath, "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);

            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 18f,
                PdfFontStyle.Bold | PdfFontStyle.Underline, PdfColor.Black);
            PdfFont sectionFont = PdfFontManager.CreateFont(baseFont, 14f,
                PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont smallFont = PdfFontManager.CreateFont(baseFont, 9f,
                PdfFontStyle.Normal, PdfColor.DarkGray);

            const int xLeft = leftMargin;
            const int ySeparator = 10;
            int currentPage = 1;
            int crtYPos = topMargin;

            string imagesPath = GetDemoImagesPath();
            string transparentPngPath = Path.Combine(imagesPath, "transparent.png");
            string jpegPath = Path.Combine(imagesPath, "image.jpg");

            // ===== Title =====
            PdfTextElement titleElement = new PdfTextElement(
                "PDF Image Demo", titleFont)
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

            // ===== Section 1: PNG with custom width, ScaleDownToFit =====
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "1. PNG with custom Width (ScaleDownToFit, EnlargeToFit)", xLeft, crtYPos, ySeparator);

            PdfImageElement pngImage = new PdfImageElement(transparentPngPath)
            {
                X = xLeft,
                Y = crtYPos,
                Width = 150,
                ScaleDownToFit = true,
                EnlargeToFit = false,
                Opacity = 0.85f
            };
            pngImage.Accessibility.AlternateText = "Transparent glass globe";

            PdfImageRenderInfo pngInfo = pdfEditor.AddImage(currentPage, pngImage);
            crtYPos = (int)pngInfo.BoundingBox.Bottom + ySeparator * 2;

            // ===== Section 2: JPEG with alignment =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 220, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "2. JPEG aligned Center horizontally", xLeft, crtYPos, ySeparator);

            PdfImageElement jpgCentered = new PdfImageElement(jpegPath)
            {
                Y = crtYPos,
                Height = 120,
                HorizontalAlign = PdfElementHorizontalAlign.Center
            };
            jpgCentered.Accessibility.AlternateText = "Wooden house on a river";

            PdfImageRenderInfo jpgInfo = pdfEditor.AddImage(currentPage, jpgCentered);
            crtYPos = (int)jpgInfo.BoundingBox.Bottom + ySeparator * 2;

            // ===== Section 3: Rotated image with QuadPoints-based outline =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 280, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "3. Rotated 30 degrees clockwise - Bounding Box (red) vs Rotated Rectangle (blue)", xLeft, crtYPos, ySeparator);

            PdfImageElement rotatedJpg = new PdfImageElement(jpegPath)
            {
                X = xLeft + 80,
                Y = crtYPos + 10,
                Width = 200,
                Height = 150,
                RotationDegrees = -30,
                RotationPivot = PdfRotationPivot.TopLeft,
                Opacity = 0.95f
            };
            rotatedJpg.Accessibility.AlternateText = "Wooden house rotated 30 degrees clockwise";

            PdfImageRenderInfo rotatedInfo = pdfEditor.AddImage(currentPage, rotatedJpg);

            // Outline the axis-aligned Bounds (red).
            var aabb = rotatedInfo.BoundingBox;
            pdfEditor.AddRectangle(currentPage, new PdfRectangleElement(aabb.X, aabb.Y, aabb.Width, aabb.Height)
            {
                FillColor = null,
                BorderColor = PdfColor.Red,
                Border = new PdfLineStyle { LineWidth = 1f, DashStyle = PdfLineDashStyle.Dotted }
            });

            // Outline the rotated QuadPoints (blue) using a polygon through the four corners.
            var q = rotatedInfo.QuadPoints;
            pdfEditor.AddPolygon(currentPage, new PdfPolygonElement(q.TopLeft, q.TopRight, q.BottomRight, q.BottomLeft)
            {
                FillColor = null,
                BorderColor = PdfColor.Blue,
                Border = new PdfLineStyle { LineWidth = 1.5f }
            });

            // Annotate with VisibleBounds info using the actual AABB bottom.
            string visibleSummary = rotatedInfo.VisibleBounds == null
                ? "VisibleBounds: image fully outside the page"
                : $"VisibleBounds=({rotatedInfo.VisibleBounds.X:F0}, {rotatedInfo.VisibleBounds.Y:F0}, " +
                  $"{rotatedInfo.VisibleBounds.Width:F0}x{rotatedInfo.VisibleBounds.Height:F0})  on page " +
                  $"{rotatedInfo.Page} ({rotatedInfo.PageWidth:F0}x{rotatedInfo.PageHeight:F0})";

            PdfTextElement visibleLabel = new PdfTextElement(visibleSummary, smallFont)
            {
                X = xLeft,
                Y = (int)aabb.Bottom + ySeparator,
                Width = contentWidth
            };
            visibleLabel.Accessibility.StructureType = PdfStructureType.Artifact;
            var visibleLabelInfo = pdfEditor.AddText(currentPage, visibleLabel);
            currentPage = visibleLabelInfo.LastPageRectangle.PageNumber;
            crtYPos = (int)visibleLabelInfo.LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 4: Different rotation pivots =====
            currentPage = pdfEditor.AddPage();
            crtYPos = topMargin;
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "4. Same image rotated 45 degrees around four different pivots", xLeft, crtYPos, ySeparator);

            PdfRotationPivot[] pivots = new[]
            {
                PdfRotationPivot.TopLeft, PdfRotationPivot.TopRight,
                PdfRotationPivot.BottomLeft, PdfRotationPivot.Center
            };

            const int cellW = 220;
            const int cellH = 220;
            int imgW = 100, imgH = 75;

            for (int i = 0; i < pivots.Length; i++)
            {
                int col = i % 2;
                int row = i / 2;
                int cellX = xLeft + col * cellW;
                int cellY = crtYPos + row * cellH;

                // Draw the cell as a dashed reference frame.
                pdfEditor.AddRectangle(currentPage, new PdfRectangleElement(cellX, cellY, cellW - 10, cellH - 10)
                {
                    FillColor = null,
                    BorderColor = PdfColor.LightGray,
                    Border = new PdfLineStyle { LineWidth = 0.5f, DashStyle = PdfLineDashStyle.Dashed }
                });

                // Center the image inside the cell, then rotate by 45 around the chosen pivot.
                int imgX = cellX + (cellW - 10 - imgW) / 2;
                int imgY = cellY + (cellH - 10 - imgH) / 2 + 10;

                PdfImageElement img = new PdfImageElement(jpegPath)
                {
                    X = imgX,
                    Y = imgY,
                    Width = imgW,
                    Height = imgH,
                    RotationDegrees = 45,
                    RotationPivot = pivots[i]
                };
                img.Accessibility.AlternateText = $"Sample image rotated 45 degrees around {pivots[i]}";
                PdfImageRenderInfo info = pdfEditor.AddImage(currentPage, img);

                // Trace the rotated quad.
                var rq = info.QuadPoints;
                pdfEditor.AddPolygon(currentPage, new PdfPolygonElement(rq.TopLeft, rq.TopRight, rq.BottomRight, rq.BottomLeft)
                {
                    FillColor = null,
                    BorderColor = PdfColor.Blue,
                    Border = new PdfLineStyle { LineWidth = 1f }
                });

                // Label.
                PdfTextElement pivotLabel = new PdfTextElement(pivots[i].ToString(), smallFont)
                { X = cellX + 4, Y = cellY + 2, Width = cellW - 14 };
                pivotLabel.Accessibility.StructureType = PdfStructureType.Artifact;
                pdfEditor.AddText(currentPage, pivotLabel);
            }

            byte[] outPdfBuffer = pdfEditor.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfImageEditDemo.pdf";
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

        private void EnsureSpaceOnPage(ref int crtYPos, ref int currentPage, int requestedHeight, PdfEditor pdfEditor, int contentHeight, int topMargin)
        {
            if (crtYPos + requestedHeight > contentHeight + topMargin)
            {
                currentPage = pdfEditor.AddPage();
                crtYPos = topMargin;
            }
        }

        private Add_Images_to_Existing_PDF_ViewModel SetViewModel()
        {
            var model = new Add_Images_to_Existing_PDF_ViewModel();

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
                0, currentPageUrl.Length - "Add_Images_to_Existing_PDF".Length);

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
