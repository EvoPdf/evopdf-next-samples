using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using System.Drawing;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.HTML_Elements_Location
{
    public class Select_in_API_Elements_to_RetrieveController : Controller
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

            // Select the HTML elements for which to retrieve location and other information from HTML document
            htmlToPdfConverter.HtmlElementsMappingOptions.HtmlElementSelectors = new string[] { collection["htmlElementsSelectorTextBox"] };

            Document pdfDocument = null;
            try
            {
                // Convert HTML page to a PDF document object which can be further modified to highlight the selected elements
                pdfDocument = htmlToPdfConverter.ConvertUrlToPdfDocumentObject(collection["urlTextBox"]);

                // Highlight the selected elements in PDF with colored rectangles
                foreach (HtmlElementMapping htmlElementInfo in htmlToPdfConverter.HtmlElementsMappingOptions.HtmlElementsMappingResult)
                {
                    // Get other information about HTML element
                    string htmlElementTagName = htmlElementInfo.HtmlElementTagName;
                    string htmlElementID = htmlElementInfo.HtmlElementId;

                    // Hightlight the HTML element in PDF

                    // A HTML element can span over many PDF pages and therefore the mapping of the HTML element in PDF document consists 
                    // in a list of rectangles, one rectangle for each PDF page where this element was rendered
                    foreach (HtmlElementPdfRectangle htmlElementLocationInPdf in htmlElementInfo.PdfRectangles)
                    {
                        // Get the HTML element location in PDF page
                        PdfPage htmlElementPdfPage = htmlElementLocationInPdf.PdfPage;
                        RectangleF htmlElementRectangleInPdfPage = htmlElementLocationInPdf.Rectangle;

                        // Highlight the HTML element element with a colored rectangle in PDF
                        RectangleElement highlightRectangle = new RectangleElement(htmlElementRectangleInPdfPage.X, htmlElementRectangleInPdfPage.Y,
                            htmlElementRectangleInPdfPage.Width, htmlElementRectangleInPdfPage.Height);

                        if (htmlElementTagName.ToLower() == "h1")
                            highlightRectangle.ForeColor = Color.Blue;
                        else if (htmlElementTagName.ToLower() == "h2")
                            highlightRectangle.ForeColor = Color.Green;
                        else if (htmlElementTagName.ToLower() == "h3")
                            highlightRectangle.ForeColor = Color.Red;
                        else if (htmlElementTagName.ToLower() == "h4")
                            highlightRectangle.ForeColor = Color.Yellow;
                        else if (htmlElementTagName.ToLower() == "h5")
                            highlightRectangle.ForeColor = Color.Indigo;
                        else if (htmlElementTagName.ToLower() == "h6")
                            highlightRectangle.ForeColor = Color.Orange;
                        else
                            highlightRectangle.ForeColor = Color.Navy;

                        highlightRectangle.LineStyle.LineDashStyle = LineDashStyle.Solid;

                        htmlElementPdfPage.AddElement(highlightRectangle);
                    }
                }

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Select_in_API_HTML_Elements_to_Retrieve.pdf";

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