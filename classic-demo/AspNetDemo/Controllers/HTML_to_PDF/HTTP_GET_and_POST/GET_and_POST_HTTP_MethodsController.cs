using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class GET_and_POST_HTTP_MethodsController : Controller
    {
        // GET: GET_and_POST_HTTP_Methods
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

            string param1Name = collection["param1NameTextBox"][0].Length > 0 ? collection["param1NameTextBox"][0] : "param1";
            string param1Value = collection["param1ValueTextBox"][0].Length > 0 ? collection["param1ValueTextBox"][0] : "Value 1";

            string param2Name = collection["param2NameTextBox"][0].Length > 0 ? collection["param2NameTextBox"][0] : "param2";
            string param2Value = collection["param2ValueTextBox"][0].Length > 0 ? collection["param2ValueTextBox"][0] : "Value 2";

            string param3Name = collection["param3NameTextBox"][0].Length > 0 ? collection["param3NameTextBox"][0] : "param3";
            string param3Value = collection["param3ValueTextBox"][0].Length > 0 ? collection["param3ValueTextBox"][0] : "Value 3";

            string param4Name = collection["param4NameTextBox"][0].Length > 0 ? collection["param4NameTextBox"][0] : "param4";
            string param4Value = collection["param4ValueTextBox"][0].Length > 0 ? collection["param4ValueTextBox"][0] : "Value 4";

            string param5Name = collection["param5NameTextBox"][0].Length > 0 ? collection["param5NameTextBox"][0] : "param5";
            string param5Value = collection["param5ValueTextBox"][0].Length > 0 ? collection["param5ValueTextBox"][0] : "Value 5";

            string urlToConvert = collection["urlTextBox"];

            if (collection["HttpMethod"] == "postMethodRadioButton")
                {
                htmlToPdfConverter.HttpPostFields.Add(param1Name, param1Value);
                htmlToPdfConverter.HttpPostFields.Add(param2Name, param2Value);
                htmlToPdfConverter.HttpPostFields.Add(param3Name, param3Value);
                htmlToPdfConverter.HttpPostFields.Add(param4Name, param4Value);
                htmlToPdfConverter.HttpPostFields.Add(param5Name, param5Value);
            }
            else
            {
                Uri getMethodUri = new Uri(collection["urlTextBox"]);

                string query = getMethodUri.Query.Length > 0 ? "&" : "?" + String.Format("{0}={1}", param1Name, param1Value);
                query += String.Format("&{0}={1}", param2Name, param2Value);
                query += String.Format("&{0}={1}", param3Name, param3Value);
                query += String.Format("&{0}={1}", param4Name, param4Value);
                query += String.Format("&{0}={1}", param5Name, param5Value);

                urlToConvert = collection["urlTextBox"] + query;
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