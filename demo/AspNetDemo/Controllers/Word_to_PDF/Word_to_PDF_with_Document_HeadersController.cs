using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.Word_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.Word_to_PDF
{
    public class Word_to_PDF_with_Document_HeadersController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Word_to_PDF_with_Document_HeadersController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConvertWordToPdf(Word_to_PDF_with_Document_Headers_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // Create a Word to PDF converter object with default settings
            WordToPdfConverter wordToPdfConverter = new WordToPdfConverter();

            // Render the header and the footer of the Word document on the PDF pages, with the page
            // number and page count fields they contain and with a different first page when the
            // document has one. A HTML header or footer set in the converter takes the place of the
            // document one
            wordToPdfConverter.PdfDocumentOptions.UseHeaderFooterFromWord = model.UseHeaderFooterFromWord;

            if (model.AddHtmlHeader)
            {
                // A HTML header set in the converter takes the place of the document header
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Html = model.HeaderHtmlTextBox;
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlBaseUrl = model.HeaderHtmlBaseUrlTextBox;
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoSizeContentHeight = true;
            }

            if (model.AddHtmlFooter)
            {
                // A HTML footer set in the converter takes the place of the document footer
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Html = model.FooterHtmlTextBox;
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlBaseUrl = model.FooterHtmlBaseUrlTextBox;
                wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoSizeContentHeight = true;
            }

            byte[] inputWordBytes = await ReadWordFile(model.WordFile, model.WordFileUrl);

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = wordToPdfConverter.ConvertToPdf(inputWordBytes);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Word_to_Pdf_Document_Headers.pdf";

            return fileResult;
        }

        private async Task<byte[]> ReadWordFile(IFormFile wordFile, string wordFileUrl)
        {
            // If an uploaded file exists, use it with priority
            if (wordFile != null && wordFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await wordFile.CopyToAsync(ms);
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded Word file", ex);
                }
            }

            // Otherwise, fall back to the URL
            string wordUrl = wordFileUrl?.Trim();
            if (string.IsNullOrWhiteSpace(wordUrl))
                throw new Exception("No Word file provided: upload a file or specify a URL");

            try
            {
                if (wordUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    string localPath = new Uri(wordUrl).LocalPath;
                    return await System.IO.File.ReadAllBytesAsync(localPath);
                }

                using var httpClient = new System.Net.Http.HttpClient();
                return await httpClient.GetByteArrayAsync(wordUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not download the Word file from URL", ex);
            }
        }

        private Word_to_PDF_with_Document_Headers_ViewModel SetViewModel()
        {
            var model = new Word_to_PDF_with_Document_Headers_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Word_to_PDF_with_Document_Headers".Length);

            model.WordFileUrl = rootUrl + "/DemoAppFiles/Input/Word_Files/Word_Document_Header_Footer.docx";
            model.HeaderHtmlTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Header_HTML.html"));
            model.HeaderHtmlBaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.FooterHtmlTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Footer_HTML.html"));
            model.FooterHtmlBaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";

            return model;
        }
    }
}
