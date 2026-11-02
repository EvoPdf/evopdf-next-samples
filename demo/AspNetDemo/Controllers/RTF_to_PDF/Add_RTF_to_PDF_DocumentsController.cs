using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.RTF_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.RTF_to_PDF
{
    public class Add_RTF_to_PDF_DocumentsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Add_RTF_to_PDF_DocumentsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePdf(Add_RTF_to_PDF_Documents_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // The RTF content: a string entered in the page, or a file given by URL or uploaded
            PdfRtfElement rtfElement;
            if (model.RtfSource == "String")
            {
                if (string.IsNullOrWhiteSpace(model.RtfString))
                    throw new Exception("No RTF content provided");

                // A RTF string is given to the element as it is
                rtfElement = new PdfRtfElement(model.RtfString);
            }
            else
            {
                byte[] inputRtfBytes = await ReadRtfFile(model.RtfFile, model.RtfFileUrl);
                rtfElement = new PdfRtfElement(inputRtfBytes);
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
            pdfDocument.PdfDocumentInfo.Title = "RTF Content in a PDF Document";

            string fontFilePath = Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 16f, PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont textFont = PdfFontManager.CreateFont(baseFont, 10f, PdfFontStyle.Normal, PdfColor.Black);

            // Elements created with the PDF creator before the document content
            PdfTextElement titleElement = new PdfTextElement("RTF Content in a PDF Document", titleFont)
            {
                X = 0,
                Y = 0,
                Width = pdfDocument.ContentWidth,
                Alignment = PdfTextAlignment.Center
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            float crtYPos = pdfDocument.AddText(titleElement).LastPageRectangle.Bounds.Bottom + 10;

            PdfTextElement introElement = new PdfTextElement(
                "The text above and this paragraph are text elements of the PDF document. The RTF content " +
                "starts below them and continues on new pages when it does not fit in this one. " +
                "The elements added after the RTF content go to its last page, below it.", textFont)
            {
                X = 0,
                Y = crtYPos,
                Width = pdfDocument.ContentWidth
            };
            crtYPos = pdfDocument.AddText(introElement).LastPageRectangle.Bounds.Bottom + 15;

            // The RTF content is rendered in the PDF document from the given position of the current
            // page and inside the page margins. The X position is kept on the next pages
            rtfElement.X = model.ElementX;
            rtfElement.Y = crtYPos;
            // 0 extends the content to the right margin of the page
            rtfElement.Width = model.ElementWidth;
            rtfElement.Zoom = model.ElementZoom;
            // The whole content is scaled down to show entirely a table wider than the content area only when asked to
            rtfElement.FitWideContentToPage = model.FitWideContentToPage;
            // The headings of the RTF document join the outline of the PDF document
            rtfElement.GenerateDocumentOutline = model.GenerateDocumentOutline;

            PdfContentRenderInfo rtfRenderInfo = pdfDocument.AddRtf(rtfElement);

            // The rendered area reported for the last page places the elements after the content
            crtYPos = rtfRenderInfo.LastPageRectangle.Bounds.Bottom + 15;

            PdfRectangleElement separator = new PdfRectangleElement(0, crtYPos, pdfDocument.ContentWidth, 1)
            {
                FillColor = PdfColor.DarkBlue,
                BorderColor = null
            };
            pdfDocument.AddRectangle(separator);

            PdfTextElement closingElement = new PdfTextElement(
                string.Format("The RTF content was rendered on {0} page(s), from page {1} to page {2}. This text was added after it.",
                    rtfRenderInfo.PagesCount, rtfRenderInfo.FirstPageNumber, rtfRenderInfo.LastPageNumber), textFont)
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
            fileResult.FileDownloadName = "RTF_in_Pdf_Document.pdf";

            return fileResult;
        }

        private async Task<byte[]> ReadRtfFile(IFormFile rtfFile, string rtfFileUrl)
        {
            // If an uploaded file exists, use it with priority
            if (rtfFile != null && rtfFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await rtfFile.CopyToAsync(ms);
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded RTF file", ex);
                }
            }

            // Otherwise, fall back to the URL
            string rtfUrl = rtfFileUrl?.Trim();
            if (string.IsNullOrWhiteSpace(rtfUrl))
                throw new Exception("No RTF file provided: upload a file or specify a URL");

            try
            {
                if (rtfUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    string localPath = new Uri(rtfUrl).LocalPath;
                    return await System.IO.File.ReadAllBytesAsync(localPath);
                }

                using var httpClient = new System.Net.Http.HttpClient();
                return await httpClient.GetByteArrayAsync(rtfUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not download the RTF file from URL", ex);
            }
        }

        private Add_RTF_to_PDF_Documents_ViewModel SetViewModel()
        {
            var model = new Add_RTF_to_PDF_Documents_ViewModel();

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Add_RTF_to_PDF_Documents".Length);

            model.RtfFileUrl = rootUrl + "/DemoAppFiles/Input/RTF_Files/RTF_Document.rtf";
            model.RtfString = @"{\rtf1\ansi\deff0{\fonttbl{\f0\fswiss Arial;}}{\colortbl;\red31\green56\blue100;}" +
                @"\pard\sa120\f0\fs28\b\cf1 RTF content given as a string\par" +
                @"\pard\sa120\fs22\b0\cf0 This paragraph comes from a RTF string entered in the demo page. The {\b bold}, {\i italic} and {\ul underlined} words keep their formatting.\par}";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
