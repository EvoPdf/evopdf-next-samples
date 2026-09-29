using System;
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

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Set the initial HTML viewer height in pixels
            if (model.HtmlViewerHeight.HasValue)
                htmlToPdfConverter.HtmlViewerHeight = model.HtmlViewerHeight.Value;

            // Optionally load the lazy images
            htmlToPdfConverter.LoadLazyImages = model.LoadLazyImages;

            // Set the lazy images load mode
            htmlToPdfConverter.LazyImagesLoadMode = model.LazyImagesLoadMode == "Browser" ?
                LazyImagesLoadMode.Browser : LazyImagesLoadMode.Custom;

            // JavaScript in the converted page; some options of the converter turn it on when they need it

            htmlToPdfConverter.JavaScriptEnabled = model.JavaScriptEnabled;

            // Set the page layout: how the width at which the HTML is laid out relates to the PDF page width
            PdfPageSize pageSize = SelectedPdfPageSize(model.PdfPageSize);
            PdfPageOrientation pageOrientation = SelectedPdfPageOrientation(model.PdfPageOrientation);

            switch (model.PageLayout)
            {
                case "FitBrowserWindowToPage":
                    // Fixed page size: the HTML is laid out as in a browser window of the given width and the result
                    // is scaled to the content width of the page, so a responsive site keeps its desktop layout.
                    // This is the default layout of the converter, with an A4 page and a 1024 pixel window
                    htmlToPdfConverter.FitBrowserWindowToPage(pageSize, pageOrientation, windowWidth: model.HtmlViewerWidth, singlePage: model.SinglePage);
                    break;

                case "LayoutAtPageWidth":
                    // Fixed page size: the HTML is laid out at the content width of the page, one CSS pixel
                    // being 0.75 points. For HTML templates designed for the paper size
                    htmlToPdfConverter.LayoutAtPageWidth(pageSize, pageOrientation, zoom: model.HtmlViewerZoom, singlePage: model.SinglePage);
                    break;

                case "PrintLikeChrome":
                    // The output of the Save as PDF command of Chrome: the print media type, 1 cm margins, no
                    // background colors or images, drawn at the zoom; the margins and the backgrounds set below
                    // replace the ones of Chrome when they were changed in the form
                    htmlToPdfConverter.PrintLikeChrome(pageSize, pageOrientation, zoom: model.HtmlViewerZoom, singlePage: model.SinglePage);
                    break;

                default:
                    // The PDF page width follows the browser window width and the HTML is drawn at the zoom, 1:1 at 100;
                    // the page height comes from the page size and the orientation
                    htmlToPdfConverter.PageWidthFromBrowserWindow(model.HtmlViewerWidth, singlePage: model.SinglePage, zoom: model.HtmlViewerZoom);
                    htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = pageSize;
                    htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = pageOrientation;
                    break;
            }

            // The page margins in points, after the layout, so that they replace the ones a layout sets. The default is 0

            htmlToPdfConverter.PdfDocumentOptions.LeftMargin = model.LeftMargin;

            htmlToPdfConverter.PdfDocumentOptions.RightMargin = model.RightMargin;

            htmlToPdfConverter.PdfDocumentOptions.TopMargin = model.TopMargin;

            htmlToPdfConverter.PdfDocumentOptions.BottomMargin = model.BottomMargin;

            // The background colors and images of the HTML, printed or not
            htmlToPdfConverter.PdfDocumentOptions.PrintBackgrounds = model.PrintBackgrounds;

            // The media type used in @media rules, after the layout, so that it replaces the one a layout sets
            htmlToPdfConverter.MediaType = model.MediaType == "Print" ? "print" : "screen";

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
            // The zoom the HTML was drawn at, read from the PDF: the zoom of the layout, lower when the browser window
            // grew to the content; in the name of the file
            string printZoom = htmlToPdfConverter.ConversionInfo.PrintZoom.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture);

            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            if (!model.OpenInline)
            {
                // send as attachment
                fileResult.FileDownloadName = "HTML_to_PDF_Getting_Started_zoom_" + printZoom + ".pdf";
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
