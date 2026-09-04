// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-http-headers-to-html-page-request.htm
// Documentation page: Add HTTP Headers to HTML Page Request

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Add_HTTP_Headers_to_RequestController : Controller
    {
        public IActionResult Index()
        {
            var model = new Add_HTTP_Headers_to_Request_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Add_HTTP_Headers_to_Request_ViewModel model)
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

            // Set the persistent HTTP Headers option to control the inclusion of headers 
            // for each requested resource in the HTML document
            htmlToPdfConverter.PersistentHttpRequestHeaders = model.PersistentHttpHeaders;

            // Add custom HTTP headers
            // The caller must provide a valid HTTP header name and printable ASCII value

            if (!string.IsNullOrEmpty(model.Header1Name) && !string.IsNullOrEmpty(model.Header1Value))
                htmlToPdfConverter.HttpRequestHeaders.Add(model.Header1Name, model.Header1Value);

            if (!string.IsNullOrEmpty(model.Header2Name) && !string.IsNullOrEmpty(model.Header2Value))
                htmlToPdfConverter.HttpRequestHeaders.Add(model.Header2Name, model.Header2Value);

            if (!string.IsNullOrEmpty(model.Header3Name) && !string.IsNullOrEmpty(model.Header3Value))
                htmlToPdfConverter.HttpRequestHeaders.Add(model.Header3Name, model.Header3Value);

            if (!string.IsNullOrEmpty(model.Header4Name) && !string.IsNullOrEmpty(model.Header4Value))
                htmlToPdfConverter.HttpRequestHeaders.Add(model.Header4Name, model.Header4Value);

            if (!string.IsNullOrEmpty(model.Header5Name) && !string.IsNullOrEmpty(model.Header5Value))
                htmlToPdfConverter.HttpRequestHeaders.Add(model.Header5Name, model.Header5Value);

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(model.Url);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "HTTP_Headers.pdf";

            return fileResult;
        }
    }
}
