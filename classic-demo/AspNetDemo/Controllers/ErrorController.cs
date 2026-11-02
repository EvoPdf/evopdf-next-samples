using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class ErrorController : Controller
    {
        // Shows the message of the error that stopped the request, without the stack trace
        public IActionResult Index()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ViewBag.ErrorMessage = Message(exceptionFeature?.Error);
            return View();
        }

        // The message of the error and of the errors inside it, each one once
        private static string Message(Exception error)
        {
            if (error == null)
                return "The request could not be completed";
            var parts = new List<string>();
            for (Exception e = error; e != null; e = e.InnerException)
            {
                string text = e.Message?.Trim();
                if (!string.IsNullOrEmpty(text) && !parts.Contains(text))
                    parts.Add(text);
            }
            return string.Join(" ", parts);
        }
    }
}
