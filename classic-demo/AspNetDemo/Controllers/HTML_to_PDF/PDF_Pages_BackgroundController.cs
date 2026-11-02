using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class PDF_Pages_BackgroundController : Controller
    {
        public ActionResult Add_Elements_in_Background()
        {
            return View();
        }

        public ActionResult Add_Elements_Over_Main_Content()
        {
            return View();
        }
    }
}