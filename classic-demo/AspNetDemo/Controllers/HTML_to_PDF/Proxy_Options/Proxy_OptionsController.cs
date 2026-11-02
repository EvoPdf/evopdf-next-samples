using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class Proxy_OptionsController : Controller
    {
        // GET: Proxy_Options
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(IFormCollection collection)
        {
            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            htmlToPdfConverter.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
            htmlToPdfConverter.ConversionDelay = 2;

            // Set proxy type
            // when converting HTML pages from HTTP addresses use the Http proxy type
            // when converting HTML pages from HTTPS addresses use Socks5 proxy type and make sure the proxy server
            // is also configured to use SOCKS5 protocol
            htmlToPdfConverter.ProxyOptions.Type = SelectedProxyType(collection["proxyTypeDropDownList"]);

            // Set proxy hostname and port number
            // Hostname and port number are required when the proxy type is set to something different from None value
            htmlToPdfConverter.ProxyOptions.HostName = collection["hostNameTextBox"];
            htmlToPdfConverter.ProxyOptions.PortNumber = int.Parse(collection["portNumberTextBox"]);

            // Optionally set proxy username and password if they are required by proxy server
            htmlToPdfConverter.ProxyOptions.Username = collection["usernameTextBox"];
            htmlToPdfConverter.ProxyOptions.Password = collection["passwordTextBox"];

            // Optionally set a list of hosts to be accessed directly without a proxy
            if (collection["bypassedHostTextBox"][0].Length > 0)
                htmlToPdfConverter.ProxyOptions.BypassedHosts = new string[] { collection["bypassedHostTextBox"] };

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(collection["urlTextBox"]);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Proxy_Options.pdf";

            return fileResult;
        }

        private NetworkProxyType SelectedProxyType(string selectedValue)
        {
            switch (selectedValue)
            {
                case "None":
                    return NetworkProxyType.None;
                case "Http":
                    return NetworkProxyType.Http;
                case "Socks5":
                    return NetworkProxyType.Socks5;
                case "HttpCaching":
                    return NetworkProxyType.HttpCaching;
                default:
                    return NetworkProxyType.None;
            }
        }
    }
}