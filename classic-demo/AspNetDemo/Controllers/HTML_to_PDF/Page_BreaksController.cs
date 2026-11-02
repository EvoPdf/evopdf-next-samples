using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class Page_BreaksController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public Page_BreaksController(IWebHostEnvironment hostingEnvironment)
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

        public ActionResult Insert_Page_Breaks_Before_After_Using_CSS()
        {
            SetCurrentViewData();

            return View();
        }

        public ActionResult Avoid_Page_Breaks_Inside_HTML_Elements_Using_CSS()
        {
            SetCurrentViewData();

            return View();
        }

        public ActionResult Insert_Page_Breaks_Before_After_Using_API()
        {
            SetCurrentViewData();

            return View();
        }

        public ActionResult Avoid_Page_Breaks_Inside_HTML_Elements_Using_API()
        {
            SetCurrentViewData();

            return View();
        }

        public ActionResult Avoid_Page_Breaks_Inside_Images_Using_API()
        {
            return View();
        }
    }
}