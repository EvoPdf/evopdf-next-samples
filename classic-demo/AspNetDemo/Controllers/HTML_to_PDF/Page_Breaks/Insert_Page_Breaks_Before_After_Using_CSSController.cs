using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.Page_Breaks
{
    public class Insert_Page_Breaks_Before_After_Using_CSSController : Controller
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

            byte[] outPdfBuffer = null;

            if (collection["HtmlPageSource"] == "convertHtmlRadioButton")
            {
                string htmlWithForm = collection["htmlStringTextBox"];
                string baseUrl = collection["baseUrlTextBox"];

                // Convert the HTML string with page-break-before:always and page-break-after:always styles to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlWithForm, baseUrl);
            }
            else
            {
                string url = collection["urlTextBox"];

                // Convert the HTML page with page-break-before:always and page-break-after:always styles to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
            }
            
            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Insert_Page_Breaks_Before_After_HTML_Elements_Using_CSS.pdf";

            return fileResult;
        }
    }
}