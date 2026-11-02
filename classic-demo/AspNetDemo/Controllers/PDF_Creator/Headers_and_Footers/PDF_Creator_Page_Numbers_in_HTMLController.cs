using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using System.Drawing;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator.Headers_and_Footers
{
    public class PDF_Creator_Page_Numbers_in_HTMLController : Controller
    {
        [HttpPost]
        public ActionResult CreatePdf(IFormCollection collection)
        {
            // Create a PDF document
            Document pdfDocument = new Document();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            pdfDocument.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Add a PDF page to PDF document
            PdfPage pdfPage = pdfDocument.AddPage();

            try
            {
                // Create the document footer template
                pdfDocument.AddFooterTemplate(50);

                // ----- Add HTML with Page Numbering to Footer -----

                // Create a variable HTML element with page numbering
                string htmlStringWithPageNumbers = collection["htmlWithPageNumbersTextBox"];
                string baseUrl = collection["baseUrlTextBox"];
                HtmlToPdfVariableElement footerHtmlWithPageNumbers = new HtmlToPdfVariableElement(htmlStringWithPageNumbers, baseUrl);

                // Set the HTML element to fit the container height
                footerHtmlWithPageNumbers.FitHeight = true;

                // Add variable HTML element with page numbering to footer
                pdfDocument.Footer.AddElement(footerHtmlWithPageNumbers);

                // Optionally draw a line at the top of the footer
                if (collection["drawFooterLineCheckBox"].Count > 0)
                {
                    float footerWidth = pdfDocument.Footer.Width;

                    // Create a line element for the top of the footer
                    LineElement footerLine = new LineElement(0, 0, footerWidth, 0);

                    // Set line color
                    footerLine.ForeColor = Color.Gray;

                    // Add line element to the bottom of the footer
                    pdfDocument.Footer.AddElement(footerLine);
                }

                // Create a HTML to PDF element to add to document
                HtmlToPdfElement htmlToPdfElement = new HtmlToPdfElement(0, 0, collection["urlTextBox"]);

                // Optionally set a delay before conversion to allow asynchonous scripts to finish
                htmlToPdfElement.ConversionDelay = 2;

                // Optionally add a space between footer and the page body
                // Leave this option not set for no spacing
                htmlToPdfElement.BottomSpacing = float.Parse(collection["footerSpacingTextBox"]);

                // Add the HTML to PDF element to document
                // This will raise the PrepareRenderPdfPageEvent event where the header and footer visibilit per page can be changed
                pdfPage.AddElement(htmlToPdfElement);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Page_Numbers_in_HTML.pdf";

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