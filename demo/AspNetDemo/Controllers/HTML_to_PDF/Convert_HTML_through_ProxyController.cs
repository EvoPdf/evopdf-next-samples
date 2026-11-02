using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Convert_HTML_through_ProxyController : Controller
    {
        public IActionResult Index()
        {
            var model = new Convert_HTML_through_Proxy_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Convert_HTML_through_Proxy_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // The type of the proxy used to load the HTML page and its resources. With the None type
            // the network settings of the system are used, as when no proxy is configured
            htmlToPdfConverter.ProxyOptions.Type = SelectedProxyType(model.ProxyType);

            if (htmlToPdfConverter.ProxyOptions.Type != NetworkProxyType.None)
            {
                // The address of the proxy server
                htmlToPdfConverter.ProxyOptions.HostName = model.ProxyHostName?.Trim();
                htmlToPdfConverter.ProxyOptions.PortNumber = model.ProxyPortNumber;

                // The credentials for a proxy which requires authentication
                if (!string.IsNullOrEmpty(model.ProxyUsername))
                {
                    htmlToPdfConverter.ProxyOptions.Username = model.ProxyUsername;
                    htmlToPdfConverter.ProxyOptions.Password = model.ProxyPassword;
                }

                // The hosts accessed directly, without the proxy: host names, IP addresses, domains with
                // their subdomains (*.example.com) or CIDR ranges (10.0.0.0/8). The local machine is
                // always accessed directly
                if (!string.IsNullOrWhiteSpace(model.BypassedHosts))
                {
                    htmlToPdfConverter.ProxyOptions.BypassedHosts = model.BypassedHosts
                        .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(host => host.Trim())
                        .ToArray();
                }
            }

            // The same proxy options are available in the HTML to image converters

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(model.Url);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "HTML_through_Proxy.pdf";

            return fileResult;
        }

        private NetworkProxyType SelectedProxyType(string selectedValue)
        {
            switch (selectedValue)
            {
                case "Http":
                    return NetworkProxyType.Http;
                case "Socks5":
                    return NetworkProxyType.Socks5;
                default:
                    return NetworkProxyType.None;
            }
        }
    }
}
