using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.HTML_to_Image
{
    public class Convert_HTML_to_ImageController : Controller
    {
        [HttpPost]
        public ActionResult ConvertHtmlToImage(IFormCollection collection)
        {
            // Create a HTML to Image converter object with default settings
            HtmlToImageConverter htmlToImageConverter = new HtmlToImageConverter();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            htmlToImageConverter.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Set HTML Viewer width in pixels which is the equivalent in converter of the browser window width
            htmlToImageConverter.HtmlViewerWidth = int.Parse(collection["htmlViewerWidthTextBox"]);

            // Set HTML viewer height in pixels to convert the top part of a HTML page 
            // Leave it not set to convert the entire HTML
            if (collection["htmlViewerHeightTextBox"][0].Length > 0)
                htmlToImageConverter.HtmlViewerHeight = int.Parse(collection["htmlViewerHeightTextBox"]);

            // Set if the created image has a transparent background
            htmlToImageConverter.TransparentBackground = SelectedImageFormat(collection["imageFormatComboBox"]) == System.Drawing.Imaging.ImageFormat.Png ? collection["transparentBackgroundCheckBox"].Count > 0 : false;

            // Set the maximum time in seconds to wait for HTML page to be loaded 
            // Leave it not set for a default 60 seconds maximum wait time
            htmlToImageConverter.NavigationTimeout = int.Parse(collection["navigationTimeoutTextBox"]);

            // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
            if (collection["conversionDelayTextBox"][0].Length > 0)
                htmlToImageConverter.ConversionDelay = int.Parse(collection["conversionDelayTextBox"]);

            System.Drawing.Image[] imageTiles = null;

            if (collection["HtmlPageSource"] == "convertUrlRadioButton")
            {
                string url = collection["urlTextBox"];

                // Convert the HTML page given by an URL to a set of Image objects
                imageTiles = htmlToImageConverter.ConvertUrlToImageTiles(url);
            }
            else
            {
                string htmlString = collection["htmlStringTextBox"];
                string baseUrl = collection["baseUrlTextBox"];

                // Convert a HTML string with a base URL to a set of Image objects
                imageTiles = htmlToImageConverter.ConvertHtmlToImageTiles(htmlString, baseUrl);
            }

            // Save the first image tile to a memory buffer

            System.Drawing.Image outImage = imageTiles[0];

            // Create a memory stream where to save the image
            System.IO.MemoryStream imageOutputStream = new System.IO.MemoryStream();

            // Save the image to memory stream
            outImage.Save(imageOutputStream, SelectedImageFormat(collection["imageFormatComboBox"]));

            // Write the memory stream to a memory buffer
            imageOutputStream.Position = 0;
            byte[] outImageBuffer = imageOutputStream.ToArray();

            // Close the output memory stream
            imageOutputStream.Close();

            string imageFormatName = collection["imageFormatComboBox"][0].ToLower();
            
            // Send the image file to browser
            FileResult fileResult = new FileContentResult(outImageBuffer, "image/" + (imageFormatName == "jpg" ? "jpeg" : imageFormatName));
            fileResult.FileDownloadName = "HTML_to_Image." + imageFormatName;

            return fileResult;
        }

        private System.Drawing.Imaging.ImageFormat SelectedImageFormat(string selectedValue)
        {
            switch (selectedValue)
            {
                case "Png":
                    return System.Drawing.Imaging.ImageFormat.Png;
                case "Jpg":
                    return System.Drawing.Imaging.ImageFormat.Jpeg;
                case "Bmp":
                    return System.Drawing.Imaging.ImageFormat.Bmp;
                default:
                    return System.Drawing.Imaging.ImageFormat.Png;
            }
        }
    }
}