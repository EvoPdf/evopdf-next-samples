// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/auto-create-hierarchical-bookmarks.htm
// Documentation page: Auto Create Hierarchical Bookmarks

using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Auto_Create_BookmarksController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Auto_Create_BookmarksController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Auto_Create_Bookmarks_ViewModel model)
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

            // Auto Create a hierarchy of bookmarks from H1 to H6 tags found in HTML
            if (model.GenerateDocumentOutline)
            {
                // Enable the creation of a hierarchy of bookmarks from H1 to H6 tags
                htmlToPdfConverter.PdfDocumentOptions.GenerateDocumentOutline = model.GenerateDocumentOutline;

                htmlToPdfConverter.ConversionDelay = 2;

                // Optionally, enable the outline mode to utilize browser capabilities. By default, a custom algorithm is used
                htmlToPdfConverter.PdfDocumentOptions.UseBrowserOutlineMode = model.UseBrowserOutlineMode;

                // Display the bookmarks panel in PDF viewer when the generated PDF is opened
                htmlToPdfConverter.PdfViewerPreferences.PageMode = ViewerPageMode.UseOutlines;
            }

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = null;

            if (model.HtmlPageSource == "Url")
            {
                string url = model.Url;

                // Convert the HTML page given by an URL to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
            }
            else
            {
                string htmlString = model.HtmlStringTextBox;
                string baseUrl = model.BaseUrlTextBox;

                // Convert a HTML string with a base URL to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlString, baseUrl);
            }

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Auto_Create_Hierarchical_Bookmarks.pdf";

            return fileResult;
        }

        private Auto_Create_Bookmarks_ViewModel SetViewModel()
        {
            var model = new Auto_Create_Bookmarks_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Auto_Create_Bookmarks".Length);

            model.HtmlStringTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Auto_Bookmarks.html"));
            model.BaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";

            return model;
        }
    }
}
