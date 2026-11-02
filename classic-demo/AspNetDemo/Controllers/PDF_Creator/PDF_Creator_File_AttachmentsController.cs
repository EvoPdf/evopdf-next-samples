using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;
using System.Drawing;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator
{
    public class PDF_Creator_File_AttachmentsController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public PDF_Creator_File_AttachmentsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: PDF_Creator_File_Attachments
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePdf(IFormCollection collection)
        {
            // Create a PDF document
            Document pdfDocument = new Document();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            pdfDocument.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Display the attachments panel when the PDF document is opened in a PDF viewer
            pdfDocument.ViewerPreferences.PageMode = ViewerPageMode.UseAttachments;

            // Add a page to PDF document
            PdfPage pdfPage = pdfDocument.AddPage();

            try
            {
                // The titles font used to mark various sections of the PDF document
                PdfFont titleFont = pdfDocument.AddFont(new Font("Times New Roman", 10, FontStyle.Bold, GraphicsUnit.Point));
                PdfFont subtitleFont = pdfDocument.AddFont(new Font("Times New Roman", 8, FontStyle.Regular, GraphicsUnit.Point));

                float xLocation = 5;
                float yLocation = 5;

                // Add document title
                TextElement titleTextElement = new TextElement(xLocation, yLocation, "Attach Files and Streams to a PDF Document", titleFont);
                AddElementResult addElementResult = pdfPage.AddElement(titleTextElement);

                yLocation = addElementResult.EndPageBounds.Bottom + 15;

                // Create an attachment from a file without icon
                string fileAttachmentPath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Attach_Files/Attachment_File.txt";
                pdfDocument.AddFileAttachment(fileAttachmentPath, "Attachment from File");

                // Create an attachment from a stream without icon
                string fileStreamAttachmentPath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Attach_Files/Attachment_Stream.txt";
                System.IO.FileStream attachmentStream = new System.IO.FileStream(fileStreamAttachmentPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                pdfDocument.AddFileAttachment(attachmentStream, "Attachment_Stream.txt", "Attachment from Stream");

                // Add the text element
                string text = "Click the next icon to open the attachment from a file:";
                float textWidth = subtitleFont.GetTextWidth(text);
                TextElement textElement = new TextElement(xLocation, yLocation, text, subtitleFont);
                addElementResult = pdfPage.AddElement(textElement);

                // Create an attachment from file with paperclip icon in PDF
                string fileAttachmentWithIconPath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Attach_Files/Attachment_File_Icon.txt";
                // Create the attachment from file
                RectangleF attachFromFileIconRectangle = new RectangleF(xLocation + textWidth + 3, yLocation, 6, 10);
                FileAttachmentElement attachFromFileElement = new FileAttachmentElement(attachFromFileIconRectangle, fileAttachmentWithIconPath);
                attachFromFileElement.IconType = FileAttachmentIcon.Paperclip;
                attachFromFileElement.Text = "Attachment from File with Paperclip Icon";
                attachFromFileElement.IconColor = Color.Blue;
                pdfPage.AddElement(attachFromFileElement);

                yLocation = addElementResult.EndPageBounds.Bottom + 10;

                // Add the text element
                text = "Click the next icon to open the attachment from a stream:";
                textWidth = subtitleFont.GetTextWidth(text);
                textElement = new TextElement(xLocation, yLocation, text, subtitleFont);
                addElementResult = pdfPage.AddElement(textElement);

                // Create an attachment from stream with pushpin icon in PDF
                string fileStreamAttachmentWithIconPath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Attach_Files/Attachment_Stream_Icon.txt";
                System.IO.FileStream attachmentStreamWithIcon = new System.IO.FileStream(fileStreamAttachmentWithIconPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                // Create the attachment from stream
                RectangleF attachFromStreamIconRectangle = new RectangleF(xLocation + textWidth + 3, yLocation, 6, 10);
                FileAttachmentElement attachFromStreamElement = new FileAttachmentElement(attachFromStreamIconRectangle, attachmentStreamWithIcon, "Attachment_Stream_Icon.txt");
                attachFromStreamElement.IconType = FileAttachmentIcon.PushPin;
                attachFromStreamElement.Text = "Attachment from Stream with Pushpin Icon";
                attachFromStreamElement.IconColor = Color.Green;
                pdfPage.AddElement(attachFromStreamElement);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "File_Attachments.pdf";

                return fileResult;
            }
            finally
            {
                // Close the PDF document
                pdfDocument.Close();
            }
        }
    }
}