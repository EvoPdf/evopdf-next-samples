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
    public class RTF_to_PDF_with_Document_HeadersController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public RTF_to_PDF_with_Document_HeadersController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConvertRtfToPdf(RTF_to_PDF_with_Document_Headers_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // Create a RTF to PDF converter object with default settings
            RtfToPdfConverter rtfToPdfConverter = new RtfToPdfConverter();

            // Render the header and the footer of the RTF document on the PDF pages, with the page
            // number and page count fields they contain and with a different first page when the
            // document has one. A HTML header or footer set in the converter takes the place of the
            // document one
            rtfToPdfConverter.PdfDocumentOptions.UseHeaderFooterFromRtf = model.UseHeaderFooterFromRtf;

            if (model.AddHtmlHeader)
            {
                // A HTML header set in the converter takes the place of the document header
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Html = model.HeaderHtmlTextBox;
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlBaseUrl = model.HeaderHtmlBaseUrlTextBox;
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoSizeContentHeight = true;
            }

            if (model.AddHtmlFooter)
            {
                // A HTML footer set in the converter takes the place of the document footer
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Html = model.FooterHtmlTextBox;
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlBaseUrl = model.FooterHtmlBaseUrlTextBox;
                rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoSizeContentHeight = true;
            }

            byte[] inputRtfBytes = await ReadRtfFile(model.RtfFile, model.RtfFileUrl);

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = rtfToPdfConverter.ConvertToPdf(inputRtfBytes);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "RTF_to_Pdf_Document_Headers.pdf";

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

        private RTF_to_PDF_with_Document_Headers_ViewModel SetViewModel()
        {
            var model = new RTF_to_PDF_with_Document_Headers_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "RTF_to_PDF_with_Document_Headers".Length);

            model.RtfFileUrl = rootUrl + "/DemoAppFiles/Input/RTF_Files/RTF_Document_Header_Footer.rtf";
            model.HeaderHtmlTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Header_HTML.html"));
            model.HeaderHtmlBaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.FooterHtmlTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Footer_HTML.html"));
            model.FooterHtmlBaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";

            return model;
        }
    }
}
