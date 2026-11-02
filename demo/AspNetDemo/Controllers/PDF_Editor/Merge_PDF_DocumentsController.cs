using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_Editor;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_Editor
{
    public class Merge_PDF_DocumentsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Merge_PDF_DocumentsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MergePdf(Merge_PDF_Documents_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            byte[] firstPdfBytes = await ReadInputPdf(model.FirstPdfFile, model.FirstPdfFileUrl);
            byte[] secondPdfBytes = await ReadInputPdf(model.SecondPdfFile, model.SecondPdfFileUrl);
            string firstPassword = string.IsNullOrEmpty(model.FirstPassword) ? null : model.FirstPassword;
            string secondPassword = string.IsNullOrEmpty(model.SecondPassword) ? null : model.SecondPassword;

            using PdfMerge pdfMerge = new PdfMerge();

            // The bookmarks of the merged documents are copied in the merged document, under the bookmarks added below
            pdfMerge.KeepSourceBookmarks = model.KeepSourceBookmarks;

            // The fields with the same name in several merged documents are renamed, so each copy keeps its own value;
            // otherwise they become one field with the same value in all the copies
            pdfMerge.KeepFieldsSeparate = model.KeepFieldsSeparate;

            // The form fields and annotations of the merged document become static content
            pdfMerge.Flatten = model.FlattenMergedPdf;

            pdfMerge.PdfDocumentInfo.Title = model.DocumentTitle;

            // The first document, all its pages or the pages of the given ranges. The add methods return the number
            // of the first added page in the merged document
            int firstDocumentPage = AddPdf(pdfMerge, firstPdfBytes, model.FirstPageRanges, firstPassword);
            AddBookmark(pdfMerge, model.AddDocumentBookmarks, "First document", firstDocumentPage);

            if (model.AddBlankPage)
            {
                // A blank page of the given size between the documents
                int blankPage = pdfMerge.AddBlankPage(SelectedPdfPageSize(model.BlankPageSize), SelectedPdfPageOrientation(model.BlankPageOrientation));
                AddBookmark(pdfMerge, model.AddDocumentBookmarks, "Blank page", blankPage);
            }

            if (model.AddImagePage)
            {
                // A page displaying an image: the page has the size of the image, or the image is fitted in a page of the given size
                string imagePath = Path.Combine(GetDemoFilesPath(), "Image_Files", "image.jpg");
                int imagePage = model.FitImageInPage
                    ? pdfMerge.AddImagePage(imagePath, PdfPageSize.A4, PdfPageOrientation.Portrait)
                    : pdfMerge.AddImagePage(imagePath);
                AddBookmark(pdfMerge, model.AddDocumentBookmarks, "Image page", imagePage);
            }

            int secondDocumentPage = AddPdf(pdfMerge, secondPdfBytes, model.SecondPageRanges, secondPassword);
            AddBookmark(pdfMerge, model.AddDocumentBookmarks, "Second document", secondDocumentPage);

            if (model.AddSecondPdfTwice)
            {
                int copyPage = AddPdf(pdfMerge, secondPdfBytes, model.SecondPageRanges, secondPassword);
                AddBookmark(pdfMerge, model.AddDocumentBookmarks, "Second document, copy", copyPage);
            }

            if (model.AddHeaderFooter)
            {
                // A template is drawn in all the pages of the merged document; the page number variables are replaced in each page
                PdfHtmlTemplate footer = pdfMerge.AddHtmlTemplate(0, 0, 300, 30,
                    "<div style=\"font-family: Arial; font-size: 10px; color: #606060;\">Merged document, page {page_number} of {total_pages}</div>", null);
                footer.HorizontalAlign = PdfTemplateHorizontalAlign.Center;
                footer.VerticalAlign = PdfTemplateVerticalAlign.Bottom;
            }

            switch (model.OpenAction)
            {
                case "SecondDocument":
                    // The merged document opens at the first page of the second document
                    pdfMerge.OpenAction = new PdfGoToPageAction(secondDocumentPage, PdfLinkPageLocation.FitPage());
                    break;
                case "LastPage":
                    pdfMerge.OpenAction = new PdfGoToPageAction(pdfMerge.PageCount, PdfLinkPageLocation.FitWidth(0));
                    break;
                case "JavaScript":
                    pdfMerge.OpenAction = new PdfJavaScriptAction("app.alert('The merged document has " + pdfMerge.PageCount + " pages');");
                    break;
            }

            byte[] outPdfBuffer = pdfMerge.Save();

            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Merge_PDF_Documents.pdf";
            return fileResult;
        }

        // Adds all the pages of a document, or the pages of the given ranges, and returns the number of the first
        // added page in the merged document
        private static int AddPdf(PdfMerge pdfMerge, byte[] pdfBytes, string pageRanges, string password)
        {
            if (string.IsNullOrWhiteSpace(pageRanges))
                return pdfMerge.AddPdf(pdfBytes, password);

            return pdfMerge.AddPdfPages(pdfBytes, pageRanges.Trim(), password);
        }

        private static void AddBookmark(PdfMerge pdfMerge, bool addBookmarks, string title, int pageNumber)
        {
            if (addBookmarks)
                pdfMerge.Bookmarks.Add(title, pageNumber, PdfLinkPageLocation.FitPage());
        }

        private static PdfPageSize SelectedPdfPageSize(string selectedValue)
        {
            switch (selectedValue)
            {
                case "A3": return PdfPageSize.A3;
                case "A5": return PdfPageSize.A5;
                case "Letter": return PdfPageSize.Letter;
                case "Legal": return PdfPageSize.Legal;
                case "Ledger": return PdfPageSize.Ledger;
                default: return PdfPageSize.A4;
            }
        }

        private static PdfPageOrientation SelectedPdfPageOrientation(string selectedValue)
        {
            return selectedValue == "Landscape" ? PdfPageOrientation.Landscape : PdfPageOrientation.Portrait;
        }

        private async Task<byte[]> ReadInputPdf(IFormFile pdfFile, string pdfFileUrl)
        {
            // If an uploaded file exists, use it with priority
            if (pdfFile != null && pdfFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await pdfFile.CopyToAsync(ms);
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded PDF file", ex);
                }
            }

            // Otherwise, fall back to the URL
            string pdfUrl = pdfFileUrl?.Trim();
            if (string.IsNullOrWhiteSpace(pdfUrl))
                throw new Exception("No PDF file provided: upload a file or specify a URL");

            try
            {
                if (pdfUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    string localPath = new Uri(pdfUrl).LocalPath;
                    return await System.IO.File.ReadAllBytesAsync(localPath);
                }

                using var httpClient = new System.Net.Http.HttpClient();
                return await httpClient.GetByteArrayAsync(pdfUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not download the PDF file from URL", ex);
            }
        }

        private Merge_PDF_Documents_ViewModel SetViewModel()
        {
            var model = new Merge_PDF_Documents_ViewModel();

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Path = request.PathBase.ToString() + request.Path.ToString(),
                Query = request.QueryString.ToString()
            };
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Merge_PDF_Documents".Length);

            model.FirstPdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/Multi_Page_Document.pdf";
            model.SecondPdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/PDF_Form.pdf";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
    }
}
