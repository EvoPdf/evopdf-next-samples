// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/convert-pdf-to-text.htm
// Documentation page: Convert PDF to Text

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_to_Text;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_to_Text
{
    public class PDF_to_TextController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public PDF_to_TextController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConvertPdfToText(PDF_to_Text_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create the PDF to Text converter instance with default options
            PdfToTextConverter pdfToTextConverter = new PdfToTextConverter();

            // Optionally set the user password to open a password-protected PDF
            if (!string.IsNullOrEmpty(model.UserPassword))
                pdfToTextConverter.UserPassword = model.UserPassword;

            // Optionally set the owner password to open a password-protected PDF
            if (!string.IsNullOrEmpty(model.OwnerPassword))
                pdfToTextConverter.OwnerPassword = model.OwnerPassword;

            // Configure the output text layout
            pdfToTextConverter.TextLayout = model.TextLayout == "Original" ? PdfToTextLayout.Original : PdfToTextLayout.Reading;

            // Mark PDF page breaks with the PdfToTextConverter.PAGE_BREAK_MARK special character
            pdfToTextConverter.MarkPageBreaks = model.MarkPageBreaks;

            // PDF page number to start text extraction from
            int startPageNumber = model.StartPageNumber;

            // PDF page number to end text extraction at
            // If 0, extraction continues to the end of the document
            int endPageNumber = 0;
            if (model.EndPageNumber.HasValue)
                endPageNumber = model.EndPageNumber.Value;

            byte[] inputPdfBytes = null;
            string outputFileName = null;

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

                outputFileName = Path.GetFileNameWithoutExtension(model.PdfFile.FileName) + ".txt";
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

                outputFileName = Path.GetFileNameWithoutExtension(model.PdfFileUrl) + ".txt";
            }

            // Extract text from the specified PDF page range
            string extractedText = pdfToTextConverter.ConvertToText(inputPdfBytes, startPageNumber, endPageNumber);

            // Encode the extracted text as UTF-8 bytes
            byte[] outputTextBytes = Encoding.UTF8.GetBytes(extractedText);

            // Return the text as a downloadable file
            return File(outputTextBytes, "text/plain; charset=utf-8", outputFileName);
        }

        private PDF_to_Text_ViewModel SetViewModel()
        {
            var model = new PDF_to_Text_ViewModel();

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "PDF_to_Text".Length);

            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PdfProcessor_Files/PDF_Document.pdf";

            return model;
        }
    }
}
