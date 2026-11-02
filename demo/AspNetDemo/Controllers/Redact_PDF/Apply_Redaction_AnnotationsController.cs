using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.Redact_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.Redact_PDF
{
    public class Apply_Redaction_AnnotationsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Apply_Redaction_AnnotationsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ApplyRedactions(Apply_Redaction_Annotations_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            byte[] inputPdfBytes = await ReadInputPdf(model);
            string password = !string.IsNullOrEmpty(model.OwnerPassword) ? model.OwnerPassword :
                !string.IsNullOrEmpty(model.UserPassword) ? model.UserPassword : null;

            using PdfRedactor pdfRedactor = new PdfRedactor(inputPdfBytes, password);

            // The areas marked with redaction annotations in a PDF viewer are redacted with the fill color and
            // the overlay text set in each annotation, then the annotations are removed from the document
            int markedCount = pdfRedactor.AddRedactionAnnotations();
            if (markedCount == 0 && !model.RedactPersonalData)
                throw new Exception("The PDF document does not contain redaction annotations");

            // The personal data found in the document can be redacted in the same operation
            if (model.RedactPersonalData)
                pdfRedactor.RedactPersonalData(PdfPersonalDataTypes.All);

            pdfRedactor.RemoveMetadata = model.RemoveMetadata;
            pdfRedactor.RemoveJavaScript = model.RemoveJavaScript;

            // Apply the redactions. The removed content cannot be recovered from the saved document
            byte[] outPdfBuffer = pdfRedactor.Save();

            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Apply_Redaction_Annotations.pdf";
            return fileResult;
        }

        private async Task<byte[]> ReadInputPdf(Apply_Redaction_Annotations_ViewModel model)
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

        private Apply_Redaction_Annotations_ViewModel SetViewModel()
        {
            var model = new Apply_Redaction_Annotations_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Apply_Redaction_Annotations".Length);

            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/Document_with_Redaction_Marks.pdf";

            return model;
        }
    }
}
