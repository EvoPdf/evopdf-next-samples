using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.IO;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class Convert_Page_in_Same_SessionController : Controller
    {
        private ICompositeViewEngine m_viewEngine;

        public Convert_Page_in_Same_SessionController(ICompositeViewEngine viewEngine)
        {
            m_viewEngine = viewEngine;
        }

        // GET: Convert_Page_in_Same_Session
        public ActionResult Index()
        {
            ViewData.Add("firstName", "John");
            ViewData.Add("lastName", "Smith");
            ViewData.Add("gender", "maleRadioButton");
            ViewData.Add("haveCar", "true");
            ViewData.Add("carType", "Volvo");
            ViewData.Add("comments", "My comments\r\nLine 1\r\nLine 2");

            return View();
        }

        public ActionResult Display_Session_Variables()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConvertPageInSameSessionToPdf(IFormCollection collection)
        {
            ViewDataDictionary viewData = new ViewDataDictionary(ViewData);
            viewData.Clear();

            // transmit the posted data to view
            viewData.Add("firstName", collection["firstNameTextBox"]);
            viewData.Add("lastName", collection["lastNameTextBox"]);
            viewData.Add("gender", collection["gender"]);
            viewData.Add("haveCar", collection["haveCarCheckBox"]);
            viewData.Add("carType", collection["carTypeDropDownList"]);
            viewData.Add("comments", collection["commentsTextBox"]);

            // The string writer where to render the HTML code of the view
            StringWriter stringWriter = new StringWriter();

            // Render the Index view in a HTML string
            ViewEngineResult viewResult = m_viewEngine.FindView(ControllerContext, "Display_Session_Variables", false);
            ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    viewData,
                    TempData,
                    stringWriter,
                    new HtmlHelperOptions()
                    );
            Task renderTask = viewResult.View.RenderAsync(viewContext);
            renderTask.Wait();

            // Get the view HTML string
            string htmlToConvert = stringWriter.ToString();

            // Get the base URL

            HttpRequest request = this.ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            String currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            String baseUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Convert_Page_in_Same_Session/ConvertPageInSameSessionToPdf".Length);

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            htmlToPdfConverter.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
            htmlToPdfConverter.ConversionDelay = 2;

            // Convert the HTML string to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlToConvert, baseUrl);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Convert_Page_in_Same_Session.pdf";

            return fileResult;
        }
    }
}