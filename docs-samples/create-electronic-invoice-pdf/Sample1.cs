using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Create_Electronic_Invoice_PDFController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Create_Electronic_Invoice_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ConvertHtmlToPdf(Create_Electronic_Invoice_PDF_ViewModel model)
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

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // An electronic invoice needs a PDF/A-3 carrier. PDF/UA-1 + PDF/A-3a produces an
            // invoice that is also accessible
            htmlToPdfConverter.PdfDocumentOptions.PdfStandard = model.PdfStandard;

            // Embed the invoice XML and write the metadata invoice software looks for
            htmlToPdfConverter.PdfDocumentOptions.AddElectronicInvoice(invoice);

            // Open the Attachments panel on document load, so the embedded XML is visible
            htmlToPdfConverter.PdfViewerPreferences.PageMode = ViewerPageMode.UseAttachments;

            byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(model.HtmlString, model.BaseUrl);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = string.Format("{0}_{1}_{2}.pdf", invoice.DocumentType, invoice.InvoiceNumber, invoice.Profile);

            return fileResult;
        }

        private async Task<byte[]> ReadInvoiceXml(Create_Electronic_Invoice_PDF_ViewModel model)
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

        private Create_Electronic_Invoice_PDF_ViewModel SetViewModel()
        {
            var model = new Create_Electronic_Invoice_PDF_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(
                0, currentPageUrl.Length - "Create_Electronic_Invoice_PDF".Length);

            model.HtmlString = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Electronic_Invoice.html"));
            model.BaseUrl = rootUrl + "DemoAppFiles/Input/HTML_Files/";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetInvoiceFilesPath() => Path.Combine(GetDemoFilesPath(), "Invoice_Files");
    }
}
