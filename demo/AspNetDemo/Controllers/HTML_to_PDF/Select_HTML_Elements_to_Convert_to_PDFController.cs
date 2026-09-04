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
    public class Select_HTML_Elements_to_Convert_to_PDFController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Select_HTML_Elements_to_Convert_to_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Index()
        {
            var model = SetCurrentViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Select_HTML_Elements_to_Convert_to_PDF_ViewModel model)
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

            bool enableElementsSelector = model.EnableElementsSelector;
            if (enableElementsSelector)
            {
                // The CSS selector used to identify the elements to include in the PDF
                htmlToPdfConverter.ConvertedElementsSelector = model.ConvertedElementsSelector;

                // Specify whether elements that are not matched by ConvertedElementsSelector
                // should be completely removed from the layout rather than just hidden
                htmlToPdfConverter.RemoveUnselectedElements = model.RemoveUnselectedElements;

                // Automatically resizes the PDF page height to match the selected HTML content height
                htmlToPdfConverter.PdfDocumentOptions.AutoResizePdfPageHeight = model.AutoResizePdfPageHeight;
                if (htmlToPdfConverter.PdfDocumentOptions.AutoResizePdfPageHeight)
                    htmlToPdfConverter.HtmlViewerHeight = 1;
            }

            byte[] outPdfBuffer = null;

            if (model.HtmlPageSource == "Html")
            {
                string htmlWithForm = model.HtmlString;
                string baseUrl = model.BaseUrl;
                // Convert a HTML string to a PDF document
                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlWithForm, baseUrl);
            }
            else
            {
                string url = model.Url;
                // Convert the HTML page to a PDF document
                outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
            }

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Selected_HTML_Elements.pdf";

            return fileResult;
        }

        private Select_HTML_Elements_to_Convert_to_PDF_ViewModel SetCurrentViewModel()
        {
            var model = new Select_HTML_Elements_to_Convert_to_PDF_ViewModel();

            var contentRootPath = m_hostingEnvironment.ContentRootPath + "/wwwroot";

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Select_HTML_Elements_to_Convert_to_PDF".Length);

            model.HtmlString = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Partially_Converterted.html"));
            model.BaseUrl = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.Url = rootUrl + "DemoAppFiles/Input/HTML_Files/Partially_Converterted.html";

            return model;
        }
    }
}
