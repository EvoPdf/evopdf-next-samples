// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/html-to-image-converter-overview.htm
// Documentation page: HTML to Image Converter Overview

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_Image;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_Image
{
    public class Convert_HTML_to_ImageController : Controller
    {
        [HttpPost]
        public ActionResult ConvertHtmlToImage(Convert_HTML_to_Image_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create a HTML to Image converter object with default settings
            HtmlToImageConverter htmlToImageConverter = new HtmlToImageConverter();

            // Set HTML Viewer width in pixels which is the equivalent in converter of the browser window width
            htmlToImageConverter.HtmlViewerWidth = model.HtmlViewerWidth;

            // Set HTML viewer height in pixels to convert the top part of a HTML page 
            // Leave it not set to convert the entire HTML
            if (model.HtmlViewerHeight.HasValue)
                htmlToImageConverter.HtmlViewerHeight = model.HtmlViewerHeight.Value;

            // enable the conversion of the entire page, not only the viewport defined by HtmlViewerWidth and HtmlViewerHeight
            htmlToImageConverter.CaptureEntirePage = model.CaptureEntirePage;

            // Set the loading mode used to capture the entire page content
            htmlToImageConverter.CaptureEntirePageMode = model.CaptureEntirePageMode == "Browser" ?
                CaptureEntirePageMode.Browser : CaptureEntirePageMode.Custom;

            // Optionally auto resize HTML viewer height at the HTML content size determined after the initial loading
            htmlToImageConverter.AutoResizeHtmlViewerHeight = model.AutoResizeViewerHeight;

            // Set the maximum time in seconds to wait for HTML page to be loaded 
            // Leave it not set for a default 120 seconds maximum wait time
            htmlToImageConverter.NavigationTimeout = model.NavigationTimeout;

            // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
            if (model.ConversionDelay.HasValue)
                htmlToImageConverter.ConversionDelay = model.ConversionDelay.Value;

            byte[] outImageBuffer = null;
            if (model.HtmlPageSource == "Url")
            {
                string url = model.Url;

                // Convert the HTML page given by an URL to an image into a memory buffer
                outImageBuffer = htmlToImageConverter.ConvertUrl(url, SelectedImageFormat(model.ImageFormat));
            }
            else
            {
                string htmlString = model.HtmlString;
                string baseUrl = model.BaseUrl;

                // Convert a HTML string with a base URL to an image into a memory buffer
                outImageBuffer = htmlToImageConverter.ConvertHtml(htmlString ?? string.Empty, baseUrl, SelectedImageFormat(model.ImageFormat));
            }

            string imageFormatName = model.ImageFormat.ToLower();

            // Send the image file to browser
            FileResult fileResult = new FileContentResult(outImageBuffer, "image/" + (imageFormatName == "jpg" ? "jpeg" : imageFormatName));
            fileResult.FileDownloadName = "HTML_to_Image." + imageFormatName;

            return fileResult;
        }

        private ImageType SelectedImageFormat(string selectedValue)
        {
            switch (selectedValue)
            {
                case "Png":
                    return ImageType.Png;
                case "Jpg":
                    return ImageType.Jpeg;
                case "Webp":
                    return ImageType.Webp;
                default:
                    return ImageType.Png;
            }
        }
    }
}
