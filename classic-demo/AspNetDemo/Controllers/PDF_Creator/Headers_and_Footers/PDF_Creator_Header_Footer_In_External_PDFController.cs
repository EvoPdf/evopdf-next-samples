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

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator.Headers_and_Footers
{
    public class PDF_Creator_Header_Footer_In_External_PDFController : Controller
    {
        IFormCollection formCollection;

        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public PDF_Creator_Header_Footer_In_External_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public ActionResult CreatePdf(IFormCollection collection)
        {
            formCollection = collection;

            // Create a PDF document
            Document pdfDocument = new Document();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            pdfDocument.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Add a PDF page to PDF document
            PdfPage pdfPage = pdfDocument.AddPage();

            try
            {
                // Add a default document header
                AddHeader(pdfDocument, true);

                // Add a default document footer
                AddFooter(pdfDocument, true, true);

                // Create a HTML to PDF element to add to document
                HtmlToPdfElement htmlToPdfElement = new HtmlToPdfElement(0, 0, collection["urlTextBox"]);

                // Optionally set a delay before conversion to allow asynchonous scripts to finish
                htmlToPdfElement.ConversionDelay = 2;

                // Add HTML to PDF element to document
                pdfPage.AddElement(htmlToPdfElement);

                // Automatically close the external PDF documents after the final document is saved
                pdfDocument.AutoCloseAppendedDocs = true;

                // Insert an external PDF document in the beginning of the final PDF document
                string pdfFileBefore = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/PDF_Files/Merge_Before_Conversion.pdf";
                Document startExternalDocument = new Document(pdfFileBefore);
                pdfDocument.InsertDocument(0, startExternalDocument, collection["addHeaderFooterInInsertedPdfCheckBox"].Count > 0,
                                collection["showHeaderInFirstPageCheckBox"].Count > 0, collection["showFooterInFirstPageCheckBox"].Count > 0);

                // Append an external PDF document at the end of the final PDF document
                string pdfFileAfter = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/PDF_Files/Merge_After_Conversion.pdf";
                Document endExternalDocument = new Document(pdfFileAfter);
                pdfDocument.AppendDocument(endExternalDocument, collection["addHeaderFooterInAppendedPdfCheckBox"].Count > 0, true, true);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Header_Footer_in_External_PDF.pdf";

                return fileResult;
            }
            finally
            {
                // Close the PDF document
                pdfDocument.Close();
            }
        }

        /// <summary>
        /// Add a header to document
        /// </summary>
        /// <param name="pdfDocument">The PDF document object</param>
        /// <param name="drawHeaderLine">A flag indicating if a line should be drawn at the bottom of the header</param>
        private void AddHeader(Document pdfDocument, bool drawHeaderLine)
        {
            string headerHtmlUrl = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/HTML_Files/Header_HTML.html";

            // Create the document footer template
            pdfDocument.AddHeaderTemplate(60);

            // Create a HTML element to be added in header
            HtmlToPdfElement headerHtml = new HtmlToPdfElement(headerHtmlUrl);

            // Set the HTML element to fit the container height
            headerHtml.FitHeight = true;

            // Add HTML element to header
            pdfDocument.Header.AddElement(headerHtml);

            if (drawHeaderLine)
            {
                float headerWidth = pdfDocument.Header.Width;
                float headerHeight = pdfDocument.Header.Height;

                // Create a line element for the bottom of the header
                LineElement headerLine = new LineElement(0, headerHeight - 1, headerWidth, headerHeight - 1);

                // Set line color
                headerLine.ForeColor = Color.Gray;

                // Add line element to the bottom of the header
                pdfDocument.Header.AddElement(headerLine);
            }
        }

        /// <summary>
        /// Add a footer to document
        /// </summary>
        /// <param name="pdfDocument">The PDF document object</param>
        /// <param name="addPageNumbers">A flag indicating if the page numbering is present in footer</param>
        /// <param name="drawFooterLine">A flag indicating if a line should be drawn at the top of the footer</param>
        private void AddFooter(Document pdfDocument, bool addPageNumbers, bool drawFooterLine)
        {
            string footerHtmlUrl = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/HTML_Files/Footer_HTML.html";

            // Create the document footer template
            pdfDocument.AddFooterTemplate(60);

            // Set footer background color
            RectangleElement backColorRectangle = new RectangleElement(0, 0, pdfDocument.Footer.Width, pdfDocument.Footer.Height);
            backColorRectangle.BackColor = Color.WhiteSmoke;
            pdfDocument.Footer.AddElement(backColorRectangle);

            // Create a HTML element to be added in footer
            HtmlToPdfElement footerHtml = new HtmlToPdfElement(footerHtmlUrl);

            // Set the HTML element to fit the container height
            footerHtml.FitHeight = true;

            // Add HTML element to footer
            pdfDocument.Footer.AddElement(footerHtml);

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
                pdfDocument.Footer.AddElement(footerText);
            }

            if (drawFooterLine)
            {
                float footerWidth = pdfDocument.Footer.Width;

                // Create a line element for the top of the footer
                LineElement footerLine = new LineElement(0, 0, footerWidth, 0);

                // Set line color
                footerLine.ForeColor = Color.Gray;

                // Add line element to the bottom of the footer
                pdfDocument.Footer.AddElement(footerLine);
            }
        }
    }
}