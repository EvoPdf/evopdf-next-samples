using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator.PDF_Actions
{
    public class PDF_Creator_Execute_JavaScript_Open_ActionController : Controller
    {
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

            try
            {
                // Create a HTML to PDF element to add to document
                HtmlToPdfElement htmlToPdfElement = new HtmlToPdfElement(collection["urlTextBox"]);

                // Optionally set a delay before conversion to allow asynchonous scripts to finish
                htmlToPdfElement.ConversionDelay = 2;

                // Add the HTML to PDF element to document
                pdfPage.AddElement(htmlToPdfElement);

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
                pdfDocument.OpenAction.Action = new PdfActionJavaScript(javaScript);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Execute_Acrobat_JavaScript.pdf";

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