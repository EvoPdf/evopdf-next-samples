using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Add_Cookies_to_RequestController : Controller
    {
        public IActionResult Index()
        {
            var model = new Add_Cookies_to_Request_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Add_Cookies_to_Request_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Add custom HTTP cookies
            // The caller must provide a valid cookie name and RFC 6265-encoded value

            if (!string.IsNullOrEmpty(model.Cookie1Name) && !string.IsNullOrEmpty(model.Cookie1Value))
                htmlToPdfConverter.HttpRequestCookies.Add(model.Cookie1Name, model.Cookie1Value);

            if (!string.IsNullOrEmpty(model.Cookie2Name) && !string.IsNullOrEmpty(model.Cookie2Value))
                htmlToPdfConverter.HttpRequestCookies.Add(model.Cookie2Name, model.Cookie2Value);

            if (!string.IsNullOrEmpty(model.Cookie3Name) && !string.IsNullOrEmpty(model.Cookie3Value))
                htmlToPdfConverter.HttpRequestCookies.Add(model.Cookie3Name, model.Cookie3Value);

            if (!string.IsNullOrEmpty(model.Cookie4Name) && !string.IsNullOrEmpty(model.Cookie4Value))
                htmlToPdfConverter.HttpRequestCookies.Add(model.Cookie4Name, model.Cookie4Value);

            if (!string.IsNullOrEmpty(model.Cookie5Name) && !string.IsNullOrEmpty(model.Cookie5Value))
                htmlToPdfConverter.HttpRequestCookies.Add(model.Cookie5Name, model.Cookie5Value);

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(model.Url);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "HTTP_Cookies.pdf";

            return fileResult;
        }
    }
}
