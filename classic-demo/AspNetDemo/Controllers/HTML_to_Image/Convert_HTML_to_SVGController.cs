using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.HTML_to_Image
{
    public class Convert_HTML_to_SVGController : Controller
    {
        [HttpPost]
        public ActionResult ConvertHtmlToSvg(IFormCollection collection)
        {
            // Create a HTML to SVG converter object with default settings
            HtmlToSvgConverter htmlToSvgConverter = new HtmlToSvgConverter();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            htmlToSvgConverter.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Set HTML Viewer width in pixels which is the equivalent in converter of the browser window width
            htmlToSvgConverter.HtmlViewerWidth = int.Parse(collection["htmlViewerWidthTextBox"]);

            // Set HTML viewer height in pixels to convert the top part of a HTML page 
            // Leave it not set to convert the entire HTML
            if (collection["htmlViewerHeightTextBox"][0].Length > 0)
                htmlToSvgConverter.HtmlViewerHeight = int.Parse(collection["htmlViewerHeightTextBox"]);

            // Set the maximum time in seconds to wait for HTML page to be loaded 
            // Leave it not set for a default 60 seconds maximum wait time
            htmlToSvgConverter.NavigationTimeout = int.Parse(collection["navigationTimeoutTextBox"]);

            // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
            if (collection["conversionDelayTextBox"][0].Length > 0)
                htmlToSvgConverter.ConversionDelay = int.Parse(collection["conversionDelayTextBox"]);

            // The buffer to receive the generated SVG document
            byte[] outSvgBuffer = null;

            if (collection["HtmlPageSource"] == "convertUrlRadioButton")
            {
                string url = collection["urlTextBox"];

                // Convert the HTML page given by an URL to a SVG document in a memory buffer
                outSvgBuffer = htmlToSvgConverter.ConvertUrl(url);
            }
            else
            {
                string htmlString = collection["htmlStringTextBox"];
                string baseUrl = collection["baseUrlTextBox"];

                // Convert a HTML string with a base URL to a SVG document in a memory buffer
                outSvgBuffer = htmlToSvgConverter.ConvertHtml(htmlString, baseUrl);
            }
            
            // Send the SVG file to browser
            FileResult fileResult = new FileContentResult(outSvgBuffer, "image/svg+xml");
            fileResult.FileDownloadName = "HTML_to_SVG.svg";

            return fileResult;
        }
    }
}