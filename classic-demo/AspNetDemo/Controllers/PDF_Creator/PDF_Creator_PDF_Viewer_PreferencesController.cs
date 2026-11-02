using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator
{
    public class PDF_Creator_PDF_Viewer_PreferencesController : Controller
    {
        public ActionResult PDF_Creator_Set_Viewer_Preferences()
        {
            return View();
        }

        public ActionResult PDF_Creator_Set_Initial_Zoom_Level()
        {
            return View();
        }
    }
}