using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;

using System.Drawing;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class Create_File_Links_and_AtachmentsController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public Create_File_Links_and_AtachmentsController(IWebHostEnvironment hostingEnvironment)
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

            Document pdfDocument = null;
            try
            {
                string htmlWithLinksAndAttachMarkers = collection["htmlStringTextBox"];
                string baseUrl = collection["baseUrlTextBox"];

                // Convert a HTML string with markers for file links and attachments to a PDF document object
                pdfDocument = htmlToPdfConverter.ConvertHtmlToPdfDocumentObject(htmlWithLinksAndAttachMarkers, baseUrl);

                // Display the attachments panel when the PDF document is opened in a PDF viewer
                pdfDocument.ViewerPreferences.PageMode = ViewerPageMode.UseAttachments;

                // Create File Attachments

                // Create an attachment from a file without icon
                string fileAttachmentPath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Attach_Files/Attachment_File.txt";
                pdfDocument.AddFileAttachment(fileAttachmentPath, "Attachment from File");

                // Create an attachment from a stream without icon
                string fileStreamAttachmentPath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Attach_Files/Attachment_Stream.txt";
                System.IO.FileStream attachmentStream = new System.IO.FileStream(fileStreamAttachmentPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                pdfDocument.AddFileAttachment(attachmentStream, "Attachment_Stream.txt", "Attachment from Stream");

                // Create an attachment from file with paperclip icon in PDF
                HtmlElementMapping attachFromFileIconMapping = htmlToPdfConverter.HtmlElementsMappingOptions.HtmlElementsMappingResult.GetElementByMappingId("attach_from_file_icon");
                if (attachFromFileIconMapping != null)
                {
                    PdfPage attachFromFilePage = attachFromFileIconMapping.PdfRectangles[0].PdfPage;
                    RectangleF attachFromFileIconRectangle = attachFromFileIconMapping.PdfRectangles[0].Rectangle;

                    string fileAttachmentWithIconPath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Attach_Files/Attachment_File_Icon.txt";

                    // Create the attachment from file
                    FileAttachmentElement attachFromFileElement = new FileAttachmentElement(attachFromFileIconRectangle, fileAttachmentWithIconPath);
                    attachFromFileElement.IconType = FileAttachmentIcon.Paperclip;
                    attachFromFileElement.Text = "Attachment from File with Paperclip Icon";
                    attachFromFileElement.IconColor = Color.Blue;
                    attachFromFilePage.AddElement(attachFromFileElement);
                }

                // Create an attachment from stream with pushpin icon in PDF
                System.IO.FileStream attachmentStreamWithIcon = null;
                HtmlElementMapping attachFromStreamIconMapping = htmlToPdfConverter.HtmlElementsMappingOptions.HtmlElementsMappingResult.GetElementByMappingId("attach_from_stream_icon");
                if (attachFromStreamIconMapping != null)
                {
                    PdfPage attachFromStreamPage = attachFromStreamIconMapping.PdfRectangles[0].PdfPage;
                    RectangleF attachFromStreamIconRectangle = attachFromStreamIconMapping.PdfRectangles[0].Rectangle;

                    string fileStreamAttachmentWithIconPath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Attach_Files/Attachment_Stream_Icon.txt";

                    attachmentStreamWithIcon = new System.IO.FileStream(fileStreamAttachmentWithIconPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                    // Create the attachment from stream
                    FileAttachmentElement attachFromStreamElement = new FileAttachmentElement(attachFromStreamIconRectangle, attachmentStreamWithIcon, "Attachment_Stream_Icon.txt");
                    attachFromStreamElement.IconType = FileAttachmentIcon.PushPin;
                    attachFromStreamElement.Text = "Attachment from Stream with Pushpin Icon";
                    attachFromStreamElement.IconColor = Color.Green;
                    attachFromStreamPage.AddElement(attachFromStreamElement);
                }

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "File_Links_and_Attachments.pdf";

                return fileResult;
            }
            finally
            {
                // Close the PDF document
                if (pdfDocument != null)
                    pdfDocument.Close();
            }            
        }
    }
}