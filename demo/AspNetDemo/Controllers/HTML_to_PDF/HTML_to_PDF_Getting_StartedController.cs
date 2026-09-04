using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class HTML_to_PDF_Getting_StartedController : Controller
    {
        // GET: Getting_Started
        public ActionResult Index()
        {
            var model = new HTML_to_PDF_Getting_Started_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(HTML_to_PDF_Getting_Started_ViewModel model)
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

            // Set HTML Viewer width in pixels which is the equivalent in converter of the browser window width
            htmlToPdfConverter.HtmlViewerWidth = model.HtmlViewerWidth;

            // Set the initial HTML viewer height in pixels
            if (model.HtmlViewerHeight.HasValue)
                htmlToPdfConverter.HtmlViewerHeight = model.HtmlViewerHeight.Value;

            // Set the HTML content zoom percentage similar to zoom level in a browser
            htmlToPdfConverter.HtmlViewerZoom = model.HtmlViewerZoom;

            // Optionally load the lazy images
            htmlToPdfConverter.LoadLazyImages = model.LoadLazyImages;

            // Set the lazy images load mode
            htmlToPdfConverter.LazyImagesLoadMode = model.LazyImagesLoadMode == "Browser" ?
                LazyImagesLoadMode.Browser : LazyImagesLoadMode.Custom;

            // Set the media type used in @media rules when rendering HTML to PDF
            htmlToPdfConverter.MediaType = model.MediaType == "Print" ? "print" : "screen";

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

            // Sets the PDF standard for the generated document
            // Leave as None to generate a plain PDF without an accessibility structure tree or archival metadata
            htmlToPdfConverter.PdfDocumentOptions.PdfStandard = model.PdfStandard;

            // Set the maximum time, in seconds, to wait for the HTML page to load
            // The default value is 120 seconds
            htmlToPdfConverter.NavigationTimeout = model.NavigationTimeout;

            // Set an additional delay, in seconds, to wait for asynchronous content after the initial load
            // The default value is 0
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
                string htmlString = model.HtmlString;
                string baseUrl = model.BaseUrl;

                // Convert a HTML string with a base URL to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlString, baseUrl);
            }

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            if (!model.OpenInline)
            {
                // send as attachment
                fileResult.FileDownloadName = "HTML_to_PDF_Getting_Started.pdf";
            }

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
    }
}