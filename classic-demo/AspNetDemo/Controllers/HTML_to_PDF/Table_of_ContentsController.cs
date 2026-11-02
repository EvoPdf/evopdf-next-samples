using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class Table_of_ContentsController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public Table_of_ContentsController(IWebHostEnvironment hostingEnvironment)
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

        public ActionResult Auto_Create_Table_of_Contents()
        {
            return View();
        }

        public ActionResult Define_in_HTML_Table_of_Contents_Items()
        {
            SetCurrentViewData();

            return View();
        }

        public ActionResult Merge_Table_of_Contents()
        {
            return View();
        }

        public ActionResult Insert_PDF_Pages_Before_Table_of_Contents()
        {
            return View();
        }
    }
}