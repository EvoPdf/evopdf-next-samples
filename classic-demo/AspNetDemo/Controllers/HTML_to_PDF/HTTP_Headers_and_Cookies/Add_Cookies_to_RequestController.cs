using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.HTTP_Headers_and_Cookies
{
    public class Add_Cookies_to_RequestController : Controller
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

            // Add custom HTTP cookies

            if (collection["cookie1NameTextBox"][0].Length > 0 && collection["cookie1ValueTextBox"][0].Length > 0)
                htmlToPdfConverter.HttpRequestCookies.Add(collection["cookie1NameTextBox"], collection["cookie1ValueTextBox"]);

            if (collection["cookie2NameTextBox"][0].Length > 0 && collection["cookie2ValueTextBox"][0].Length > 0)
                htmlToPdfConverter.HttpRequestCookies.Add(collection["cookie2NameTextBox"], collection["cookie2ValueTextBox"]);

            if (collection["cookie3NameTextBox"][0].Length > 0 && collection["cookie3ValueTextBox"][0].Length > 0)
                htmlToPdfConverter.HttpRequestCookies.Add(collection["cookie3NameTextBox"], collection["cookie3ValueTextBox"]);

            if (collection["cookie4NameTextBox"][0].Length > 0 && collection["cookie4ValueTextBox"][0].Length > 0)
                htmlToPdfConverter.HttpRequestCookies.Add(collection["cookie4NameTextBox"], collection["cookie4ValueTextBox"]);

            if (collection["cookie5NameTextBox"][0].Length > 0 && collection["cookie5ValueTextBox"][0].Length > 0)
                htmlToPdfConverter.HttpRequestCookies.Add(collection["cookie5NameTextBox"], collection["cookie5ValueTextBox"]);

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(collection["urlTextBox"]);
            
            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "HTTP_Cookies.pdf";

            return fileResult;
        }
    }
}