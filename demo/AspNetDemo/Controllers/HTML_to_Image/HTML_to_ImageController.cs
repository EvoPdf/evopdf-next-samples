using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_Image;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_Image
{
    public class HTML_to_ImageController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public HTML_to_ImageController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Convert_HTML_to_Image()
        {
            var model = SetViewModel();
            return View(model);
        }

        public ActionResult Select_HTML_Elements_to_Convert_to_Image()
        {
            var model = SetSelectHtmlElementsToConvertViewModel();
            return View(model);
        }

        public ActionResult Select_HTML_Elements_to_Exclude_from_Image()
        {
            var model = SetSelectHtmlElementsToExcludeViewModel();
            return View(model);
        }

        private Convert_HTML_to_Image_ViewModel SetViewModel()
        {
            var model = new Convert_HTML_to_Image_ViewModel();
            return model;
        }

        private Select_HTML_Elements_to_Convert_to_Image_ViewModel SetSelectHtmlElementsToConvertViewModel()
        {
            var model = new Select_HTML_Elements_to_Convert_to_Image_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "HTML_to_Image/Select_HTML_Elements_to_Convert_to_Image".Length);

            model.HtmlString = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Partially_Converterted.html"));
            model.BaseUrl = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.Url = rootUrl + "DemoAppFiles/Input/HTML_Files/Partially_Converterted.html";

            return model;
        }

        private Select_HTML_Elements_to_Exclude_from_Image_ViewModel SetSelectHtmlElementsToExcludeViewModel()
        {
            var model = new Select_HTML_Elements_to_Exclude_from_Image_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "HTML_to_Image/Select_HTML_Elements_to_Exclude_from_Image".Length);

            model.HtmlString = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Excluded_Elements.html"));
            model.BaseUrl = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.Url = rootUrl + "DemoAppFiles/Input/HTML_Files/Excluded_Elements.html";

            return model;
        }
    }
}