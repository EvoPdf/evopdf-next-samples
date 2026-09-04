// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/html-header-and-footer-in-browser-mode.htm
// Documentation page: Add HTML in Header and Footer Using Browser Mode

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
    public class HTML_in_Header_Footer_Browser_ModeController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public HTML_in_Header_Footer_Browser_ModeController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: HTML_in_Header_Footer
        public ActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(HTML_in_Header_Footer_Browser_Mode_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create an HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Enable the built-in HTML header and footer functionality
            htmlToPdfConverter.PdfDocumentOptions.EnableHeaderFooter = model.EnableHeaderFooter;

            if (htmlToPdfConverter.PdfDocumentOptions.EnableHeaderFooter)
            {
                if (model.HeaderEnabled)
                {
                    if (model.HeaderHeight.HasValue)
                        htmlToPdfConverter.PdfDocumentOptions.TopMargin = model.HeaderHeight.Value;

                    string headerTemplateHtml = model.HeaderTemplate;
                    htmlToPdfConverter.PdfDocumentOptions.HeaderTemplate = headerTemplateHtml;
                }

                if (model.FooterEnabled)
                {
                    if (model.FooterHeight.HasValue)
                        htmlToPdfConverter.PdfDocumentOptions.BottomMargin = model.FooterHeight.Value;

                    string footerTemplateHtml = model.FooterTemplate;
                    htmlToPdfConverter.PdfDocumentOptions.FooterTemplate = footerTemplateHtml;
                }
            }

            // Convert the HTML page to a PDF document and store it in memory
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(model.Url);

            // Send the PDF file to the browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "HTML_in_Header_Footer.pdf";

            return fileResult;
        }

        private HTML_in_Header_Footer_Browser_Mode_ViewModel SetViewModel()
        {
            var model = new HTML_in_Header_Footer_Browser_Mode_ViewModel();

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

            model.HeaderTemplate = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/HeaderTemplate.html"));
            model.FooterTemplate = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/FooterTemplate.html"));

            return model;
        }
    }
}
