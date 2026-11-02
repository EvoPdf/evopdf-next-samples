using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.Word_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.Word_to_PDF
{
    public class Word_to_PDF_with_FontsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Word_to_PDF_with_FontsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConvertWordToPdf(Word_to_PDF_with_Fonts_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // Create a Word to PDF converter object with default settings
            WordToPdfConverter wordToPdfConverter = new WordToPdfConverter();

            // Use the fonts stored in the Word document. Word embeds the fonts of a document when the
            // option to embed fonts is on when the document is saved, so the text keeps its look where
            // the fonts are not installed. When this option is off the stored fonts are ignored and the
            // text is rendered with the installed fonts
            wordToPdfConverter.UseEmbeddedFonts = model.UseEmbeddedFonts;

            string fontsPath = GetDemoFontsPath();

            if (model.UseFontFile)
            {
                // A font file given to the converter is used for the text set in its font family, in
                // place of an installed font with the same family name or when that font is not installed
                wordToPdfConverter.FontFiles.Add(Path.Combine(fontsPath, "DejaVuSerif.ttf"));
            }

            if (model.UseFallbackFontFile)
            {
                // A fallback font file is used for the characters that no font of the document covers,
                // such as the Arabic text of the sample document
                wordToPdfConverter.FallbackFontFiles.Add(Path.Combine(fontsPath, "NotoSansArabic-Regular.ttf"));
            }

            byte[] inputWordBytes = await ReadWordFile(model.WordFile, model.WordFileUrl);

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = wordToPdfConverter.ConvertToPdf(inputWordBytes);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Word_to_Pdf_Fonts.pdf";

            return fileResult;
        }

        private async Task<byte[]> ReadWordFile(IFormFile wordFile, string wordFileUrl)
        {
            // If an uploaded file exists, use it with priority
            if (wordFile != null && wordFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await wordFile.CopyToAsync(ms);
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded Word file", ex);
                }
            }

            // Otherwise, fall back to the URL
            string wordUrl = wordFileUrl?.Trim();
            if (string.IsNullOrWhiteSpace(wordUrl))
                throw new Exception("No Word file provided: upload a file or specify a URL");

            try
            {
                if (wordUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    string localPath = new Uri(wordUrl).LocalPath;
                    return await System.IO.File.ReadAllBytesAsync(localPath);
                }

                using var httpClient = new System.Net.Http.HttpClient();
                return await httpClient.GetByteArrayAsync(wordUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not download the Word file from URL", ex);
            }
        }

        private Word_to_PDF_with_Fonts_ViewModel SetViewModel()
        {
            var model = new Word_to_PDF_with_Fonts_ViewModel();

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Word_to_PDF_with_Fonts".Length);

            model.WordFileUrl = rootUrl + "/DemoAppFiles/Input/Word_Files/Word_Document_Fonts.docx";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
