using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EvoPdf_Next_AspNetDemo.Controllers
{
    public class ErrorController : Controller
    {
        // Shows the message of the error that stopped the request, without the stack trace
        public IActionResult Index()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ViewBag.ErrorMessage = exceptionFeature?.Error?.Message ?? "The request could not be completed";
            return View();
        }
    }
}
