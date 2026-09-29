using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Convert_Internal_Links_to_PDFController : Controller
    {
        // GET: Convert_Internal_Links_to_PDF
        public ActionResult Index()
        {
            var model = new Convert_Internal_Links_to_PDF_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Convert_Internal_Links_to_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(model.Url);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Convert_Internal_Links_to_PDF.pdf";

            return fileResult;
        }
    }
}
