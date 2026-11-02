using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.Triggering_Modes
{
    public class Conversion_Triggering_ModesController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public Conversion_Triggering_ModesController(IWebHostEnvironment hostingEnvironment)
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

        // GET: Conversion_Triggering_Modes
        public ActionResult Index()
        {
            SetCurrentViewData();

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

            // Set the conversion triggering mode
            if (collection["TriggeringMode"] == "autoRadioButton")
            {
                // Set Auto triggering mode
                htmlToPdfConverter.TriggeringMode = TriggeringMode.Auto;
            }
            else if (collection["TriggeringMode"] == "delayedRadioButton")
            {
                // Set delayed triggering moe
                htmlToPdfConverter.ConversionDelay = int.Parse(collection["conversionDelayTextBox"]);
            }
            else if (collection["TriggeringMode"] == "manualRadioButton")
            {
                // Set manual triggering mode
                // The conversion starts when the evoPdfConverter.startConversion() is called 
                // in JavaScript code of the converted HTML page
                htmlToPdfConverter.TriggeringMode = TriggeringMode.Manual;
            }

            byte[] outPdfBuffer = null;

            if (collection["HtmlPageSource"] == "convertHtmlRadioButton")
            {
                string htmlWithForm = collection["htmlStringTextBox"];
                string baseUrl = collection["baseUrlTextBox"];

                // Convert the HTML string with page-break-inside:avoid styles to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlWithForm, baseUrl);
            }
            else
            {
                string url = collection["urlTextBox"];

                // Convert the HTML page with page-break-inside:avoid styles to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
            }
            
            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Conversion_Triggering_Modes.pdf";

            return fileResult;
        }
    }
}