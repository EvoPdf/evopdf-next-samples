using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using System.Drawing;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.PDF_Actions
{
    public class Execute_JavaScript_Button_ClickedController : Controller
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
                string htmlWithButton = collection["htmlStringTextBox"];
                string baseUrl = collection["baseUrlTextBox"];

                // Convert a HTML string with a button to a PDF document object
                pdfDocument = htmlToPdfConverter.ConvertHtmlToPdfDocumentObject(htmlWithButton, baseUrl);

                // Get the button location in PDF
                HtmlElementMapping buttonMapping = htmlToPdfConverter.HtmlElementsMappingOptions.HtmlElementsMappingResult.GetElementByMappingId("javascript_button");
                if (buttonMapping != null)
                {
                    PdfPage buttonPdfPage = buttonMapping.PdfRectangles[0].PdfPage;
                    RectangleF buttonRectangle = buttonMapping.PdfRectangles[0].Rectangle;

                    // The font used for buttons text in PDF document
                    PdfFont buttonTextFont = pdfDocument.AddFont(new Font("Times New Roman", 8, FontStyle.Regular, GraphicsUnit.Point));

                    // Create a PDF form button
                    PdfFormButton pdfButton = pdfDocument.Form.AddButton(buttonPdfPage, buttonRectangle, "Execute Acrobat JavaScript", buttonTextFont);

                    // Set JavaScript action to be executed when the button is clicked
                    string javaScript = null;
                    if (collection["JavaScriptAction"] == "alertMessageRadioButton")
                    {
                        // JavaScript to display an alert mesage 
                        javaScript = String.Format("app.alert(\"{0}\")", collection["alertMessageTextBox"]);
                    }
                    else if (collection["JavaScriptAction"] == "printDialogRadioButton")
                    {
                        // JavaScript to open the print dialog
                        javaScript = "print()";
                    }
                    else if (collection["JavaScriptAction"] == "zoomLevelRadioButton")
                    {
                        // JavaScript to set an initial zoom level 
                        javaScript = String.Format("zoom={0}", int.Parse(collection["zoomLevelTextBox"]));
                    }

                    // Set the JavaScript action
                    pdfButton.Action = new PdfActionJavaScript(javaScript);
                }

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();

                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Button_JavaScript_Actions.pdf";

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