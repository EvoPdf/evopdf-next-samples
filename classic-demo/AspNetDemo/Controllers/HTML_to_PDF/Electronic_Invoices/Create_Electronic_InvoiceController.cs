using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.Electronic_Invoices
{
    public class Create_Electronic_InvoiceController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public Create_Electronic_InvoiceController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        private void SetCurrentViewData()
        {
            ViewData["ContentRootPath"] = m_hostingEnvironment.ContentRootPath + "/wwwroot";

            HttpRequest request = this.ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            ViewData["CurrentPageUrl"] = uriBuilder.Uri.AbsoluteUri;
        }

        // GET: Create_Electronic_Invoice
        public ActionResult Index()
        {
            SetCurrentViewData();

            return View();
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(IFormCollection collection)
        {
            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            htmlToPdfConverter.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
            htmlToPdfConverter.ConversionDelay = 2;

            // Read the invoice XML, the uploaded file or the selected sample. The profile and the document type
            // (invoice, credit note, order) are taken from the XML itself
            EvoPdf.Next.ElectronicInvoice invoice = EvoPdf.Next.ElectronicInvoice.FromXml(ReadInvoiceXml(collection));

            // How the embedded XML relates to the visible page
            invoice.Relationship = SelectedRelationship(collection["relationshipDropDownList"]);

            // The classic converter runs over an EvoPdf.Next converter, created for each conversion and exposed by
            // the Next property. The NextConverterCreated event sets on it the options that have no classic
            // counterpart: the PDF/A-3 standard an electronic invoice needs, the embedded invoice with the
            // metadata invoice software looks for and the Attachments panel opened on document load
            htmlToPdfConverter.NextConverterCreated += nextConverter =>
            {
                nextConverter.PdfDocumentOptions.PdfStandard = SelectedPdfStandard(collection["pdfStandardDropDownList"]);
                nextConverter.PdfDocumentOptions.AddElectronicInvoice(invoice);
                nextConverter.PdfViewerPreferences.PageMode = EvoPdf.Next.ViewerPageMode.UseAttachments;
            };

            string htmlString = collection["htmlStringTextBox"];
            string baseUrl = collection["baseUrlTextBox"];

            // Convert the invoice HTML string with a base URL to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlString, baseUrl);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = string.Format("{0}_{1}_{2}.pdf", invoice.DocumentType, invoice.InvoiceNumber, invoice.Profile);

            return fileResult;
        }

        private byte[] ReadInvoiceXml(IFormCollection collection)
        {
            IFormFile uploadedFile = collection.Files["invoiceXmlFileInput"];
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    uploadedFile.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }

            // Only file names from the samples folder are accepted
            string fileName = Path.GetFileName(collection["sampleInvoiceDropDownList"].ToString());
            string filePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Invoice_Files/" + fileName;
            if (fileName.Length == 0 || !System.IO.File.Exists(filePath))
                throw new Exception("No invoice XML provided: upload a file or select a sample");

            return System.IO.File.ReadAllBytes(filePath);
        }

        private EvoPdf.Next.PdfAttachmentRelationship SelectedRelationship(string selectedValue)
        {
            EvoPdf.Next.PdfAttachmentRelationship relationship;
            return Enum.TryParse(selectedValue, out relationship) ? relationship : EvoPdf.Next.PdfAttachmentRelationship.Alternative;
        }

        private EvoPdf.Next.PdfStandard SelectedPdfStandard(string selectedValue)
        {
            EvoPdf.Next.PdfStandard pdfStandard;
            return Enum.TryParse(selectedValue, out pdfStandard) ? pdfStandard : EvoPdf.Next.PdfStandard.PdfA3b;
        }
    }
}
