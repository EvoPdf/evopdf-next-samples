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
    public class Table_of_ContentsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Table_of_ContentsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Table_of_Contents_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Enable or disable the automatic creation of a table of contents in the PDF document based on H1 to H6 HTML tags
            htmlToPdfConverter.PdfDocumentOptions.GenerateTableOfContents = model.GenerateToc;

            // Optionally set the table of contents to display inline within the 'html-to-pdf-toc' DIV
            // By default, the table of contents is created at the start of the generated PDF document
            htmlToPdfConverter.PdfDocumentOptions.TableOfContents.CreateInline = model.InlineToc;

            // Optionally enable the usage of browser capabilities when creating the table of contents
            // This option is applicable only if the table of contents is not inline and it is false by default
            htmlToPdfConverter.PdfDocumentOptions.TableOfContents.UseBrowserMode = model.UseBrowserMode;

            // Set if the page numbers from table of contents are displyed
            htmlToPdfConverter.PdfDocumentOptions.TableOfContents.ShowPageNumbers = model.ShowPageNumbers;

            // Set if the TOC pages are included in the page numbers displayed in the TOC.
            // This option is not applicable to the inline table of contents
            htmlToPdfConverter.PdfDocumentOptions.TableOfContents.CountTocAndStartPages = model.CountTocPages;

            // Set an offset to be applied to all page numbers in the table of contents.
            // It can be useful when merging a PDF with a table of contents with other PDF documents
            htmlToPdfConverter.PdfDocumentOptions.TableOfContents.PageNumbersOffset = model.PageNumbersOffset;

            // Set the table of contents title
            htmlToPdfConverter.PdfDocumentOptions.TableOfContents.Title = model.TocTitle;

            // Optionally set a custom CSS style for the table of contents
            // A default style is applied by the library if this property is not set
            htmlToPdfConverter.PdfDocumentOptions.TableOfContents.Style = model.TocStyleTextBox;

            // Set HTML Viewer width in pixels which is the equivalent in converter of the browser window width
            htmlToPdfConverter.HtmlViewerWidth = model.HtmlViewerWidth;

            // Set the initial HTML viewer height in pixels
            if (model.HtmlViewerHeight.HasValue)
                htmlToPdfConverter.HtmlViewerHeight = model.HtmlViewerHeight.Value;

            // Set the HTML content zoom percentage similar to zoom level in a browser
            htmlToPdfConverter.HtmlViewerZoom = model.HtmlViewerZoom;

            // Automatically resize the PDF page width to match the HtmlViewerWidth property
            // The default value is true
            htmlToPdfConverter.PdfDocumentOptions.AutoResizePdfPageWidth = model.AutoResizePdfPageWidth;

            // Set the PDF page size, which can be a predefined size like A4 or a custom size in points
            // The default is A4
            // Important Note: The PDF page width is automatically determined from the HTML viewer width
            // when the AutoResizePdfPageWidth property is true
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = SelectedPdfPageSize(model.PdfPageSize);

            // Set the PDF page orientation to Portrait or Landscape. The default is Portrait
            htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = SelectedPdfPageOrientation(model.PdfPageOrientation);

            // Set the PDF page margins in points. The default is 0
            htmlToPdfConverter.PdfDocumentOptions.LeftMargin = model.LeftMargin;
            htmlToPdfConverter.PdfDocumentOptions.RightMargin = model.RightMargin;
            htmlToPdfConverter.PdfDocumentOptions.TopMargin = model.TopMargin;
            htmlToPdfConverter.PdfDocumentOptions.BottomMargin = model.BottomMargin;

            // Set the maximum time in seconds to wait for HTML page to be loaded 
            // Leave it not set for a default 120 seconds maximum wait time
            htmlToPdfConverter.NavigationTimeout = model.NavigationTimeout;

            // Set an additional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchronous operations to finish
            if (model.ConversionDelay.HasValue)
                htmlToPdfConverter.ConversionDelay = model.ConversionDelay.Value;

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
            fileResult.FileDownloadName = "Table_of_Contents.pdf";

            return fileResult;
        }

        private PdfPageSize SelectedPdfPageSize(string selectedValue)
        {
            switch (selectedValue)
            {
                case "A0":
                    return PdfPageSize.A0;
                case "A1":
                    return PdfPageSize.A1;
                case "A10":
                    return PdfPageSize.A10;
                case "A2":
                    return PdfPageSize.A2;
                case "A3":
                    return PdfPageSize.A3;
                case "A4":
                    return PdfPageSize.A4;
                case "A5":
                    return PdfPageSize.A5;
                case "A6":
                    return PdfPageSize.A6;
                case "A7":
                    return PdfPageSize.A7;
                case "A8":
                    return PdfPageSize.A8;
                case "A9":
                    return PdfPageSize.A9;
                case "ArchA":
                    return PdfPageSize.ArchA;
                case "ArchB":
                    return PdfPageSize.ArchB;
                case "ArchC":
                    return PdfPageSize.ArchC;
                case "ArchD":
                    return PdfPageSize.ArchD;
                case "ArchE":
                    return PdfPageSize.ArchE;
                case "B0":
                    return PdfPageSize.B0;
                case "B1":
                    return PdfPageSize.B1;
                case "B2":
                    return PdfPageSize.B2;
                case "B3":
                    return PdfPageSize.B3;
                case "B4":
                    return PdfPageSize.B4;
                case "B5":
                    return PdfPageSize.B5;
                case "Flsa":
                    return PdfPageSize.Flsa;
                case "HalfLetter":
                    return PdfPageSize.HalfLetter;
                case "Ledger":
                    return PdfPageSize.Ledger;
                case "Legal":
                    return PdfPageSize.Legal;
                case "Letter":
                    return PdfPageSize.Letter;
                case "Letter11x17":
                    return PdfPageSize.Letter11x17;
                case "Note":
                    return PdfPageSize.Note;
                default:
                    return PdfPageSize.A4;
            }
        }

        private PdfPageOrientation SelectedPdfPageOrientation(string selectedValue)
        {
            return selectedValue == "Portrait" ? PdfPageOrientation.Portrait : PdfPageOrientation.Landscape;
        }

        private Table_of_Contents_ViewModel SetViewModel()
        {
            var model = new Table_of_Contents_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Table_of_Contents".Length);

            model.HtmlStringTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Table_of_Contents.html"));
            model.TocStyleTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/TOC_Style.css"));
            model.BaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";

            return model;
        }
    }
}
