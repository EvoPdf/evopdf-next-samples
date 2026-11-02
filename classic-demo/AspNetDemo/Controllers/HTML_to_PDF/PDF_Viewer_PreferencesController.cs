using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class PDF_Viewer_PreferencesController : Controller
    {
        public ActionResult Set_Viewer_Preferences()
        {
            return View();
        }

        public ActionResult Set_Initial_Zoom_Level()
        {
            return View();
        }
    }
}