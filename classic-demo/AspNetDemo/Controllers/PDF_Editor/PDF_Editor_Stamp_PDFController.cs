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

namespace EvoHtmlToPdfDemo.Controllers.PDF_Editor
{
    public class PDF_Editor_Stamp_PDFController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public PDF_Editor_Stamp_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        private void SetCurrentViewData()
        {
            ViewData["ContentRootPath"] = m_hostingEnvironment.ContentRootPath + "/wwwroot";

            HttpRequest request = this.ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            ViewData["CurrentPageUrl"] = uriBuilder.Uri.AbsoluteUri;
        }

        // GET: PDF_Editor_Stamp_PDF
        public ActionResult Index()
        {
            SetCurrentViewData();

            return View();
        }

        [HttpPost]
        public ActionResult StampPdf(IFormCollection collection)
        {
            Document pdfDocument = null;
            try
            {
                // Load the PDF document to stamp
                string pdfFileToStampPath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/PDF_Files/PDF_Document.pdf";
                pdfDocument = new Document(pdfFileToStampPath);

                // Set license key received after purchase to use the converter in licensed mode
                // Leave it not set to use the converter in demo mode
                pdfDocument.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

                // Get the stamp width and height
                float stampWidth = float.Parse(collection["stampWidthTextBox"]);
                float stampHeight = float.Parse(collection["stampHeightTextBox"]);

                // Center the stamp at the top of PDF page
                float stampXLocation = (pdfDocument.Pages[0].ClientRectangle.Width - stampWidth) / 2;
                float stampYLocation = 0;

                RectangleF stampRectangle = new RectangleF(stampXLocation, stampYLocation, stampWidth, stampHeight);

                // Create the stamp template to be repeated in each PDF page
                Template stampTemplate = pdfDocument.AddTemplate(stampRectangle);

                // Create the HTML element to add in stamp template
                HtmlToPdfElement stampHtmlElement = new HtmlToPdfElement(collection["htmlStringTextBox"], collection["baseUrlTextBox"]);

                // Set the HTML viewer width for the HTML added in stamp
                stampHtmlElement.HtmlViewerWidth = 600;
                // Fit the HTML content in stamp template
                stampHtmlElement.FitWidth = true;
                stampHtmlElement.FitHeight = true;

                // Add HTML to stamp template
                stampTemplate.AddElement(stampHtmlElement);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Stamp_PDF.pdf";

                return fileResult;
            }
            finally
            {
                // Close the document to stamp
                if (pdfDocument != null)
                    pdfDocument.Close();
            }
        }
    }
}