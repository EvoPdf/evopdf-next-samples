using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Add_Bookmarks_and_Open_ActionController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Add_Bookmarks_and_Open_ActionController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Add_Bookmarks_and_Open_Action_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // The bookmarks created from the headings of the HTML document come first in the outline
            htmlToPdfConverter.PdfDocumentOptions.GenerateDocumentOutline = model.GenerateDocumentOutline;

            int htmlFirstPage = 1;
            if (model.AddStartPdf)
            {
                // A PDF document inserted before the HTML content; the page numbers of the bookmarks and of the open action
                // count its pages
                string startPdfPath = Path.Combine(GetDemoFilesPath(), "PDF_Files", "Letterhead.pdf");
                htmlToPdfConverter.PdfDocumentOptions.AddStartPdf(startPdfPath);
                htmlFirstPage = 2;
            }

            if (model.AddBookmarks)
            {
                PdfBookmarkCollection bookmarks = htmlToPdfConverter.PdfDocumentOptions.Bookmarks;

                // Bookmarks added by code, after the bookmarks created from the headings. The page numbers refer to the
                // pages of the generated document and the location in a page is given from its top left corner
                if (model.AddStartPdf)
                    bookmarks.Add("Cover page", 1, PdfLinkPageLocation.FitPage());

                PdfBookmark contentBookmark = bookmarks.Add("HTML content", htmlFirstPage, PdfLinkPageLocation.FitPage());
                contentBookmark.Open = true;
                contentBookmark.Children.Add("Start of the content", htmlFirstPage, PdfLinkPageLocation.FitWidth(0));
                contentBookmark.Children.Add("Second page at 150%", htmlFirstPage + 1, PdfLinkPageLocation.AtCoordinates(0, 0, 1.5f));
                contentBookmark.Children.Add("Middle of the second page", htmlFirstPage + 1, PdfLinkPageLocation.AtCoordinates(0, 400));

                // A bookmark that opens a web address in the browser
                bookmarks.AddUrl("Product web site", "https://www.evopdf.com");

                if (model.StyleBookmarks)
                {
                    // The title of a bookmark can be bold, italic and colored
                    contentBookmark.Bold = true;
                    contentBookmark.Color = PdfColor.DarkBlue;
                    foreach (PdfBookmark child in contentBookmark.Children)
                        child.Italic = true;
                    bookmarks.Find("Product web site").Color = PdfColor.DarkGreen;
                }
            }

            if (model.ShowBookmarksPanel)
            {
                // Display the bookmarks panel in PDF viewer when the generated PDF is opened
                htmlToPdfConverter.PdfViewerPreferences.PageMode = ViewerPageMode.UseOutlines;
            }

            switch (model.OpenAction)
            {
                case "SecondPage":
                    // The viewer opens the document at the second page, fitted to the window width
                    htmlToPdfConverter.PdfDocumentOptions.OpenAction = new PdfGoToPageAction(htmlFirstPage + 1, PdfLinkPageLocation.FitWidth(0));
                    break;
                case "ZoomedPage":
                    htmlToPdfConverter.PdfDocumentOptions.OpenAction = new PdfGoToPageAction(htmlFirstPage, PdfLinkPageLocation.AtCoordinates(0, 0, 2f));
                    break;
                case "LastPage":
                    // A named action of the viewer
                    htmlToPdfConverter.PdfDocumentOptions.OpenAction = new PdfNamedAction(PdfNamedActionType.LastPage);
                    break;
                case "JavaScript":
                    htmlToPdfConverter.PdfDocumentOptions.OpenAction = new PdfJavaScriptAction("app.alert('The document was generated with the HTML to PDF converter');");
                    break;
                case "OpenUrl":
                    htmlToPdfConverter.PdfDocumentOptions.OpenAction = new PdfOpenUrlAction("https://www.evopdf.com");
                    break;
            }

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = null;

            if (model.HtmlPageSource == "Url")
            {
                string url = model.Url;

                // Convert the HTML page given by an URL to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
            }
            else
            {
                string htmlString = model.HtmlStringTextBox;
                string baseUrl = model.BaseUrlTextBox;

                // Convert a HTML string with a base URL to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlString, baseUrl);
            }

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Add_Bookmarks_and_Open_Action.pdf";

            return fileResult;
        }

        private Add_Bookmarks_and_Open_Action_ViewModel SetViewModel()
        {
            var model = new Add_Bookmarks_and_Open_Action_ViewModel();

            var contentRootPath = Path.Combine(m_hostingEnvironment.ContentRootPath, "wwwroot");

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Add_Bookmarks_and_Open_Action".Length);

            model.HtmlStringTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Auto_Bookmarks.html"));
            model.BaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
    }
}
