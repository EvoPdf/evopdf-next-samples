// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/convert-current-html-page-to-pdf.htm
// Documentation page: Convert the Current HTML Page to PDF

using System;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Convert_Current_PageController : Controller
    {
        private ICompositeViewEngine m_viewEngine;

        public Convert_Current_PageController(ICompositeViewEngine viewEngine)
        {
            m_viewEngine = viewEngine;
        }

        // GET: Convert_Current_Page
        public ActionResult Index()
        {
            var model = new Convert_Current_Page_ViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertCurrentPageToPdf(Convert_Current_Page_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            ViewDataDictionary viewData = new ViewDataDictionary(ViewData);
            viewData.Clear();
            viewData.Model = model;

            // The string writer where to render the HTML code of the view
            StringWriter stringWriter = new StringWriter();

            // Render the Index view in a HTML string
            ViewEngineResult viewResult = m_viewEngine.FindView(ControllerContext, "Index", true);
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

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string baseUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Convert_Current_Page/ConvertCurrentPageToPdf".Length);

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Convert the HTML string to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlToConvert, baseUrl);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Convert_Current_Page.pdf";

            return fileResult;
        }
    }
}
