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
    public class PDF_Creator_URI_LinksController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public PDF_Creator_URI_LinksController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: PDF_Creator_URI_Links
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

            // Add a page to PDF document
            PdfPage pdfPage = pdfDocument.AddPage();

            try
            {
                // The titles font used to mark various sections of the PDF document
                PdfFont titleFont = pdfDocument.AddFont(new Font("Times New Roman", 10, FontStyle.Bold, GraphicsUnit.Point));
                PdfFont subtitleFont = pdfDocument.AddFont(new Font("Times New Roman", 8, FontStyle.Regular, GraphicsUnit.Point));

                // The links text font
                PdfFont linkTextFont = pdfDocument.AddFont(new Font("Times New Roman", 8, FontStyle.Bold, GraphicsUnit.Point));
                linkTextFont.IsUnderline = true;

                float xLocation = 5;
                float yLocation = 5;

                // Add document title
                TextElement titleTextElement = new TextElement(xLocation, yLocation, "Create URI Links in PDF Document", titleFont);
                AddElementResult addElementResult = pdfPage.AddElement(titleTextElement);

                yLocation = addElementResult.EndPageBounds.Bottom + 15;

                // Make a text in PDF a link to a web page

                // Add the text element
                string text = "Click this text to open a web page!";
                float textWidth = linkTextFont.GetTextWidth(text);
                TextElement linkTextElement = new TextElement(xLocation, yLocation, text, linkTextFont);
                linkTextElement.ForeColor = Color.Navy;
                addElementResult = pdfPage.AddElement(linkTextElement);

                // Create the URI link element having the size of the text element
                RectangleF linkRectangle = new RectangleF(xLocation, yLocation, textWidth, addElementResult.EndPageBounds.Height);
                string url = "http://www.evopdf.com";
                LinkUrlElement uriLink = new LinkUrlElement(linkRectangle, url);

                // Add the URI link to PDF document
                pdfPage.AddElement(uriLink);

                yLocation = addElementResult.EndPageBounds.Bottom + 10;

                // Make an image in PDF a link to a web page

                TextElement subtitleTextElement = new TextElement(xLocation, yLocation, "Click the image below to open a web page:", subtitleFont);
                addElementResult = pdfPage.AddElement(subtitleTextElement);

                yLocation = addElementResult.EndPageBounds.Bottom + 5;

                // Add the image element
                ImageElement linkImageElement = new ImageElement(xLocation, yLocation, 120, m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Images/logo.jpg");
                addElementResult = pdfPage.AddElement(linkImageElement);

                // Create the URI link element having the size of the image element
                linkRectangle = addElementResult.EndPageBounds;
                uriLink = new LinkUrlElement(linkRectangle, url);

                // Add the URI link to PDF document
                pdfPage.AddElement(uriLink);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "URI_Links.pdf";

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