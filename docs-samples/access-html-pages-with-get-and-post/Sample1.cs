// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/access-html-pages-with-get-and-post.htm
// Documentation page: Access a HTML Page Using GET and POST HTTP Methods

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models; 
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class GET_and_POST_HTTP_MethodsController : Controller
    {
        public IActionResult Index()
        {
            var model = new GET_and_POST_HTTP_Methods_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(GET_and_POST_HTTP_Methods_ViewModel model)
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

            // The POST field names and values are automatically form-urlencoded by the converter
            // Set the EncodeHttpPostFields property to false if they are already encoded by the caller
            // The parameters transmitted in query string when using the GET method must be URL-encoded by the caller

            string param1Name = !string.IsNullOrEmpty(model.Param1Name) ? model.Param1Name : "param1";
            string param1Value = !string.IsNullOrEmpty(model.Param1Value) ? model.Param1Value : "Value1";

            string param2Name = !string.IsNullOrEmpty(model.Param2Name) ? model.Param2Name : "param2";
            string param2Value = !string.IsNullOrEmpty(model.Param2Value) ? model.Param2Value : "Value2";

            string param3Name = !string.IsNullOrEmpty(model.Param3Name) ? model.Param3Name : "param3";
            string param3Value = !string.IsNullOrEmpty(model.Param3Value) ? model.Param3Value : "Value3";

            string param4Name = !string.IsNullOrEmpty(model.Param4Name) ? model.Param4Name : "param4";
            string param4Value = !string.IsNullOrEmpty(model.Param4Value) ? model.Param4Value : "Value4";

            string param5Name = !string.IsNullOrEmpty(model.Param5Name) ? model.Param5Name : "param5";
            string param5Value = !string.IsNullOrEmpty(model.Param5Value) ? model.Param5Value : "Value5";

            string urlToConvert = model.Url;

            if (model.HttpMethod == "Post")
            {
                htmlToPdfConverter.HttpPostFields.Add(param1Name, param1Value);
                htmlToPdfConverter.HttpPostFields.Add(param2Name, param2Value);
                htmlToPdfConverter.HttpPostFields.Add(param3Name, param3Value);
                htmlToPdfConverter.HttpPostFields.Add(param4Name, param4Value);
                htmlToPdfConverter.HttpPostFields.Add(param5Name, param5Value);
            }
            else
            {
                Uri getMethodUri = new Uri(model.Url);

                string query = (getMethodUri.Query.Length > 0 ? "&" : "?") + String.Format("{0}={1}", param1Name, param1Value);
                query += String.Format("&{0}={1}", param2Name, param2Value);
                query += String.Format("&{0}={1}", param3Name, param3Value);
                query += String.Format("&{0}={1}", param4Name, param4Value);
                query += String.Format("&{0}={1}", param5Name, param5Value);

                urlToConvert = model.Url + query;
            }

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(urlToConvert);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "GET_and_POST.pdf";

            return fileResult;
        }
    }
}
