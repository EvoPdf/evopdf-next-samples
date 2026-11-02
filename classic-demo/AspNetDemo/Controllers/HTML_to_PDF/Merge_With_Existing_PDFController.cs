using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class Merge_With_Existing_PDFController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public Merge_With_Existing_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: Merge_With_Existing_PDF
        public ActionResult Index()
        {
            return View();
        }

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

            // Set the PDF file to be inserted before conversion result
            string pdfFileBefore = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/PDF_Files/Merge_Before_Conversion.pdf";
            htmlToPdfConverter.PdfDocumentOptions.AddStartDocument(pdfFileBefore);

            // Set the PDF file to be added after conversion result
            string pdfFileAfter = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/PDF_Files/Merge_After_Conversion.pdf";
            htmlToPdfConverter.PdfDocumentOptions.AddEndDocument(pdfFileAfter);

            // The URL to convert
            string url = collection["urlTextBox"];

            // Convert the HTML page to a PDF document and add the external PDF documents
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
                        
            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Merge_HTML_with_Existing_PDF.pdf";

            return fileResult;
        }
    }
}