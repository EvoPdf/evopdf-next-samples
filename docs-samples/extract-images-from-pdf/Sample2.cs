// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/extract-images-from-pdf.htm
// Documentation page: Extract Images from PDF

using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_Images_Extractor;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_Images_Extractor
{
    public class Extract_PDF_ImagesController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Extract_PDF_ImagesController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExtractPdfImages(Extract_PDF_Images_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the extractor in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create the PDF Images Extractor instance with default options
            PdfImagesExtractor pdfImagesExtractor = new PdfImagesExtractor();

            // Optionally set the user password to open a password-protected PDF
            if (!string.IsNullOrEmpty(model.UserPassword))
                pdfImagesExtractor.UserPassword = model.UserPassword;

            // Optionally set the owner password to open a password-protected PDF
            if (!string.IsNullOrEmpty(model.OwnerPassword))
                pdfImagesExtractor.OwnerPassword = model.OwnerPassword;

            // PDF page number to start extraction from
            int startPageNumber = model.StartPageNumber;

            // PDF page number to end extraction at
            // If 0, extraction continues to the end of the document
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

            // Extract the images from the specified PDF page range, grouped by page
            ExtractedImage[][] extractedImages = pdfImagesExtractor.ExtractImages(inputPdfBytes, startPageNumber, endPageNumber);

            int nPdfPages = extractedImages.Length;
            if (nPdfPages == 1 && extractedImages[0].Length > 0 && model.ExtractLargest)
            {
                // If only one page was processed and only the largest image is requested, return that image directly
                // Return the largest image as a downloadable file
                outputFileName += "-largest.png";
                ExtractedImage largestImage = GetLargestImage(extractedImages[0]);
                return File(largestImage.ImageData, "image/png", outputFileName);
            }
            else
            {
                // Build an in-memory ZIP with all page images and return it
                using var zipMs = new MemoryStream();
                using (var zip = new System.IO.Compression.ZipArchive(zipMs, System.IO.Compression.ZipArchiveMode.Create, leaveOpen: true))
                {
                    for (int pageIdx = 0; pageIdx < extractedImages.Length; pageIdx++)
                    {
                        var pageImages = extractedImages[pageIdx];
                        if (model.ExtractLargest)
                        {
                            // Add only the largest image from the page to the ZIP
                            ExtractedImage largestImage = GetLargestImage(pageImages);
                            if (largestImage != null)
                            {
                                var entry = zip.CreateEntry($"page-{largestImage.PageNumber:000000}-largest.png", System.IO.Compression.CompressionLevel.Fastest);
                                // Write the image bytes into the ZIP entry
                                using var entryStream = entry.Open();
                                entryStream.Write(largestImage.ImageData, 0, largestImage.ImageData.Length);
                            }
                        }
                        else
                        {
                            // Add all images from the PDF page to the ZIP
                            for (int imgIdx = 0; imgIdx < pageImages.Length; imgIdx++)
                            {
                                ExtractedImage extractedImage = pageImages[imgIdx];
                                var entry = zip.CreateEntry($"page-{extractedImage.PageNumber:000000}-{imgIdx:000000}.png", System.IO.Compression.CompressionLevel.Fastest);

                                // Write the image bytes into the ZIP entry
                                using var entryStream = entry.Open();
                                entryStream.Write(extractedImage.ImageData, 0, extractedImage.ImageData.Length);
                            }
                        }
                    }
                }

                outputFileName += ".zip";

                // Copy ZIP memory stream to a byte array
                byte[] outputZipBytes = zipMs.ToArray();

                // Return the ZIP as a downloadable file
                return File(outputZipBytes, "application/zip", outputFileName);
            }
        }

        private ExtractedImage GetLargestImage(ExtractedImage[] extractedImages)
        {
            ExtractedImage largestImage = null;
            int largestSize = 0;
            foreach (var image in extractedImages)
            {
                if (image.ImageData.Length > largestSize)
                {
                    largestImage = image;
                    largestSize = image.ImageData.Length;
                }
            }
            return largestImage;
        }

        private Extract_PDF_Images_ViewModel SetViewModel()
        {
            var model = new Extract_PDF_Images_ViewModel();

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Extract_PDF_Images".Length);

            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PdfProcessor_Files/PDF_Document.pdf";

            return model;
        }
    }
}
