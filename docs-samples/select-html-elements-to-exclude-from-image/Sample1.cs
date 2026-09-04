// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/select-html-elements-to-exclude-from-image.htm
// Documentation page: Select HTML Elements to Exclude from Image

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_Image;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_Image
{
    public class Select_HTML_Elements_to_Exclude_from_ImageController : Controller
    {
        [HttpPost]
        public ActionResult ConvertHtmlToImage(Select_HTML_Elements_to_Exclude_from_Image_ViewModel model)
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

            bool enableExcludedElementsSelector = model.EnableExcludedElementsSelector;
            if (enableExcludedElementsSelector)
            {
                // The CSS selector used to identify the elements to exclude from conversion to image
                htmlToImageConverter.ExcludedElementsSelector = model.ExcludedElementsSelector;

                // Specify whether elements that are not matched by ExcludedElementsSelector
                // should be completely removed from the layout rather than just hidden
                htmlToImageConverter.RemoveExcludedElements = model.RemoveExcludedElements;
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
            fileResult.FileDownloadName = "Select_HTML_Elements_to_Exclude_from_Image.png";

            return fileResult;
        }
    }
}
