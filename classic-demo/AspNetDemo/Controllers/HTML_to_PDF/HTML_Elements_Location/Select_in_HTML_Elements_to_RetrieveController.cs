using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using System.Drawing;
using System.Text;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.HTML_Elements_Location
{
    public class Select_in_HTML_Elements_to_RetrieveController : Controller
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

            Document pdfDocument = null;
            try
            {
                // Convert HTML page or string with mapping attributes to a PDF document object 
                // The document can be further modified to highlight the selected elements
                if (collection["HtmlPageSource"] == "convertHtmlRadioButton")
                {
                    string htmlWithMappingAttributes = collection["htmlStringTextBox"];
                    string baseUrl = collection["baseUrlTextBox"];

                    // Convert a HTML string with mapping attributes to a PDF document object
                    pdfDocument = htmlToPdfConverter.ConvertHtmlToPdfDocumentObject(htmlWithMappingAttributes, baseUrl);
                }
                else
                {
                    string url = collection["urlTextBox"];

                    // Convert a HTML page with mapping attributes to a PDF document object
                    pdfDocument = htmlToPdfConverter.ConvertUrlToPdfDocumentObject(url);
                }

                // Display detailed information about the selected elements
                StringBuilder htmlElementInfoBuilder = new StringBuilder();
                foreach (HtmlElementMapping htmlElementInfo in htmlToPdfConverter.HtmlElementsMappingOptions.HtmlElementsMappingResult)
                {
                    // Get other information about HTML element
                    string htmlElementTagName = htmlElementInfo.HtmlElementTagName;
                    string htmlElementID = htmlElementInfo.HtmlElementId;
                    string htmlElementMappingID = htmlElementInfo.MappingId;
                    string htmlElementCssClasssName = htmlElementInfo.HtmlElementCssClassName;
                    string htmlElementHtmlCode = htmlElementInfo.HtmlElementOuterHtml;
                    string htmlElementInnerHtml = htmlElementInfo.HtmlElementInnerHtml;
                    string htmlElementText = htmlElementInfo.HtmlElementText;
                    System.Collections.Specialized.NameValueCollection htmlElementAttributes = htmlElementInfo.HtmlElementAttributes;
                    HtmlElementPdfRectangle[] htmlElementRectanglesInPdf = htmlElementInfo.PdfRectangles;

                    htmlElementInfoBuilder.AppendFormat("<br/>---------------------------------------- HTML Element Info ----------------------------------------<br/><br/>");
                    htmlElementInfoBuilder.AppendFormat("<b>Tag Name:</b> {0}<br/>", htmlElementTagName);
                    htmlElementInfoBuilder.AppendFormat("<b>Element ID:</b> {0}<br/>", htmlElementID);
                    htmlElementInfoBuilder.AppendFormat("<b>Mapping ID:</b> {0}<br/>", htmlElementMappingID);
                    htmlElementInfoBuilder.AppendFormat("<b>Text:</b> {0}<br/>", htmlElementText);

                    htmlElementInfoBuilder.AppendFormat("<b>Attributes:</b><br/>");
                    for (int i = 0; i < htmlElementAttributes.Count; i++)
                        htmlElementInfoBuilder.AppendFormat("&nbsp;&nbsp;&nbsp;{0} = \"{1}\"<br/>", htmlElementAttributes.GetKey(i), htmlElementAttributes.Get(i));


                    htmlElementInfoBuilder.AppendFormat("<b>Location in PDF:</b><br/>");
                    for (int i = 0; i < htmlElementRectanglesInPdf.Length; i++)
                    {
                        PdfPage pdfPage = htmlElementRectanglesInPdf[i].PdfPage;
                        int pdfPageIndex = htmlElementRectanglesInPdf[i].PageIndex;
                        RectangleF rectangleInPdfPage = htmlElementRectanglesInPdf[i].Rectangle;

                        htmlElementInfoBuilder.AppendFormat("&nbsp;&nbsp;&nbsp;PDF Page Index: {0}<br>", pdfPageIndex);
                        htmlElementInfoBuilder.AppendFormat("&nbsp;&nbsp;&nbsp;Rectangle: X = {0:N2} pt , Y = {1:N2} pt , W = {2:N2} pt , H = {3:N2} pt<br/>",
                                rectangleInPdfPage.X, rectangleInPdfPage.Y, rectangleInPdfPage.Width, rectangleInPdfPage.Height);
                    }

                    htmlElementInfoBuilder.AppendFormat("<br/>");
                }

                PdfPage lastPdfPage = htmlToPdfConverter.ConversionSummary.LastPdfPage;
                RectangleF lastPageRectangle = htmlToPdfConverter.ConversionSummary.LastPageRectangle;

                HtmlToPdfElement htmlElementInfoHtml = new HtmlToPdfElement(0, lastPageRectangle.Bottom + 1, htmlElementInfoBuilder.ToString(), null);
                lastPdfPage.AddElement(htmlElementInfoHtml);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();

                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Select_in_HTML_Elements_to_Retrieve.pdf";

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