using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class Images_QualityController : Controller
    {
        public ActionResult Replace_with_Higher_Quality_Images()
        {
            return View();
        }
    }
}