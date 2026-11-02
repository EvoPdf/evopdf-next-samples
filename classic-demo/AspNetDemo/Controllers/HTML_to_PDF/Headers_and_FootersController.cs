using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;
using System.Drawing;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class Headers_and_FootersController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public Headers_and_FootersController(IWebHostEnvironment hostingEnvironment)
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

        public ActionResult HTML_in_Header_Footer()
        {
            return View();
        }

        public ActionResult Page_Numbers_in_HTML()
        {
            SetCurrentViewData();

            return View();
        }

        public ActionResult Header_Footer_Change()
        {
            return View();
        }

        public ActionResult Header_Footer_In_External_PDF()
        {
            SetCurrentViewData();

            return View();
        }

        public ActionResult Header_Footer_on_Merged_HTML()
        {
            return View();
        }

        public ActionResult Header_Footer_Auto_Resize()
        {
            return View();
        }
    }
}