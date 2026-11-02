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
    public class Word_to_PDF_with_Outline_and_AttachmentsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Word_to_PDF_with_Outline_and_AttachmentsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConvertWordToPdf(Word_to_PDF_with_Outline_and_Attachments_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            byte[] inputWordBytes = await ReadWordFile(model.WordFile, model.WordFileUrl);

            // Create a Word to PDF converter object with default settings
            WordToPdfConverter wordToPdfConverter = new WordToPdfConverter();

            // The same outline, bookmarks, open action and attachments options are available in the
            // Excel, RTF and Markdown to PDF converters

            // Create the outline of the PDF document from the headings of the Word document, with a
            // bookmark for each heading, nested by the heading level
            wordToPdfConverter.PdfDocumentOptions.GenerateDocumentOutline = model.GenerateDocumentOutline;

            int firstContentPage = 1;
            if (model.AddStartPdf)
            {
                // The pages of an existing PDF document are placed before the converted content. The
                // page numbers of the bookmarks and of the open action count these pages
                string startPdfPath = Path.Combine(GetDemoFilesPath(), "PDF_Files", "PDF_Document.pdf");
                wordToPdfConverter.PdfDocumentOptions.AddStartPdf(startPdfPath);

                using PdfSplit startPdf = new PdfSplit(startPdfPath);
                firstContentPage += startPdf.PageCount;
            }

            if (model.AddCustomBookmarks)
            {
                // Bookmarks added to the outline after the ones created from the headings: a bookmark to
                // a page location, with a child bookmark and a bookmark opening a web address
                PdfBookmark contentBookmark = wordToPdfConverter.PdfDocumentOptions.Bookmarks.Add(
                    "Converted Word document", firstContentPage, PdfLinkPageLocation.FitWidth(0));
                contentBookmark.Bold = true;
                contentBookmark.Color = PdfColor.DarkBlue;
                contentBookmark.Children.Add("Top of the first page, zoomed", firstContentPage, PdfLinkPageLocation.AtCoordinates(0, 0, 1.5f));

                wordToPdfConverter.PdfDocumentOptions.Bookmarks.AddUrl("Library web site", "https://www.evopdf.com");
            }

            if (model.SetOpenAction)
            {
                // The action executed when the document is opened: go to the first page of the
                // converted content, fit to the page width
                wordToPdfConverter.PdfDocumentOptions.OpenAction = new PdfGoToPageAction(firstContentPage, PdfLinkPageLocation.FitWidth(0));
            }

            if (model.AttachSourceDocument)
            {
                // Attach the source Word document to the generated PDF. The attachment appears in the
                // Attachments panel of the PDF viewer
                PdfFileAttachment attachment = PdfFileAttachment.FromBytes(inputWordBytes, "Source_Document.docx");
                attachment.Description = "The Word document the PDF was generated from";
                wordToPdfConverter.PdfDocumentOptions.AddFileAttachment(attachment);
            }

            // The panel shown by the viewer when the document is opened: the outline or the attachments
            wordToPdfConverter.PdfViewerPreferences.PageMode = SelectedPageMode(model.PageMode);

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = wordToPdfConverter.ConvertToPdf(inputWordBytes);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Word_to_Pdf_Outline_Attachments.pdf";

            return fileResult;
        }

        private ViewerPageMode SelectedPageMode(string selectedValue)
        {
            switch (selectedValue)
            {
                case "UseOutlines":
                    return ViewerPageMode.UseOutlines;
                case "UseAttachments":
                    return ViewerPageMode.UseAttachments;
                default:
                    return ViewerPageMode.UseNone;
            }
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

        private Word_to_PDF_with_Outline_and_Attachments_ViewModel SetViewModel()
        {
            var model = new Word_to_PDF_with_Outline_and_Attachments_ViewModel();

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Word_to_PDF_with_Outline_and_Attachments".Length);

            model.WordFileUrl = rootUrl + "/DemoAppFiles/Input/Word_Files/Word_Document.docx";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
    }
}
