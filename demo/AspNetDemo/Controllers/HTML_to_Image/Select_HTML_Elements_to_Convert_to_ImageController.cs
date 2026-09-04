using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_Image;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_Image
{
    public class Select_HTML_Elements_to_Convert_to_ImageController : Controller
    {
        [HttpPost]
        public ActionResult ConvertHtmlToImage(Select_HTML_Elements_to_Convert_to_Image_ViewModel model)
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

            bool enableElementsSelector = model.EnableElementsSelector;
            if (enableElementsSelector)
            {
                // The CSS selector used to identify the elements to include in the image
                htmlToImageConverter.ConvertedElementsSelector = model.ConvertedElementsSelector;

                // Specify whether elements that are not matched by ConvertedElementsSelector
                // should be completely removed from the layout rather than just hidden
                htmlToImageConverter.RemoveUnselectedElements = model.RemoveUnselectedElements;

                // Allow image to auto-resize at minimum height
                htmlToImageConverter.HtmlViewerHeight = 1;
            }

            byte[] outImageBuffer = null;

            if (model.HtmlPageSource == "Html")
            {
                string htmlWithForm = model.HtmlString;
                string baseUrl = model.BaseUrl;

                // Convert a HTML string to a PNG image
                outImageBuffer = htmlToImageConverter.ConvertHtml(htmlWithForm ?? string.Empty, baseUrl, ImageType.Png);
            }
            else
            {
                string url = model.Url;

                // Convert the HTML page to a PNG image
                outImageBuffer = htmlToImageConverter.ConvertUrl(url, ImageType.Png);
            }

            // Send the image file to browser
            FileResult fileResult = new FileContentResult(outImageBuffer, "image/png");
            fileResult.FileDownloadName = "Select_HTML_Elements_to_Convert_to_Image.png";

            return fileResult;
        }
    }
}
