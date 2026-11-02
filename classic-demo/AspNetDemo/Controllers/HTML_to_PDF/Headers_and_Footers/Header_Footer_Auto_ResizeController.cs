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
    public class Header_Footer_Auto_ResizeController : Controller
    {
        // Define the HTML to PDF converter object as a class member to make it accessible in the headerHtml_NavigationCompletedEvent handler
        // where the header height will be automatically adjusted
        private HtmlToPdfConverter htmlToPdfConverter;

        // Indicates if a line should be drawn at the botom of the header
        private bool drawHeaderLine = true;

        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public Header_Footer_Auto_ResizeController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(IFormCollection collection)
        {
            // Create a HTML to PDF converter object with default settings
            htmlToPdfConverter = new HtmlToPdfConverter();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            htmlToPdfConverter.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
            htmlToPdfConverter.ConversionDelay = 2;

            // Enable header in the generated PDF document
            htmlToPdfConverter.PdfDocumentOptions.ShowHeader = true;

            string headerHtmlUrl = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/HTML_Files/Header_HTML.html";
            Document documentObject = null;
            try
            {
                if (collection["AutoResize"] == "autoResizeHeaderRadioButton")
                {
                    // Create a HTML element to be added in header
                    HtmlToPdfElement headerHtml = new HtmlToPdfElement(headerHtmlUrl);

                    // Install a handler where to set the automatically calculated header height
                    headerHtml.NavigationCompletedEvent += new NavigationCompletedDelegate(headerHtml_NavigationCompletedEvent);

                    // Add the HTML element to header
                    // When the element is rendered in header by converter, the headerHtml_NavigationCompletedEvent handler 
                    // will be invoked and the header height will be automatically calculated
                    htmlToPdfConverter.PdfHeaderOptions.AddElement(headerHtml);

                    // Call the converter to produce a Document object
                    documentObject = htmlToPdfConverter.ConvertUrlToPdfDocumentObject(collection["urlTextBox"]);

                    // Uninstall the handler
                    headerHtml.NavigationCompletedEvent -= new NavigationCompletedDelegate(headerHtml_NavigationCompletedEvent);

                    // Draw a line at the header bottom
                    if (drawHeaderLine)
                    {
                        float headerWidth = documentObject.Header.Width;
                        float headerHeight = documentObject.Header.Height;

                        // Create a line element for the bottom of the header
                        LineElement headerLine = new LineElement(0, headerHeight - 1, headerWidth, headerHeight - 1);

                        // Set line color
                        headerLine.ForeColor = Color.Gray;

                        // Add line element to the bottom of the header
                        documentObject.Header.AddElement(headerLine);
                    }

                    // Save the PDF document in a memory buffer
                    byte[] outPdfBuffer = documentObject.Save();
                    
                    // Send the PDF file to browser
                    FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                    fileResult.FileDownloadName = "Auto_Resize_Header_Footer.pdf";

                    return fileResult;
                }
                else
                {
                    // Create a HTML to PDF element to be added in header
                    HtmlToPdfElement headerHtml = new HtmlToPdfElement(headerHtmlUrl);

                    // Set a fixed header height in points
                    htmlToPdfConverter.PdfHeaderOptions.HeaderHeight = float.Parse(collection["headerHeightTextBox"]);

                    // Set the HTML element to fit the container height
                    headerHtml.FitHeight = true;

                    // Add HTML element to fit the fixed header height
                    htmlToPdfConverter.PdfHeaderOptions.AddElement(headerHtml);

                    // Draw a line at the header bottom
                    if (drawHeaderLine)
                    {
                        // Calculate the header width based on PDF page size and margins
                        float headerWidth = htmlToPdfConverter.PdfDocumentOptions.PdfPageSize.Width -
                                    htmlToPdfConverter.PdfDocumentOptions.LeftMargin - htmlToPdfConverter.PdfDocumentOptions.RightMargin;

                        // Calculate header height
                        float headerHeight = htmlToPdfConverter.PdfHeaderOptions.HeaderHeight;

                        // Create a line element for the bottom of the header
                        LineElement headerLine = new LineElement(0, headerHeight - 1, headerWidth, headerHeight - 1);

                        // Set line color
                        headerLine.ForeColor = Color.Gray;

                        // Add line element to the bottom of the header
                        htmlToPdfConverter.PdfHeaderOptions.AddElement(headerLine);
                    }

                    // Convert the HTML page to a PDF document in a memory buffer
                    byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(collection["urlTextBox"]);
                    
                    // Send the PDF file to browser
                    FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                    fileResult.FileDownloadName = "Auto_Resize_Header_Footer.pdf";

                    return fileResult;
                }
            }
            finally
            {
                // Close the PDF document
                if (documentObject != null)
                    documentObject.Close();
            }
        }

        /// <summary>
        /// This handler is called after the navigation to header HTML completed. The document header is resized in this event handler
        /// </summary>
        /// <param name="eventParams">The event parameter containing the HTML content size in pixels and points</param>
        void headerHtml_NavigationCompletedEvent(NavigationCompletedParams eventParams)
        {
            // Get the header HTML width and height from event parameters
            float headerHtmlWidth = eventParams.HtmlContentWidthPt;
            float headerHtmlHeight = eventParams.HtmlContentHeightPt;

            // Calculate the header width from coverter settings
            float headerWidth = htmlToPdfConverter.PdfDocumentOptions.PdfPageSize.Width - htmlToPdfConverter.PdfDocumentOptions.LeftMargin -
                        htmlToPdfConverter.PdfDocumentOptions.RightMargin;

            // Calculate a resize factor to fit the header width
            float resizeFactor = 1;
            if (headerHtmlWidth > headerWidth)
                resizeFactor = headerWidth / headerHtmlWidth;

            // Calculate the header height to preserve the HTML aspect ratio
            float headerHeight = headerHtmlHeight * resizeFactor;

            if (!(headerHeight < htmlToPdfConverter.PdfDocumentOptions.PdfPageSize.Height - htmlToPdfConverter.PdfDocumentOptions.TopMargin -
                        htmlToPdfConverter.PdfDocumentOptions.BottomMargin))
            {
                throw new Exception("The header height cannot be bigger than PDF page height");
            }

            // Set the calculated header height
            htmlToPdfConverter.PdfDocumentOptions.DocumentObject.Header.Height = headerHeight;
        }
    }
}