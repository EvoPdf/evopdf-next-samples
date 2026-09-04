using System;
using System.IO;
using System.Text;
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
    public class Add_Attachments_to_Generated_PDFController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Add_Attachments_to_Generated_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Add_Attachments_to_Generated_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Open the Attachments panel on document load
            htmlToPdfConverter.PdfViewerPreferences.PageMode = ViewerPageMode.UseAttachments;

            // Sets the PDF standard for the generated document
            // Leave as None to generate a plain PDF without an accessibility structure tree or archival metadata
            htmlToPdfConverter.PdfDocumentOptions.PdfStandard = model.PdfStandard;

            // ===== Document-level attachments =====
            // Attachments added via PdfDocumentOptions.AddFileAttachment appear
            // in the viewer's Attachments panel (opened with the paperclip icon
            // in Acrobat's left sidebar).  They have no visible marker on any
            // page.  Two factory methods cover the common cases: FromBytes for
            // in-memory data and FromFile for files on disk

            // FromBytes -- embed an in-memory XML invoice
            byte[] invoiceXmlBytes = Encoding.UTF8.GetBytes(BuildSampleInvoiceXml());
            var xmlAttachment = PdfFileAttachment.FromBytes(invoiceXmlBytes, "invoice.xml");
            xmlAttachment.MimeType = "application/xml";
            xmlAttachment.Description = "Source XML invoice data";
            xmlAttachment.Relationship = PdfAttachmentRelationship.Source;
            htmlToPdfConverter.PdfDocumentOptions.AddFileAttachment(xmlAttachment);

            // FromFile -- embed a file from disk
            string alphabetFilePath = Path.Combine(GetDemoTextsPath(), "Alphabet.txt");
            var textAttachment = PdfFileAttachment.FromFile(alphabetFilePath);
            textAttachment.MimeType = "text/plain";
            textAttachment.Description = "Sample alphabet text";
            htmlToPdfConverter.PdfDocumentOptions.AddFileAttachment(textAttachment);

            byte[] outPdfBuffer = null;

            if (model.HtmlPageSource == "Url")
            {
                string url = model.Url;

                outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
            }
            else
            {
                string htmlWithForm = model.HtmlString;
                string baseUrl = model.BaseUrl;

                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlWithForm, baseUrl);
            }

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfAttachmentsDemo.pdf";

            return fileResult;
        }

        private Add_Attachments_to_Generated_PDF_ViewModel SetViewModel()
        {
            var model = new Add_Attachments_to_Generated_PDF_ViewModel();

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
                0, currentPageUrl.Length - "Add_Attachments_to_Generated_PDF".Length);

            model.HtmlString = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/PDF_Standards.html"));
            model.BaseUrl = rootUrl + "DemoAppFiles/Input/HTML_Files/";

            return model;
        }

        // ===== Sample data builders =====

        private static string BuildSampleInvoiceXml()
        {
            return
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<Invoice>\n" +
                "  <InvoiceNumber>2026-0042</InvoiceNumber>\n" +
                "  <IssueDate>2026-05-18</IssueDate>\n" +
                "  <Customer>Acme Corporation</Customer>\n" +
                "  <Items>\n" +
                "    <Item><Name>Widget</Name><Quantity>10</Quantity><UnitPrice>1.50</UnitPrice></Item>\n" +
                "    <Item><Name>Gadget</Name><Quantity>5</Quantity><UnitPrice>3.75</UnitPrice></Item>\n" +
                "    <Item><Name>Sprocket</Name><Quantity>2</Quantity><UnitPrice>12.00</UnitPrice></Item>\n" +
                "  </Items>\n" +
                "  <Total>57.75</Total>\n" +
                "</Invoice>\n";
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoTextsPath() => Path.Combine(GetDemoFilesPath(), "Text_Files");
    }
}
