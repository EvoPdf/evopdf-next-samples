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
    public class Create_Electronic_Invoice_from_WordController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Create_Electronic_Invoice_from_WordController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConvertWordToPdf(Create_Electronic_Invoice_from_Word_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // The invoice XML: the uploaded file when there is one, otherwise the selected sample
            byte[] invoiceXml = await ReadInvoiceXml(model);

            // Read the invoice. The profile and the document type (invoice, credit note, order...) are
            // taken from the XML itself
            ElectronicInvoice invoice = ElectronicInvoice.FromXml(invoiceXml);

            // How the embedded XML relates to the visible page. MINIMUM and BASIC WL do not carry
            // the whole invoice, so for them the relationship is always Data
            invoice.Relationship = model.Relationship;

            // Create a Word to PDF converter object with default settings
            WordToPdfConverter wordToPdfConverter = new WordToPdfConverter();

            // An electronic invoice needs a PDF/A-3 carrier. PDF/UA-1 + PDF/A-3a produces an
            // invoice that is also accessible
            wordToPdfConverter.PdfDocumentOptions.PdfStandard = model.PdfStandard;

            // Embed the invoice XML and write the metadata invoice software looks for. The Excel,
            // RTF and Markdown to PDF converters embed an electronic invoice the same way
            wordToPdfConverter.PdfDocumentOptions.AddElectronicInvoice(invoice);

            // Open the Attachments panel on document load, so the embedded XML is visible
            wordToPdfConverter.PdfViewerPreferences.PageMode = ViewerPageMode.UseAttachments;

            byte[] inputWordBytes = await ReadWordFile(model.WordFile, model.WordFileUrl);

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = wordToPdfConverter.ConvertToPdf(inputWordBytes);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = string.Format("{0}_{1}_{2}.pdf", invoice.DocumentType, invoice.InvoiceNumber, invoice.Profile);

            return fileResult;
        }

        private async Task<byte[]> ReadInvoiceXml(Create_Electronic_Invoice_from_Word_ViewModel model)
        {
            if (model.InvoiceXmlFile != null && model.InvoiceXmlFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await model.InvoiceXmlFile.CopyToAsync(ms);
                return ms.ToArray();
            }

            // Only file names from the samples folder are accepted
            string fileName = Path.GetFileName(model.SampleInvoice ?? string.Empty);
            string filePath = Path.Combine(GetInvoiceFilesPath(), fileName);
            if (fileName.Length == 0 || !System.IO.File.Exists(filePath))
                throw new Exception("No invoice XML provided: upload a file or select a sample");

            return await System.IO.File.ReadAllBytesAsync(filePath);
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

        private Create_Electronic_Invoice_from_Word_ViewModel SetViewModel()
        {
            var model = new Create_Electronic_Invoice_from_Word_ViewModel();

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Create_Electronic_Invoice_from_Word".Length);

            model.WordFileUrl = rootUrl + "/DemoAppFiles/Input/Word_Files/Word_Invoice.docx";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetInvoiceFilesPath() => Path.Combine(GetDemoFilesPath(), "Invoice_Files");
    }
}
