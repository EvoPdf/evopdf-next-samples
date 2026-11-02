using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class HTTP_Headers_and_CookiesController : Controller
    {
        public ActionResult Add_HTTP_Headers_to_Request()
        {
            return View();
        }

        public ActionResult Add_Cookies_to_Request()
        {
            return View();
        }
    }
}