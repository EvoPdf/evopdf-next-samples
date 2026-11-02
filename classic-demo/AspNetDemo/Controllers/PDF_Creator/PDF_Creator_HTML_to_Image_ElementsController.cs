using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator
{
    public class PDF_Creator_HTML_to_Image_ElementsController : Controller
    {
        // GET: PDF_Creator_HTML_to_Image_Elements
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

            // Create a PDF page where to add the first HTML
            PdfPage firstPdfPage = pdfDocument.AddPage();

            try
            {
                // The image location in PDF
                float xLocation = float.Parse(collection["xLocationTextBox"]);
                float yLocation = float.Parse(collection["yLocationTextBox"]);

                // The URL of the HTML page to convert to an image in PDF
                string urlToConvert = collection["urlTextBox"];

                // Create the HTML to Image element
                HtmlToImageElement htmlToImageElement = new HtmlToImageElement(xLocation, yLocation, urlToConvert);

                // Optionally set the HTML viewer width
                htmlToImageElement.HtmlViewerWidth = int.Parse(collection["htmlViewerWidthTextBox"]);

                // Optionally set the HTML viewer height
                if (collection["htmlViewerHeightTextBox"][0].Length > 0)
                    htmlToImageElement.HtmlViewerHeight = int.Parse(collection["htmlViewerHeightTextBox"]);

                // Optionally set the HTML content clipping option to force the HTML content width to be exactly HtmlViewerWidth pixels
                htmlToImageElement.ClipHtmlView = collection["clipContentCheckBox"].Count > 0;

                // Optionally set the destination width in PDF
                if (collection["contentWidthTextBox"][0].Length > 0)
                    htmlToImageElement.Width = float.Parse(collection["contentWidthTextBox"]);

                // Optionally set the destination height in PDF
                if (collection["contentHeightTextBox"][0].Length > 0)
                    htmlToImageElement.Height = float.Parse(collection["contentHeightTextBox"]);

                // Optionally set a delay before conversion to allow asynchonous scripts to finish
                htmlToImageElement.ConversionDelay = 2;

                // Add the HTML to Image element to PDF document
                // The AddElementResult contains the bounds of the HTML to Image Element in last rendered PDF page
                // such that you can start a new PDF element right under it
                AddElementResult result = firstPdfPage.AddElement(htmlToImageElement);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "HTML_to_Image_Elements.pdf";

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