using System;
using System.IO;
using System.Text;
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
    public class Add_HTML_to_Existing_PDFController : Controller
    {
        // The area of the letterhead pages left free by their header and footer
        private const int leftMargin = 36;
        private const int topMargin = 90;
        private const int bottomMargin = 70;
        // The content area of the pages inside the demo margins, from the size of the first page of the loaded document
        private int contentWidth;

        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Add_HTML_to_Existing_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPdf(Add_HTML_to_Existing_PDF_ViewModel model)
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

            string password = string.IsNullOrEmpty(model.OwnerPassword) ? model.UserPassword : model.OwnerPassword;
            using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, password);

            PdfPageInfo firstPageInfo = pdfEditor.GetPdfPageInfo(1);
            // A rotation of 90 or 270 degrees swaps the width and the height of the page as it is displayed
            bool firstPageTurned = firstPageInfo.Rotation % 180 != 0;
            contentWidth = (int)(firstPageTurned ? firstPageInfo.PageSize.Height : firstPageInfo.PageSize.Width) - 2 * leftMargin;

            string fontFilePath = Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 16f, PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont textFont = PdfFontManager.CreateFont(baseFont, 10f, PdfFontStyle.Normal, PdfColor.Black);
            PdfFont smallFont = PdfFontManager.CreateFont(baseFont, 8f, PdfFontStyle.Normal, PdfColor.DarkGray);

            // The title and the HTML content go to the first page of the document, below its header
            int currentPage = 1;
            int crtYPos = topMargin;

            PdfTextElement titleElement = new PdfTextElement("HTML Content in an Existing PDF Document", titleFont)
            {
                X = leftMargin,
                Y = crtYPos,
                Width = contentWidth,
                Alignment = PdfTextAlignment.Center
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            var titleInfo = pdfEditor.AddText(currentPage, titleElement);
            currentPage = titleInfo.LastPageRectangle.PageNumber;
            crtYPos = (int)titleInfo.LastPageRectangle.Bounds.Bottom + 15;

            // A page added after the pages of the document: the content continues over it and over the pages before
            // it, or the pages where the content continues are inserted before them and they keep their content, as
            // the option asks
            int pageAfterContent = pdfEditor.AddPage();
            pdfEditor.AddText(pageAfterContent, new PdfTextElement("The page that followed the HTML content", smallFont)
            {
                X = leftMargin,
                Y = topMargin
            });

            // The HTML content rendered from the given position of the page, relative to the top left corner of the
            // page. The pages of an edited document have no margins, so the continuation margins give the area of the
            // pages where the content continues
            PdfHtmlElement htmlElement = model.HtmlPageSource == "Url"
                ? new PdfHtmlElement(model.Url)
                : new PdfHtmlElement(model.HtmlStringTextBox ?? string.Empty, model.BaseUrlTextBox);
            htmlElement.X = leftMargin + model.ElementX;
            // The content starts at the given distance below the title
            htmlElement.Y = crtYPos + model.ElementY;
            // 0 extends the content to the right edge of the page
            htmlElement.Width = model.ElementWidth;
            // The pages where the content continues are inserted after the current one, so the pages that follow in
            // the document keep their content; when false, the content continues on the following pages of the document
            htmlElement.InsertContinuationPages = model.InsertContinuationPages;
            htmlElement.ContinuationTopMargin = topMargin;
            htmlElement.ContinuationBottomMargin = bottomMargin;

            // The converter of the element takes the page size and the margins from the page; its other settings are
            // set as for a HTML to PDF conversion
            htmlElement.Converter.MediaType = model.MediaType;
            // The headings of the HTML content join the bookmarks of the document
            htmlElement.Converter.PdfDocumentOptions.GenerateDocumentOutline = model.GenerateDocumentOutline;

            if (model.RetrieveElementPositions)
            {
                // The positions of the elements matching the selector are reported in the render information
                htmlElement.HtmlElementsInfoSelector = model.HtmlElementsSelector;
            }

            PdfHtmlRenderInfo htmlRenderInfo = pdfEditor.AddHtml(currentPage, htmlElement);

            // The elements after the HTML content are placed below it, at the end of the content on its last page
            currentPage = htmlRenderInfo.LastPageNumber;
            crtYPos = (int)htmlRenderInfo.LastPageRectangle.Bounds.Bottom + 15;

            PdfRectangleElement separator = new PdfRectangleElement(leftMargin, crtYPos, contentWidth, 1)
            {
                FillColor = PdfColor.DarkBlue,
                BorderColor = null
            };
            pdfEditor.AddRectangle(currentPage, separator);

            PdfTextElement closingElement = new PdfTextElement(
                string.Format("The HTML content was rendered on {0} page(s), from page {1} to page {2}. This text was added after it.",
                    htmlRenderInfo.PagesCount, htmlRenderInfo.FirstPageNumber, htmlRenderInfo.LastPageNumber), textFont)
            {
                X = leftMargin,
                Y = crtYPos + 8,
                Width = contentWidth
            };
            var closingInfo = pdfEditor.AddText(currentPage, closingElement);
            currentPage = closingInfo.LastPageRectangle.PageNumber;
            crtYPos = (int)closingInfo.LastPageRectangle.Bounds.Bottom + 10;

            if (model.RetrieveElementPositions)
            {
                // The page and the rectangle of each matching element; an element split by a page break has a rectangle for each page
                StringBuilder positions = new StringBuilder();
                foreach (HtmlElementInfo elementInfo in htmlRenderInfo.HtmlElementsInfo.Elements)
                {
                    positions.Append(elementInfo.TagName);
                    if (!string.IsNullOrEmpty(elementInfo.Id))
                        positions.Append(" #").Append(elementInfo.Id);
                    positions.Append(" \"").Append(Shorten(elementInfo.Text)).Append("\":");
                    foreach (PdfRenderedRectangle rectangle in elementInfo.RenderedRectangles)
                    {
                        positions.Append(string.Format(" page {0} ({1:0}, {2:0}, {3:0}, {4:0})", rectangle.PageNumber,
                            rectangle.Bounds.X, rectangle.Bounds.Y, rectangle.Bounds.Width, rectangle.Bounds.Height));
                    }
                    positions.Append('\n');
                }

                PdfTextElement positionsElement = new PdfTextElement(
                    "Positions of the elements matching \"" + model.HtmlElementsSelector + "\":\n" + positions, smallFont)
                {
                    X = leftMargin,
                    Y = crtYPos,
                    Width = contentWidth,
                    ContinueOnNextPage = true,
                    ContinuationTopMargin = topMargin,
                    ContinuationBottomMargin = bottomMargin
                };
                pdfEditor.AddText(currentPage, positionsElement);
            }

            byte[] outPdfBuffer = pdfEditor.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "HTML_in_Existing_Pdf_Document.pdf";
            return fileResult;
        }

        private static string Shorten(string text)
        {
            text = (text ?? string.Empty).Replace('\n', ' ').Replace('\r', ' ').Trim();
            return text.Length > 40 ? text.Substring(0, 40) + "..." : text;
        }

        private Add_HTML_to_Existing_PDF_ViewModel SetViewModel()
        {
            var model = new Add_HTML_to_Existing_PDF_ViewModel();

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
                0, currentPageUrl.Length - "Add_HTML_to_Existing_PDF".Length);
            string contentRootPath = m_hostingEnvironment.ContentRootPath + "/wwwroot/";

            // A one-page document with a letterhead; the content goes between its header and footer
            model.PdfFileUrl = rootUrl + "DemoAppFiles/Input/PDF_Files/Letterhead.pdf";
            model.HtmlStringTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Auto_Bookmarks.html"));
            model.BaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
