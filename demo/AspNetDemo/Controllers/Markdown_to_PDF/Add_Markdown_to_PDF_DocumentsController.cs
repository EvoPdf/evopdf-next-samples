using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.Markdown_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.Markdown_to_PDF
{
    public class Add_Markdown_to_PDF_DocumentsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Add_Markdown_to_PDF_DocumentsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePdf(Add_Markdown_to_PDF_Documents_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // The Markdown content: a string entered in the page, with a base URL for its relative
            // links and images, or a file given by URL or uploaded
            PdfMarkdownElement markdownElement;
            if (model.MarkdownSource == "String")
            {
                if (string.IsNullOrWhiteSpace(model.MarkdownString))
                    throw new Exception("No Markdown content provided");

                markdownElement = new PdfMarkdownElement(model.MarkdownString, model.BaseUrl);
            }
            else
            {
                string markdownContent = await ReadMarkdownFile(model.MarkdownFile, model.MarkdownFileUrl);
                markdownElement = new PdfMarkdownElement(markdownContent, model.BaseUrl);
            }

            PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
            {
                PageSize = PdfPageSize.A4,
                PageOrientation = PdfPageOrientation.Portrait,
                Margins = new PdfMargins(36, 36, 36, 36),
                PdfStandard = model.PdfStandard,
                Language = "en-US"
            };

            using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);
            pdfDocument.PdfDocumentInfo.Title = "Markdown Content in a PDF Document";

            string fontFilePath = Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 16f, PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont textFont = PdfFontManager.CreateFont(baseFont, 10f, PdfFontStyle.Normal, PdfColor.Black);

            // Elements created with the PDF creator before the document content
            PdfTextElement titleElement = new PdfTextElement("Markdown Content in a PDF Document", titleFont)
            {
                X = 0,
                Y = 0,
                Width = pdfDocument.ContentWidth,
                Alignment = PdfTextAlignment.Center
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            float crtYPos = pdfDocument.AddText(titleElement).LastPageRectangle.Bounds.Bottom + 10;

            PdfTextElement introElement = new PdfTextElement(
                "The text above and this paragraph are text elements of the PDF document. The Markdown content " +
                "starts below them and continues on new pages when it does not fit in this one. " +
                "The elements added after the Markdown content go to its last page, below it.", textFont)
            {
                X = 0,
                Y = crtYPos,
                Width = pdfDocument.ContentWidth
            };
            crtYPos = pdfDocument.AddText(introElement).LastPageRectangle.Bounds.Bottom + 15;

            // The Markdown content is rendered in the PDF document from the given position of the
            // current page and inside the page margins. The X position is kept on the next pages
            markdownElement.X = model.ElementX;
            markdownElement.Y = crtYPos;
            // 0 extends the content to the right margin of the page
            markdownElement.Width = model.ElementWidth;
            markdownElement.Zoom = model.ElementZoom;
            // The whole content is scaled down to show entirely a table wider than the content area only when asked to
            markdownElement.FitWideContentToPage = model.FitWideContentToPage;
            // The CSS applied to the rendered Markdown content
            markdownElement.StyleSheet = model.StyleSheet;
            // Raw HTML inside the Markdown is rendered as HTML, or shown as text when disabled
            markdownElement.DisableRawHtml = model.DisableRawHtml;
            // The headings of the Markdown content join the outline of the PDF document
            markdownElement.GenerateDocumentOutline = model.GenerateDocumentOutline;

            PdfContentRenderInfo markdownRenderInfo = pdfDocument.AddMarkdown(markdownElement);

            // The rendered area reported for the last page places the elements after the content
            crtYPos = markdownRenderInfo.LastPageRectangle.Bounds.Bottom + 15;

            PdfRectangleElement separator = new PdfRectangleElement(0, crtYPos, pdfDocument.ContentWidth, 1)
            {
                FillColor = PdfColor.DarkBlue,
                BorderColor = null
            };
            pdfDocument.AddRectangle(separator);

            PdfTextElement closingElement = new PdfTextElement(
                string.Format("The Markdown content was rendered on {0} page(s), from page {1} to page {2}. This text was added after it.",
                    markdownRenderInfo.PagesCount, markdownRenderInfo.FirstPageNumber, markdownRenderInfo.LastPageNumber), textFont)
            {
                X = 0,
                Y = crtYPos + 8,
                Width = pdfDocument.ContentWidth
            };
            pdfDocument.AddText(closingElement);

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = pdfDocument.Save();

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Markdown_in_Pdf_Document.pdf";

            return fileResult;
        }

        private async Task<string> ReadMarkdownFile(IFormFile markdownFile, string markdownFileUrl)
        {
            // If an uploaded file exists, use it with priority
            if (markdownFile != null && markdownFile.Length > 0)
            {
                try
                {
                    using var reader = new StreamReader(markdownFile.OpenReadStream());
                    return await reader.ReadToEndAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded Markdown file", ex);
                }
            }

            // Otherwise, fall back to the URL
            string markdownUrl = markdownFileUrl?.Trim();
            if (string.IsNullOrWhiteSpace(markdownUrl))
                throw new Exception("No Markdown file provided: upload a file or specify a URL");

            try
            {
                if (markdownUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    string localPath = new Uri(markdownUrl).LocalPath;
                    return await System.IO.File.ReadAllTextAsync(localPath);
                }

                using var httpClient = new System.Net.Http.HttpClient();
                return await httpClient.GetStringAsync(markdownUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not download the Markdown file from URL", ex);
            }
        }

        private Add_Markdown_to_PDF_Documents_ViewModel SetViewModel()
        {
            var model = new Add_Markdown_to_PDF_Documents_ViewModel();

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Add_Markdown_to_PDF_Documents".Length);

            var contentRootPath = Path.Combine(m_hostingEnvironment.ContentRootPath, "wwwroot");

            model.MarkdownFileUrl = rootUrl + "/DemoAppFiles/Input/Markdown_Files/Markdown_Document.md";
            model.BaseUrl = rootUrl + "/DemoAppFiles/Input/Markdown_Files/";
            model.MarkdownString = "# Markdown content given as a string\n\nThis paragraph comes from a Markdown string entered in the demo page. " +
                "The **bold**, *italic* and `code` words keep their formatting.\n\n- The first item of a list\n- The second item, with a [link](https://www.evopdf.com)\n\n" +
                "<p style=\"color: darkred\">This paragraph is raw HTML inside the Markdown; it is rendered as text when raw HTML is disabled.</p>\n";
            model.StyleSheet = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/Markdown_Files/Markdown_Style.css"));

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
