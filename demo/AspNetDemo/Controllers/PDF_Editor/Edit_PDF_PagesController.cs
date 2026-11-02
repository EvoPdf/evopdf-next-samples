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
    public class Edit_PDF_PagesController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Edit_PDF_PagesController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPdf(Edit_PDF_Pages_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            byte[] inputPdfBytes = await ReadInputPdf(model);

            string password = string.IsNullOrEmpty(model.OwnerPassword) ? model.UserPassword : model.OwnerPassword;
            byte[] outPdfBuffer;

            using (PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, password))
            {
                string fontFilePath = Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf");
                PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
                PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 16f, PdfFontStyle.Bold, PdfColor.DarkBlue);
                PdfFont textFont = PdfFontManager.CreateFont(baseFont, 10f, PdfFontStyle.Normal, PdfColor.Black);
                PdfFont noteFont = PdfFontManager.CreateFont(baseFont, 12f, PdfFontStyle.Bold, PdfColor.DarkRed);

                // The page operations come first; the text drawn afterwards goes to the pages at their
                // final position and rotation
                if (!string.IsNullOrWhiteSpace(model.RemovePageRanges))
                {
                    // The pages are removed first, so the ranges refer to the pages of the loaded document. The
                    // ranges are 1-based, for example "2,4-5" or "3-" for the pages from the third one to the end
                    pdfEditor.RemovePages(model.RemovePageRanges.Trim());
                }

                int coverPageNumber = 0;
                if (model.InsertCoverPage)
                {
                    // Insert a blank page of the chosen size before the first page. The existing pages
                    // are shifted and keep their content, links and bookmarks
                    coverPageNumber = pdfEditor.InsertPage(1, SelectedPdfPageSize(model.CoverPageSize),
                        SelectedPdfPageOrientation(model.CoverPageOrientation));
                }

                int lastPageNumber = 0;
                if (model.AddLastPage)
                {
                    // Add a blank page at the end of the document, with the size of the last page
                    lastPageNumber = pdfEditor.AddPage();
                }

                if (model.MovePage)
                {
                    // Move a page to another position; the pages in between are shifted
                    int pageCount = pdfEditor.PageCount;
                    if (model.MovePageNumber < 1 || model.MovePageNumber > pageCount || model.MoveToPageNumber < 1 || model.MoveToPageNumber > pageCount)
                        throw new Exception("The page numbers to move must be between 1 and " + pageCount);

                    pdfEditor.MovePage(model.MovePageNumber, model.MoveToPageNumber);

                    // The pages added above follow the move
                    coverPageNumber = PageNumberAfterMove(coverPageNumber, model.MovePageNumber, model.MoveToPageNumber);
                    lastPageNumber = PageNumberAfterMove(lastPageNumber, model.MovePageNumber, model.MoveToPageNumber);
                }

                if (model.RotatePage)
                {
                    int pageCount = pdfEditor.PageCount;
                    if (model.RotatePageNumber < 1 || model.RotatePageNumber > pageCount)
                        throw new Exception("The page number to rotate must be between 1 and " + pageCount);

                    // Rotate the page clockwise by a multiple of 90 degrees. The rotation is added to the
                    // current rotation of the page; SetPageRotation would set an absolute rotation instead.
                    // A page is rotated before content is added to it
                    pdfEditor.RotatePage(model.RotatePageNumber, model.RotateDegrees);
                }

                if (lastPageNumber > 0)
                {
                    PdfTextElement lastPageText = new PdfTextElement("This page was added at the end of the document with PdfEditor.AddPage()", textFont)
                    {
                        X = 36,
                        Y = 36,
                        Width = pdfEditor.GetPdfPageInfo(lastPageNumber).PageSize.Width - 72
                    };
                    pdfEditor.AddText(lastPageNumber, lastPageText);
                }

                if (model.RotatePage)
                {
                    // A note at the bottom of the rotated page, placed in the page as it is displayed:
                    // a rotation of 90 or 270 degrees swaps its width and height
                    PdfPageInfo rotatedPageInfo = pdfEditor.GetPdfPageInfo(model.RotatePageNumber);
                    bool turned = rotatedPageInfo.Rotation % 180 != 0;
                    float displayedWidth = turned ? rotatedPageInfo.PageSize.Height : rotatedPageInfo.PageSize.Width;
                    float displayedHeight = turned ? rotatedPageInfo.PageSize.Width : rotatedPageInfo.PageSize.Height;
                    PdfTextElement rotatedPageText = new PdfTextElement(
                        "This page was rotated by " + model.RotateDegrees + " degrees with PdfEditor.RotatePage()", noteFont)
                    {
                        X = 36,
                        Y = displayedHeight - 60,
                        Width = displayedWidth - 72
                    };
                    pdfEditor.AddText(model.RotatePageNumber, rotatedPageText);
                }

                if (coverPageNumber > 0)
                {
                    // The cover page lists the pages of the edited document with the size and
                    // rotation reported by GetPdfPageInfo
                    PdfPageInfo coverPageInfo = pdfEditor.GetPdfPageInfo(coverPageNumber);
                    float textWidth = coverPageInfo.PageSize.Width - 72;

                    PdfTextElement coverTitle = new PdfTextElement("Pages of the edited document", titleFont)
                    {
                        X = 36,
                        Y = 36,
                        Width = textWidth
                    };
                    float crtYPos = pdfEditor.AddText(coverPageNumber, coverTitle).LastPageRectangle.Bounds.Bottom + 12;

                    PdfTextElement coverIntro = new PdfTextElement(
                        "This cover page was inserted before the first page with PdfEditor.InsertPage(). " +
                        "The list below is taken from PdfEditor.GetPdfPageInfo() for each page of the document.", textFont)
                    {
                        X = 36,
                        Y = crtYPos,
                        Width = textWidth
                    };
                    crtYPos = pdfEditor.AddText(coverPageNumber, coverIntro).LastPageRectangle.Bounds.Bottom + 12;

                    int pageCount = pdfEditor.PageCount;
                    for (int pageNumber = 1; pageNumber <= pageCount; pageNumber++)
                    {
                        PdfPageInfo pageInfo = pdfEditor.GetPdfPageInfo(pageNumber);
                        string pageLine = string.Format("Page {0}: {1} x {2} pt, rotation {3} degrees",
                            pageInfo.PageNumber, pageInfo.PageSize.Width, pageInfo.PageSize.Height, pageInfo.Rotation);

                        PdfTextElement pageText = new PdfTextElement(pageLine, textFont)
                        {
                            X = 54,
                            Y = crtYPos,
                            Width = textWidth - 18
                        };
                        crtYPos = pdfEditor.AddText(coverPageNumber, pageText).LastPageRectangle.Bounds.Bottom + 4;
                    }

                    if (!string.IsNullOrWhiteSpace(model.RemovePageRanges))
                    {
                        PdfTextElement removeNote = new PdfTextElement(
                            "The pages " + model.RemovePageRanges.Trim() + " of the loaded document were removed with PdfEditor.RemovePages() before the other operations", textFont)
                        {
                            X = 36,
                            Y = crtYPos + 8,
                            Width = textWidth
                        };
                        pdfEditor.AddText(coverPageNumber, removeNote);
                    }
                }

                outPdfBuffer = pdfEditor.Save();
            }

            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfPagesEditDemo.pdf";
            return fileResult;
        }

        private async Task<byte[]> ReadInputPdf(Edit_PDF_Pages_ViewModel model)
        {
            // If an uploaded file exists, use it with priority
            if (model.PdfFile != null && model.PdfFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await model.PdfFile.CopyToAsync(ms);
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded PDF file", ex);
                }
            }

            // Otherwise, fall back to the URL
            string pdfUrl = model.PdfFileUrl?.Trim();
            if (string.IsNullOrWhiteSpace(pdfUrl))
                throw new Exception("No PDF file provided: upload a file or specify a URL");

            try
            {
                if (pdfUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    string localPath = new Uri(pdfUrl).LocalPath;
                    return await System.IO.File.ReadAllBytesAsync(localPath);
                }

                using var httpClient = new System.Net.Http.HttpClient();
                return await httpClient.GetByteArrayAsync(pdfUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not download the PDF file from URL", ex);
            }
        }

        // The number of a page after another page was moved
        private static int PageNumberAfterMove(int pageNumber, int movedPage, int newPage)
        {
            if (pageNumber == movedPage)
                return newPage;
            if (movedPage < pageNumber && pageNumber <= newPage)
                return pageNumber - 1;
            if (newPage <= pageNumber && pageNumber < movedPage)
                return pageNumber + 1;
            return pageNumber;
        }

        private PdfPageSize SelectedPdfPageSize(string selectedValue)
        {
            switch (selectedValue)
            {
                case "A3":
                    return PdfPageSize.A3;
                case "A4":
                    return PdfPageSize.A4;
                case "A5":
                    return PdfPageSize.A5;
                case "Letter":
                    return PdfPageSize.Letter;
                case "Legal":
                    return PdfPageSize.Legal;
                case "Ledger":
                    return PdfPageSize.Ledger;
                default:
                    return PdfPageSize.A4;
            }
        }

        private PdfPageOrientation SelectedPdfPageOrientation(string selectedValue)
        {
            return selectedValue == "Portrait" ? PdfPageOrientation.Portrait : PdfPageOrientation.Landscape;
        }

        private Edit_PDF_Pages_ViewModel SetViewModel()
        {
            var model = new Edit_PDF_Pages_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Edit_PDF_Pages".Length);

            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/PDF_Document.pdf";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
