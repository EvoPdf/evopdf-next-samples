using System;
using System.IO;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_Creator;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_Creator
{
    public class Create_PDF_Documents_with_HTMLController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Create_PDF_Documents_with_HTMLController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePdf(Create_PDF_Documents_with_HTML_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
            {
                PageSize = PdfPageSize.A4,
                PageOrientation = PdfPageOrientation.Portrait,
                Margins = new PdfMargins(36, 36, 36, 36),
                PdfStandard = model.PdfStandard
            };

            using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);
            pdfDocument.PdfDocumentInfo.Title = "HTML Content in a PDF Document";

            string fontFilePath = Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 16f, PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont textFont = PdfFontManager.CreateFont(baseFont, 10f, PdfFontStyle.Normal, PdfColor.Black);
            PdfFont smallFont = PdfFontManager.CreateFont(baseFont, 8f, PdfFontStyle.Normal, PdfColor.DarkGray);

            // Elements created with the PDF creator before the HTML content
            PdfTextElement titleElement = new PdfTextElement("HTML Content in a PDF Document", titleFont)
            {
                X = 0,
                Y = 0,
                Width = pdfDocument.ContentWidth,
                Alignment = PdfTextAlignment.Center
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            float crtYPos = pdfDocument.AddText(titleElement).LastPageRectangle.Bounds.Bottom + 10;

            PdfTextElement introElement = new PdfTextElement(
                "The text above and this paragraph are text elements of the PDF document. The HTML content " +
                "starts below them and continues on new pages when it does not fit in this one. " +
                "The elements added after the HTML content go to its last page, below it.", textFont)
            {
                X = 0,
                Y = crtYPos,
                Width = pdfDocument.ContentWidth
            };
            crtYPos = pdfDocument.AddText(introElement).LastPageRectangle.Bounds.Bottom + 15;

            // The HTML content rendered in the PDF document, from the given position of the current page and
            // inside the page margins. The X position is kept on the next pages
            PdfHtmlElement htmlElement = model.HtmlPageSource == "Url"
                ? new PdfHtmlElement(model.Url)
                : new PdfHtmlElement(model.HtmlStringTextBox, model.BaseUrlTextBox);
            htmlElement.X = model.ElementX;
            // The content starts at the given distance below the text above it
            htmlElement.Y = crtYPos + model.ElementY;
            // 0 extends the content to the right margin of the page
            htmlElement.Width = model.ElementWidth;

            // The converter of the element takes the page size, the margins and the PDF standard from the document;
            // its other settings are set as for a HTML to PDF conversion
            htmlElement.Converter.MediaType = model.MediaType;
            // The headings of the HTML content join the outline of the PDF document
            htmlElement.Converter.PdfDocumentOptions.GenerateDocumentOutline = model.GenerateDocumentOutline;

            if (model.RetrieveElementPositions)
            {
                // The positions of the elements matching the selector are reported in the render information
                htmlElement.HtmlElementsInfoSelector = model.HtmlElementsSelector;
            }

            PdfHtmlRenderInfo htmlRenderInfo = pdfDocument.AddHtml(htmlElement);

            // The elements after the HTML content are placed below it, at the end of the content on its last page,
            // or on a new page after the content. A new page is also added when the content ends too close to the
            // bottom of its last page for the elements that follow
            const float MinimumSpace = 60;
            crtYPos = htmlRenderInfo.LastPageRectangle.Bounds.Bottom + 15;
            if (model.FollowingElementsOnNewPage || crtYPos > pdfDocument.ContentHeight - MinimumSpace)
            {
                pdfDocument.AddPage();
                crtYPos = 0;
            }

            PdfRectangleElement separator = new PdfRectangleElement(0, crtYPos, pdfDocument.ContentWidth, 1)
            {
                FillColor = PdfColor.DarkBlue,
                BorderColor = null
            };
            pdfDocument.AddRectangle(separator);

            PdfTextElement closingElement = new PdfTextElement(
                string.Format("The HTML content was rendered on {0} page(s), from page {1} to page {2}. This text was added after it.",
                    htmlRenderInfo.PagesCount, htmlRenderInfo.FirstPageNumber, htmlRenderInfo.LastPageNumber), textFont)
            {
                X = 0,
                Y = crtYPos + 8,
                Width = pdfDocument.ContentWidth,
                ContinueOnNextPage = true
            };
            crtYPos = pdfDocument.AddText(closingElement).LastPageRectangle.Bounds.Bottom + 10;

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
                    X = 0,
                    Y = crtYPos,
                    Width = pdfDocument.ContentWidth,
                    ContinueOnNextPage = true
                };
                crtYPos = pdfDocument.AddText(positionsElement).LastPageRectangle.Bounds.Bottom + 15;
            }

            if (model.AddSecondElement)
            {
                // A second HTML element continues after the first one, from the current position of the last page,
                // or from the top of a new page when the space left is too small
                if (crtYPos > pdfDocument.ContentHeight - MinimumSpace)
                {
                    pdfDocument.AddPage();
                    crtYPos = 0;
                }
                PdfHtmlElement secondElement = new PdfHtmlElement(
                    "<div style=\"font-family: Arial; font-size: 12px; border: 1px solid #1F4E79; padding: 8px; background: #EEF3F8;\">" +
                    "<b>Second HTML element</b><br/>This HTML string is rendered by another element added after the text above. " +
                    "It can have its own width and position in the page and its own converter settings.</div>", null)
                {
                    X = model.ElementX + 20,
                    Y = crtYPos,
                    Width = model.ElementWidth > 0 ? model.ElementWidth - 20 : pdfDocument.ContentWidth - model.ElementX - 40
                };
                PdfHtmlRenderInfo secondRenderInfo = pdfDocument.AddHtml(secondElement);
                crtYPos = secondRenderInfo.LastPageRectangle.Bounds.Bottom + 10;

                pdfDocument.AddText(new PdfTextElement(
                    string.Format("The second element was rendered on page {0}.", secondRenderInfo.LastPageNumber), textFont)
                {
                    X = 0,
                    Y = crtYPos,
                    Width = pdfDocument.ContentWidth,
                    ContinueOnNextPage = true
                });
            }

            if (model.AddPageNumbers)
            {
                // A template with the page number centered in the bottom margin of every page, over the pages of the HTML content too
                PdfHtmlTemplate footer = pdfDocument.AddHtmlTemplate(0, 0, pdfDocument.PageSize.Width, 20,
                    "<div style=\"font-family: Arial; font-size: 9px; color: #606060; text-align: center;\">Page {page_number} of {total_pages}</div>", null);
                footer.VerticalAlign = PdfTemplateVerticalAlign.Bottom;
            }

            byte[] outPdfBuffer = pdfDocument.Save();

            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "HTML_in_Pdf_Document.pdf";
            return fileResult;
        }

        private static string Shorten(string text)
        {
            text = (text ?? string.Empty).Replace('\n', ' ').Replace('\r', ' ').Trim();
            return text.Length > 40 ? text.Substring(0, 40) + "..." : text;
        }

        private Create_PDF_Documents_with_HTML_ViewModel SetViewModel()
        {
            var model = new Create_PDF_Documents_with_HTML_ViewModel();

            var contentRootPath = Path.Combine(m_hostingEnvironment.ContentRootPath, "wwwroot");

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Create_PDF_Documents_with_HTML".Length);

            model.HtmlStringTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Auto_Bookmarks.html"));
            model.BaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
