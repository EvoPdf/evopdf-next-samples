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

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.Headers_and_Footers
{
    public class Page_Numbers_in_HTMLController : Controller
    {
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

            // Enable footer in the generated PDF document
            htmlToPdfConverter.PdfDocumentOptions.ShowFooter = true;

            // Optionally add a space between footer and the page body
            // Leave this option not set for no spacing
            htmlToPdfConverter.PdfDocumentOptions.BottomSpacing = float.Parse(collection["footerSpacingTextBox"]);

            // Set the footer height in points
            htmlToPdfConverter.PdfFooterOptions.FooterHeight = 50;

            // ----- Add HTML with Page Numbering to Footer -----

            // Create a variable HTML element with page numbering
            string htmlStringWithPageNumbers = collection["htmlWithPageNumbersTextBox"];
            string baseUrl = collection["baseUrlTextBox"];
            HtmlToPdfVariableElement footerHtmlWithPageNumbers = new HtmlToPdfVariableElement(htmlStringWithPageNumbers, baseUrl);

            // Set the HTML element to fit the container height
            footerHtmlWithPageNumbers.FitHeight = true;

            // Add variable HTML element with page numbering to footer
            htmlToPdfConverter.PdfFooterOptions.AddElement(footerHtmlWithPageNumbers);

            // Optionally draw a line at the top of the footer
            if (collection["drawFooterLineCheckBox"].Count > 0)
            {
                // Calculate the footer width based on PDF page size and margins
                float footerWidth = htmlToPdfConverter.PdfDocumentOptions.PdfPageSize.Width -
                            htmlToPdfConverter.PdfDocumentOptions.LeftMargin - htmlToPdfConverter.PdfDocumentOptions.RightMargin;

                // Create a line element for the top of the footer
                LineElement footerLine = new LineElement(0, 0, footerWidth, 0);

                // Set line color
                footerLine.ForeColor = Color.Gray;

                // Add line element to the bottom of the footer
                htmlToPdfConverter.PdfFooterOptions.AddElement(footerLine);
            }

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(collection["urlTextBox"]);
            
            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Page_Numbers_in_HTML.pdf";

            return fileResult;
        }
    }
}