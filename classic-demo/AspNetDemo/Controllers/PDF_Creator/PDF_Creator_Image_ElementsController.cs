using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;
using System.Drawing;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator
{
    public class PDF_Creator_Image_ElementsController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public PDF_Creator_Image_ElementsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: PDF_Creator_Image_Elements
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePdf(IFormCollection collection)
        {
            // Create a PDF document
            Document pdfDocument = new Document();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            pdfDocument.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            try
            {
                // The result of adding elements to PDF document
                AddElementResult addElementResult = null;

                // The titles font used to mark various sections of the PDF document
                PdfFont titleFont = pdfDocument.AddFont(new Font("Times New Roman", 12, FontStyle.Bold, GraphicsUnit.Point));
                PdfFont subtitleFont = pdfDocument.AddFont(new Font("Times New Roman", 8, FontStyle.Bold, GraphicsUnit.Point));

                // The position on X anf Y axes where to add the next element
                float yLocation = 5;
                float xLocation = 5;

                // Create a PDF page in PDF document
                PdfPage pdfPage = pdfDocument.AddPage();

                // Add section title
                TextElement titleTextElement = new TextElement(xLocation, yLocation, "Images Scaling", titleFont);
                titleTextElement.ForeColor = Color.Black;
                addElementResult = pdfPage.AddElement(titleTextElement);
                yLocation = addElementResult.EndPageBounds.Bottom + 10;
                pdfPage = addElementResult.EndPdfPage;

                float titlesYLocation = yLocation;

                // Add an unscaled image

                // Add section title
                TextElement subtitleTextElement = new TextElement(xLocation, titlesYLocation, "Unscaled small image with normal resolution", subtitleFont);
                subtitleTextElement.ForeColor = Color.Navy;
                addElementResult = pdfPage.AddElement(subtitleTextElement);

                pdfPage = addElementResult.EndPdfPage;
                float imagesYLocation = addElementResult.EndPageBounds.Bottom + 10;

                string imagePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Images/picture_small.jpg";
                ImageElement unscaledImageElement = new ImageElement(xLocation, imagesYLocation, imagePath);
                addElementResult = pdfPage.AddElement(unscaledImageElement);

                RectangleF scaledDownImageRectangle = new RectangleF(addElementResult.EndPageBounds.Right + 30, addElementResult.EndPageBounds.Y,
                            addElementResult.EndPageBounds.Width, addElementResult.EndPageBounds.Height);

                // Add a large image scaled down to same size in PDF

                // Add section title
                subtitleTextElement = new TextElement(scaledDownImageRectangle.X, titlesYLocation, "Scaled down large image has higher resolution", subtitleFont);
                subtitleTextElement.ForeColor = Color.Navy;
                pdfPage.AddElement(subtitleTextElement);

                imagePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Images/picture_large.jpg";
                ImageElement scaledDownImageElement = new ImageElement(scaledDownImageRectangle.X, scaledDownImageRectangle.Y, scaledDownImageRectangle.Width, imagePath);
                AddElementResult scaledDownImageResult = pdfPage.AddElement(scaledDownImageElement);

                // Add a border around the scaled down image
                RectangleElement borderElement = new RectangleElement(scaledDownImageRectangle);
                pdfPage.AddElement(borderElement);

                // Add an unscaled small image

                float columnX = scaledDownImageResult.EndPageBounds.Right + 30;

                // Add section title
                subtitleTextElement = new TextElement(columnX, titlesYLocation, "Unscaled small image", subtitleFont);
                subtitleTextElement.ForeColor = Color.Navy;
                pdfPage.AddElement(subtitleTextElement);

                imagePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Images/picture_smaller.jpg";
                unscaledImageElement = new ImageElement(columnX, imagesYLocation, imagePath);
                AddElementResult unscaledImageResult = pdfPage.AddElement(unscaledImageElement);

                RectangleF unscaledImageRectangle = unscaledImageResult.EndPageBounds;

                // Add an enlarged image

                // Add section title
                subtitleTextElement = new TextElement(columnX, unscaledImageRectangle.Bottom + 10, "Enlarged small image has lower resolution", subtitleFont);
                subtitleTextElement.ForeColor = Color.Navy;
                AddElementResult enlargedImageTitle = pdfPage.AddElement(subtitleTextElement);

                float enlargedImageWidth = unscaledImageRectangle.Width + 35;

                imagePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Images/picture_smaller.jpg";
                ImageElement enlargedImageElement = new ImageElement(columnX, enlargedImageTitle.EndPageBounds.Bottom + 10, enlargedImageWidth, imagePath);
                // Allow the image to be enlarged
                enlargedImageElement.EnlargeEnabled = true;
                AddElementResult enalargedImageResult = pdfPage.AddElement(enlargedImageElement);

                yLocation = addElementResult.EndPageBounds.Bottom + 10;

                // Scale an image preserving the aspect ratio

                titlesYLocation = yLocation;

                // Add section title
                subtitleTextElement = new TextElement(xLocation, titlesYLocation, "Scaled down image preserving aspect ratio", subtitleFont);
                subtitleTextElement.ForeColor = Color.Navy;
                addElementResult = pdfPage.AddElement(subtitleTextElement);

                pdfPage = addElementResult.EndPdfPage;
                yLocation = addElementResult.EndPageBounds.Bottom + 10;

                RectangleF boundingRectangle = new RectangleF(xLocation, yLocation, scaledDownImageRectangle.Width, scaledDownImageRectangle.Width);
                imagesYLocation = boundingRectangle.Y;

                imagePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Images/landscape.jpg";
                ImageElement keepAspectImageElement = new ImageElement(boundingRectangle.X, imagesYLocation, boundingRectangle.Width, boundingRectangle.Width, true, imagePath);
                addElementResult = pdfPage.AddElement(keepAspectImageElement);

                borderElement = new RectangleElement(boundingRectangle);
                borderElement.ForeColor = Color.Black;
                pdfPage.AddElement(borderElement);

                // Scale an image without preserving aspect ratio
                // This can produce a distorted image

                boundingRectangle = new RectangleF(addElementResult.EndPageBounds.Right + 30, addElementResult.EndPageBounds.Y, scaledDownImageRectangle.Width, scaledDownImageRectangle.Width);

                // Add section title
                subtitleTextElement = new TextElement(boundingRectangle.X, titlesYLocation, "Scaled down image without preserving aspect ratio", subtitleFont);
                subtitleTextElement.ForeColor = Color.Navy;
                pdfPage.AddElement(subtitleTextElement);

                imagePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Images/landscape.jpg";
                ImageElement notKeepAspectImageElement = new ImageElement(boundingRectangle.X, imagesYLocation, boundingRectangle.Width, boundingRectangle.Width, false, imagePath);
                addElementResult = pdfPage.AddElement(notKeepAspectImageElement);

                borderElement = new RectangleElement(boundingRectangle);
                borderElement.ForeColor = Color.Black;
                pdfPage.AddElement(borderElement);

                pdfPage = addElementResult.EndPdfPage;
                yLocation = addElementResult.EndPageBounds.Bottom + 20;

                // Add transparent images

                // Add section title
                titleTextElement = new TextElement(xLocation, yLocation, "Transparent Images", titleFont);
                titleTextElement.ForeColor = Color.Black;
                addElementResult = pdfPage.AddElement(titleTextElement);
                yLocation = addElementResult.EndPageBounds.Bottom + 10;
                pdfPage = addElementResult.EndPdfPage;

                imagePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Images/transparent.png";
                ImageElement trasparentImageElement = new ImageElement(xLocation, yLocation, 150, imagePath);
                addElementResult = pdfPage.AddElement(trasparentImageElement);

                imagePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Images/rose.png";
                trasparentImageElement = new ImageElement(addElementResult.EndPageBounds.Right + 60, yLocation + 20, 150, imagePath);
                pdfPage.AddElement(trasparentImageElement);

                pdfPage = addElementResult.EndPdfPage;
                yLocation = addElementResult.EndPageBounds.Bottom + 20;

                // Rotate images

                // Add section title
                titleTextElement = new TextElement(xLocation, yLocation, "Rotated Images", titleFont);
                titleTextElement.ForeColor = Color.Black;
                addElementResult = pdfPage.AddElement(titleTextElement);
                yLocation = addElementResult.EndPageBounds.Bottom + 10;
                pdfPage = addElementResult.EndPdfPage;

                // Add a not rotated image
                imagePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Images/compass.png";
                ImageElement noRotationImageElement = new ImageElement(xLocation, yLocation, 125, imagePath);
                addElementResult = pdfPage.AddElement(noRotationImageElement);

                float imageXLocation = addElementResult.EndPageBounds.X;
                float imageYLocation = addElementResult.EndPageBounds.Y;
                float imageWidth = addElementResult.EndPageBounds.Width;
                float imageHeight = addElementResult.EndPageBounds.Height;

                // The rotated coordinates system location
                float rotatedImageXLocation = imageXLocation + imageWidth + 20 + imageHeight;
                float rotatedImageYLocation = imageYLocation;

                // Add the image rotated 90 degrees
                ImageElement rotate90ImageElement = new ImageElement(0, 0, 125, imagePath);
                rotate90ImageElement.Translate(rotatedImageXLocation, rotatedImageYLocation);
                rotate90ImageElement.Rotate(90);
                pdfPage.AddElement(rotate90ImageElement);

                rotatedImageXLocation += 20 + imageWidth;
                rotatedImageYLocation = imageYLocation + imageHeight;

                // Add the image rotated 180 degrees
                ImageElement rotate180ImageElement = new ImageElement(0, 0, 125, imagePath);
                rotate180ImageElement.Translate(rotatedImageXLocation, rotatedImageYLocation);
                rotate180ImageElement.Rotate(180);
                pdfPage.AddElement(rotate180ImageElement);

                rotatedImageXLocation += 20;
                rotatedImageYLocation = imageYLocation + imageWidth;

                // Add the image rotated 270 degrees
                ImageElement rotate270ImageElement = new ImageElement(0, 0, 125, imagePath);
                rotate270ImageElement.Translate(rotatedImageXLocation, rotatedImageYLocation);
                rotate270ImageElement.Rotate(270);
                pdfPage.AddElement(rotate270ImageElement);

                pdfPage = addElementResult.EndPdfPage;
                yLocation = addElementResult.EndPageBounds.Bottom + 20;

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Image_Elements.pdf";

                return fileResult;
            }
            finally
            {
                // Close the PDF document
                pdfDocument.Close();
            }
        }
    }
}