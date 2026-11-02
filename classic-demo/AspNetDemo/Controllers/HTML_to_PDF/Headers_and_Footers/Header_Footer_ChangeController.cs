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
    public class Header_Footer_ChangeController : Controller
    {
        IFormCollection formCollection;

        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public Header_Footer_ChangeController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(IFormCollection collection)
        {
            formCollection = collection;

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            htmlToPdfConverter.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
            htmlToPdfConverter.ConversionDelay = 2;

            // Install a handler where to change the header and footer in first page
            htmlToPdfConverter.PrepareRenderPdfPageEvent += new PrepareRenderPdfPageDelegate(htmlToPdfConverter_PrepareRenderPdfPageEvent);

            // Add Document Header

            // Enable header in the generated PDF document
            htmlToPdfConverter.PdfDocumentOptions.ShowHeader = true;

            // Draw document header elements
            if (htmlToPdfConverter.PdfDocumentOptions.ShowHeader)
                DrawHeader(htmlToPdfConverter, true);

            // Add Document Footer

            // Enable footer in the generated PDF document
            htmlToPdfConverter.PdfDocumentOptions.ShowFooter = true;

            // Draw document footer elements
            if (htmlToPdfConverter.PdfDocumentOptions.ShowFooter)
                DrawFooter(htmlToPdfConverter, true, true);

            // Convert the HTML page to PDF document in a memory buffer
            // The PrepareRenderPdfPageEvent event handler will be invoked for each rendered PDF page
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(collection["urlTextBox"]);
            
            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Change_Header_Footer_Per_Page.pdf";

            return fileResult;
        }

        /// <summary>
        /// The handler for HtmlToPdfConverter.PrepareRenderPdfPageEvent event where you can set the visibility of header and footer
        /// in each page or you can add a custom header or footer in a page
        /// </summary>
        /// <param name="eventParams">The event parameter containin the PDF page to customize before rendering</param>
        void htmlToPdfConverter_PrepareRenderPdfPageEvent(PrepareRenderPdfPageParams eventParams)
        {
            if ((eventParams.PageNumber == 1) && formCollection["changeHeaderAndFooterCheckBox"].Count > 0)
            {
                // Change the header and footer in first page with an alternative header and footer

                // The PDF page being rendered
                PdfPage pdfPage = eventParams.Page;

                // Add a custom header of 80 points in height to this page
                // The default document header will be replaced in this page
                pdfPage.AddHeaderTemplate(80);
                // Draw the page header elements
                DrawAlternativePageHeader(pdfPage.Header, true);

                // Add a custom footer of 80 points in height to this page
                // The default document footer will be replaced in this page
                pdfPage.AddFooterTemplate(80);
                // Draw the page header elements
                DrawAlternativePageFooter(pdfPage.Footer, true, true);
            }
        }

        /// <summary>
        /// Draw the document header elements
        /// </summary>
        /// <param name="htmlToPdfConverter">The HTML to PDF Converter object</param>
        /// <param name="drawHeaderLine">A flag indicating if a line should be drawn at the bottom of the header</param>
        private void DrawHeader(HtmlToPdfConverter htmlToPdfConverter, bool drawHeaderLine)
        {
            string headerHtmlUrl = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/HTML_Files/Header_HTML.html";

            // Set the header height in points
            htmlToPdfConverter.PdfHeaderOptions.HeaderHeight = 60;

            // Set header background color
            htmlToPdfConverter.PdfHeaderOptions.HeaderBackColor = Color.White;

            // Create a HTML element to be added in header
            HtmlToPdfElement headerHtml = new HtmlToPdfElement(headerHtmlUrl);

            // Set the HTML element to fit the container height
            headerHtml.FitHeight = true;

            // Add HTML element to header
            htmlToPdfConverter.PdfHeaderOptions.AddElement(headerHtml);

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
        }

        /// <summary>
        /// Draw the document footer elements
        /// </summary>
        /// <param name="htmlToPdfConverter">The HTML to PDF Converter object</param>
        /// <param name="addPageNumbers">A flag indicating if the page numbering is present in footer</param>
        /// <param name="drawFooterLine">A flag indicating if a line should be drawn at the top of the footer</param>
        private void DrawFooter(HtmlToPdfConverter htmlToPdfConverter, bool addPageNumbers, bool drawFooterLine)
        {
            string footerHtmlUrl = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/HTML_Files/Footer_HTML.html";

            // Set the footer height in points
            htmlToPdfConverter.PdfFooterOptions.FooterHeight = 60;

            // Set footer background color
            htmlToPdfConverter.PdfFooterOptions.FooterBackColor = Color.WhiteSmoke;

            // Create a HTML element to be added in footer
            HtmlToPdfElement footerHtml = new HtmlToPdfElement(footerHtmlUrl);

            // Set the HTML element to fit the container height
            footerHtml.FitHeight = true;

            // Add HTML element to footer
            htmlToPdfConverter.PdfFooterOptions.AddElement(footerHtml);

            // Add page numbering
            if (addPageNumbers)
            {
                // Create a text element with page numbering place holders &p; and & P;
                TextElement footerText = new TextElement(0, 30, "Page &p; of &P;  ",
                    new System.Drawing.Font(new System.Drawing.FontFamily("Times New Roman"), 10, System.Drawing.GraphicsUnit.Point));

                // Align the text at the right of the footer
                footerText.TextAlign = HorizontalTextAlign.Right;

                // Set page numbering text color
                footerText.ForeColor = Color.Navy;

                // Embed the text element font in PDF
                footerText.EmbedSysFont = true;

                // Add the text element to footer
                htmlToPdfConverter.PdfFooterOptions.AddElement(footerText);
            }

            if (drawFooterLine)
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
        }

        /// <summary>
        /// Draw the alternative page header elements
        /// </summary>
        /// <param name="headerTemplate">The page header template</param>
        /// <param name="drawHeaderLine">A flag indicating if a line should be drawn at the bottom of the header</param>
        private void DrawAlternativePageHeader(Template headerTemplate, bool drawHeaderLine)
        {
            string headerHtmlUrl = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/HTML_Files/Header_Alt_HTML.html";

            // Create a HTML element to be added in header
            HtmlToPdfElement headerHtml = new HtmlToPdfElement(headerHtmlUrl);

            // Set the HTML element to fit the container height
            headerHtml.FitHeight = true;

            // Add HTML element to header
            headerTemplate.AddElement(headerHtml);

            if (drawHeaderLine)
            {
                float headerWidth = headerTemplate.Width;
                float headerHeight = headerTemplate.Height;

                // Create a line element for the bottom of the header
                LineElement headerLine = new LineElement(0, headerHeight - 1, headerWidth, headerHeight - 1);

                // Set line color
                headerLine.ForeColor = Color.Gray;

                // Add line element to the bottom of the header
                headerTemplate.AddElement(headerLine);
            }
        }

        /// <summary>
        /// Draw the alternative page footer elements
        /// </summary>
        /// <param name="htmlToPdfConverter">The HTML to PDF Converter object</param>
        /// <param name="addPageNumbers">A flag indicating if the page numbering is present in footer</param>
        /// <param name="drawFooterLine">A flag indicating if a line should be drawn at the top of the footer</param>
        private void DrawAlternativePageFooter(Template footerTemplate, bool addPageNumbers, bool drawFooterLine)
        {
            string footerHtmlUrl = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/HTML_Files/Footer_Alt_HTML.html";

            // Create a HTML element to be added in footer
            HtmlToPdfElement footerHtml = new HtmlToPdfElement(footerHtmlUrl);

            // Set the HTML element to fit the container height
            footerHtml.FitHeight = true;

            // Add HTML element to footer
            footerTemplate.AddElement(footerHtml);

            // Add page numbering
            if (addPageNumbers)
            {
                // Create a text element with page numbering place holders &p; and & P;
                TextElement footerText = new TextElement(10, 30, "Page &p; of &P;",
                    new System.Drawing.Font(new System.Drawing.FontFamily("Times New Roman"), 10, System.Drawing.GraphicsUnit.Point));

                // Align the text at the right of the footer
                footerText.TextAlign = HorizontalTextAlign.Left;

                // Set page numbering text color
                footerText.ForeColor = Color.Navy;

                // Embed the text element font in PDF
                footerText.EmbedSysFont = true;

                // Add the text element to footer
                footerTemplate.AddElement(footerText);
            }

            if (drawFooterLine)
            {
                float footerWidth = footerTemplate.Width;

                // Create a line element for the top of the footer
                LineElement footerLine = new LineElement(0, 0, footerWidth, 0);

                // Set line color
                footerLine.ForeColor = Color.Gray;

                // Add line element to the bottom of the footer
                footerTemplate.AddElement(footerLine);
            }
        }
    }
}