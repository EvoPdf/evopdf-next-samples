using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator
{
    public class PDF_Creator_Headers_and_FootersController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public PDF_Creator_Headers_and_FootersController(IWebHostEnvironment hostingEnvironment)
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

        public ActionResult PDF_Creator_HTML_in_Header_Footer()
        {
            return View();
        }
        
        public ActionResult PDF_Creator_Page_Numbers_in_HTML()
        {
            SetCurrentViewData();

            return View();
        }

        public ActionResult PDF_Creator_Header_Footer_Change()
        {
            return View();
        }

        public ActionResult PDF_Creator_Header_Footer_In_External_PDF()
        {
            SetCurrentViewData();

            return View();
        }

        public ActionResult PDF_Creator_Header_Footer_Auto_Resize()
        {
            return View();
        }
    }
}