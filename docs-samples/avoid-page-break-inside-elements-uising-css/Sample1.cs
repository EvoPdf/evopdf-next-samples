// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/avoid-page-break-inside-elements-uising-css.htm
// Documentation page: Avoid Page Breaks Inside HTML Elements Using CSS

using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Avoid_Page_Breaks_InsideController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Avoid_Page_Breaks_InsideController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: Avoid_Page_Breaks_Inside
        public ActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Avoid_Page_Breaks_Inside_ViewModel model)
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

            byte[] outPdfBuffer = null;

            if (model.HtmlPageSource == "Html")
            {
                string htmlWithForm = model.HtmlString;
                string baseUrl = model.BaseUrl;

                // Convert the HTML string with page-break-inside:avoid styles to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlWithForm, baseUrl);
            }
            else
            {
                string url = model.Url;

                // Convert the HTML page with page-break-inside:avoid styles to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
            }

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Avoid_Page_Breaks_Inside.pdf";

            return fileResult;
        }

        private Avoid_Page_Breaks_Inside_ViewModel SetViewModel()
        {
            var model = new Avoid_Page_Breaks_Inside_ViewModel();

            var contentRootPath = Path.Combine(m_hostingEnvironment.ContentRootPath, "wwwroot");

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Avoid_Page_Breaks_Inside".Length);

            model.HtmlString = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Page_Breaks_Inside_Avoid_Using_CSS.html"));
            model.BaseUrl = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.Url = rootUrl + "DemoAppFiles/Input/HTML_Files/Page_Breaks_Inside_Avoid_Using_CSS.html";

            return model;
        }
    }
}
