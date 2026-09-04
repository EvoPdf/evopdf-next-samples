using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_to_Image;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_to_Image
{
    public class PDF_to_ImageController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public PDF_to_ImageController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConvertPdfToImage(PDF_to_Image_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create the PDF to Image converter instance with default options
            PdfToImageConverter pdfToImageConverter = new PdfToImageConverter();

            // Optionally set the user password to open a password-protected PDF
            if (!string.IsNullOrEmpty(model.UserPassword))
                pdfToImageConverter.UserPassword = model.UserPassword;

            // Optionally set the owner password to open a password-protected PDF
            if (!string.IsNullOrEmpty(model.OwnerPassword))
                pdfToImageConverter.OwnerPassword = model.OwnerPassword;

            // Set the color space of the resulting images
            pdfToImageConverter.ColorSpace = SelectedColorSpace(model.ColorSpace);

            // Set the resolution of the resulting images
            pdfToImageConverter.Resolution = model.Resolution;

            // Set whether image background transparency is enabled
            pdfToImageConverter.TransparencyEnabled = model.TransparencyEnabled;

            // PDF page number to start conversion from
            int startPageNumber = model.StartPageNumber;

            // PDF page number to end conversion at
            // If 0, conversion continues to the end of the document
            int endPageNumber = 0;
            if (model.EndPageNumber.HasValue)
                endPageNumber = model.EndPageNumber.Value;

            byte[] inputPdfBytes = null;
            string outputFileName = null;

            // If an uploaded file exists, use it with priority
            if (model.PdfFile != null && model.PdfFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await model.PdfFile.CopyToAsync(ms);
                    inputPdfBytes = ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded PDF file", ex);
                }

                outputFileName = Path.GetFileNameWithoutExtension(model.PdfFile.FileName);
            }
            else
            {
                // Otherwise, fall back to the URL
                string pdfUrl = model.PdfFileUrl?.Trim();
                if (string.IsNullOrWhiteSpace(pdfUrl))
                    throw new Exception("No PDF file provided: upload a file or specify a URL");

                try
                {
                    if (pdfUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                    {
                        string localPath = new Uri(pdfUrl).LocalPath;
                        inputPdfBytes = await System.IO.File.ReadAllBytesAsync(localPath);
                    }
                    else
                    {
                        using var httpClient = new System.Net.Http.HttpClient();
                        inputPdfBytes = await httpClient.GetByteArrayAsync(pdfUrl);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not download the PDF file from URL", ex);
                }

                outputFileName = Path.GetFileNameWithoutExtension(model.PdfFileUrl);
            }

            // Convert to images the specified PDF page range
            PdfPageImage[] pdfPageImages = pdfToImageConverter.ConvertToImages(inputPdfBytes, startPageNumber, endPageNumber);

            if (pdfPageImages.Length == 1)
            {
                // Return the single image as a downloadable file
                outputFileName += ".png";
                return File(pdfPageImages[0].ImageData, "image/png", outputFileName);
            }
            else
            {
                // Build an in-memory ZIP with all page images
                using var zipMs = new MemoryStream();
                using (var zip = new System.IO.Compression.ZipArchive(zipMs, System.IO.Compression.ZipArchiveMode.Create, leaveOpen: true))
                {
                    foreach (var pdfPageImage in pdfPageImages)
                    {
                        var entry = zip.CreateEntry($"page-{pdfPageImage.PageNumber:000000}.png", System.IO.Compression.CompressionLevel.Fastest);

                        // Write the image bytes into the ZIP entry
                        using var entryStream = entry.Open();
                        entryStream.Write(pdfPageImage.ImageData, 0, pdfPageImage.ImageData.Length);
                    }
                }

                outputFileName += ".zip";

                // Copy ZIP memory stream to a byte array
                byte[] outputZipBytes = zipMs.ToArray();

                // Return the ZIP as a downloadable file                
                return File(outputZipBytes, "application/zip", outputFileName);
            }
        }

        private PdfPageImageColorSpace SelectedColorSpace(string colorSpace)
        {
            switch (colorSpace)
            {
                case "RGB":
                    return PdfPageImageColorSpace.RGB;
                case "Mono":
                    return PdfPageImageColorSpace.Mono;
                case "Gray":
                    return PdfPageImageColorSpace.Gray;
                default:
                    return PdfPageImageColorSpace.RGB;
            }
        }

        private PDF_to_Image_ViewModel SetViewModel()
        {
            var model = new PDF_to_Image_ViewModel();

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "PDF_to_Image".Length);

            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PdfProcessor_Files/PDF_Document.pdf";

            return model;
        }
    }
}