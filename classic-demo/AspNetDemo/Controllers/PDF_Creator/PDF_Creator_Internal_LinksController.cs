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
    public class PDF_Creator_Internal_LinksController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public PDF_Creator_Internal_LinksController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: PDF_Creator_Internal_Links
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

            // Add second page to PDF document
            PdfPage secondPdfPage = pdfDocument.AddPage();

            // Add third page to PDF document
            PdfPage thirdPdfPage = pdfDocument.AddPage();

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
                TextElement titleTextElement = new TextElement(xLocation, yLocation, "Create Internal Links in PDF Document", titleFont);
                AddElementResult addElementResult = pdfPage.AddElement(titleTextElement);

                yLocation = addElementResult.EndPageBounds.Bottom + 15;

                // Add a text in second page
                TextElement secondPageTextElement = new TextElement(5, 5, "This text is the target of an internal text link", subtitleFont);
                secondPdfPage.AddElement(secondPageTextElement);

                // Add a text in third page
                TextElement thirdPageTextElement = new TextElement(5, 5, "This text is the target of an internal image link", subtitleFont);
                thirdPdfPage.AddElement(thirdPageTextElement);

                // Make a text in PDF an internal link to the second page of the PDF document

                // Add the text element
                string text = "Click this text to go to the second page of this document!";
                float textWidth = linkTextFont.GetTextWidth(text);
                TextElement linkTextElement = new TextElement(xLocation, yLocation, text, linkTextFont);
                linkTextElement.ForeColor = Color.Navy;
                addElementResult = pdfPage.AddElement(linkTextElement);

                // Make the text element an internal link to the second page of this document
                RectangleF linkRectangle = new RectangleF(xLocation, yLocation, textWidth, addElementResult.EndPageBounds.Height);
                // Create the destination in second page
                ExplicitDestination secondPageDestination = new ExplicitDestination(secondPdfPage, new PointF(5, 5));
                // Create the internal link from text element to second page
                InternalLinkElement internalLink = new InternalLinkElement(linkRectangle, secondPageDestination);

                // Add the internal link to PDF document
                pdfPage.AddElement(internalLink);

                yLocation = addElementResult.EndPageBounds.Bottom + 10;

                // Make an image in PDF an internal link to the third page of the PDF document

                TextElement subtitleTextElement = new TextElement(xLocation, yLocation, "Click the image below to go to the third page of this document:", subtitleFont);
                addElementResult = pdfPage.AddElement(subtitleTextElement);

                yLocation = addElementResult.EndPageBounds.Bottom + 5;

                // Add the image element
                ImageElement linkImageElement = new ImageElement(xLocation, yLocation, 120, m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Images/logo.jpg");
                addElementResult = pdfPage.AddElement(linkImageElement);

                // Make the image element an internal link to the third page of this document
                linkRectangle = addElementResult.EndPageBounds;
                // Create the destination in third page
                ExplicitDestination thirdPageDestination = new ExplicitDestination(thirdPdfPage, new PointF(5, 5));
                // Create the internal link from image element to third page
                internalLink = new InternalLinkElement(linkRectangle, thirdPageDestination);

                // Add the internal link to PDF document
                pdfPage.AddElement(internalLink);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Internal_Links.pdf";

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