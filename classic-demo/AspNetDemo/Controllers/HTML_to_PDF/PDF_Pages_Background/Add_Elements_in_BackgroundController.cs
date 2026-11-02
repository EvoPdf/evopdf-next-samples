using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.PDF_Pages_Background
{
    public class Add_Elements_in_BackgroundController : Controller
    {
        IFormCollection formCollection;

        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public Add_Elements_in_BackgroundController(IWebHostEnvironment hostingEnvironment)
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

            // Set a handler for BeforeRenderPdfPageEvent where to set the background image in each PDF page before main content is rendered
            htmlToPdfConverter.BeforeRenderPdfPageEvent += new BeforeRenderPdfPageDelegate(htmlToPdfConverter_BeforeRenderPdfPageEvent);

            try
            {
                // The buffer to receive the generated PDF document
                byte[] outPdfBuffer = null;

                if (collection["HtmlPageSource"] == "convertUrlRadioButton")
                {
                    string url = collection["urlTextBox"];

                    // Convert the HTML page given by an URL to a PDF document in a memory buffer
                    outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
                }
                else
                {
                    string htmlString = collection["htmlStringTextBox"];
                    string baseUrl = collection["baseUrlTextBox"];

                    // Convert a HTML string with a base URL to a PDF document in a memory buffer
                    outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlString, baseUrl);
                }

                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Add_Elements_in_Background.pdf";

                return fileResult;
            }
            finally
            {
                // Uninstall the handler
                htmlToPdfConverter.BeforeRenderPdfPageEvent -= new BeforeRenderPdfPageDelegate(htmlToPdfConverter_BeforeRenderPdfPageEvent);
            }            
        }

        /// <summary>
        /// The BeforeRenderPdfPageEvent event handler where a background image is set in each PDF page
        /// before the main content is rendered
        /// </summary>
        /// <param name="eventParams">The event parameter containing the PDF page being rendered</param>
        void htmlToPdfConverter_BeforeRenderPdfPageEvent(BeforeRenderPdfPageParams eventParams)
        {
            if (formCollection["addBackgroundImageCheckBox"][0] == null)
                return;

            // Get the PDF page being rendered
            PdfPage pdfPage = eventParams.Page;

            // Get the PDF page drawable area width and height
            float pdfPageWidth = pdfPage.ClientRectangle.Width;
            float pdfPageHeight = pdfPage.ClientRectangle.Height;

            // The image to be added as background
            string backgroundImagePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Images/background.jpg";

            // The image element to add in background
            ImageElement backgroundImageElement = new ImageElement(0, 0, pdfPageWidth, pdfPageHeight, backgroundImagePath);
            backgroundImageElement.KeepAspectRatio = true;
            backgroundImageElement.EnlargeEnabled = true;

            // Add the background image element to PDF page before the main content is rendered
            pdfPage.AddElement(backgroundImageElement);
        }
    }
}