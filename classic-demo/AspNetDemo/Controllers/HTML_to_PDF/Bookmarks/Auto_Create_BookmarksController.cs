using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.Bookmarks
{
    public class Auto_Create_BookmarksController : Controller
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

            // Auto Create a hierarchy of bookmarks from H1 to H6 tags found in HTML
            if (collection["autoBookmarksCheckBox"].Count > 0)
            {
                // Enable the creation of a hierarchy of bookmarks from H1 to H6 tags
                htmlToPdfConverter.PdfBookmarkOptions.AutoBookmarksEnabled = true;

                // Display the bookmarks panel in PDF viewer when the generated PDF is opened
                htmlToPdfConverter.PdfViewerPreferences.PageMode = ViewerPageMode.UseOutlines;
            }

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(collection["urlTextBox"]);
            
            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Auto_Create_Hierarchical_Bookmarks.pdf";

            return fileResult;
        }
    }
}