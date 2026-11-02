using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using System.Drawing;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class Merge_Multiple_HTMLController : Controller
    {
        // GET: Merge_Multiple_HTML
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(IFormCollection collection)
        {
            // Create the PDF document where to add the HTML documents
            Document pdfDocument = new Document();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            pdfDocument.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Create a PDF page where to add the first HTML
            PdfPage firstPdfPage = pdfDocument.AddPage();

            try
            {
                // Create the first HTML to PDF element
                HtmlToPdfElement firstHtml = new HtmlToPdfElement(0, 0, collection["firstUrlTextBox"]);

                // Optionally set a delay before conversion to allow asynchonous scripts to finish
                firstHtml.ConversionDelay = 2;

                // Add the first HTML to PDF document
                AddElementResult firstAddResult = firstPdfPage.AddElement(firstHtml);

                PdfPage secondPdfPage = null;
                PointF secondHtmlLocation = Point.Empty;

                if (collection["startNewPageCheckBox"].Count > 0)
                {
                    // Create a PDF page where to add the second HTML
                    secondPdfPage = pdfDocument.AddPage();
                    secondHtmlLocation = PointF.Empty;
                }
                else
                {
                    // Add the second HTML on the PDF page where the first HTML ended
                    secondPdfPage = firstAddResult.EndPdfPage;
                    secondHtmlLocation = new PointF(firstAddResult.EndPageBounds.Left, firstAddResult.EndPageBounds.Bottom);
                }

                // Create the second HTML to PDF element
                HtmlToPdfElement secondHtml = new HtmlToPdfElement(secondHtmlLocation.X, secondHtmlLocation.Y, collection["secondUrlTextBox"]);

                // Optionally set a delay before conversion to allow asynchonous scripts to finish
                secondHtml.ConversionDelay = 2;

                // Add the second HTML to PDF document
                secondPdfPage.AddElement(secondHtml);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();

                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Merge_Multipe_HTML.pdf";

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