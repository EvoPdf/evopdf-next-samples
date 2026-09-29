using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Convert_Multiple_HTML_to_PDF_in_ParallelController : Controller
    {
        public IActionResult Index()
        {
            var model = new Convert_Multiple_HTML_to_PDF_in_Parallel_ViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ConvertToPdfInParallel(Convert_Multiple_HTML_to_PDF_in_Parallel_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // Create PDF merger
            using PdfMerge pdfMerge = new PdfMerge();

            // Set PDF merge options
            SetPdfMergeOptions(pdfMerge, model);

            // Create the list of URLs to convert
            List<string> urlsToConvert = new List<string>() { 
                model.FirstUrl, 
                model.SecondUrl
            };

            if (model.AsyncParallelConversionEnabled)
            {
                // Parallel conversion using asynchronous methods to convert HTML to PDF

                // List of tasks that will run in parallel
                List<Task<byte[]>> conversionTasks = new List<Task<byte[]>>();

                foreach (string url in urlsToConvert)
                {
                    // Create the HTML to PDF Converter
                    HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

                    // Set the HTML to PDF Converter options
                    SetHtmlToPdfConverterOptions(htmlToPdfConverter, model);

                    // Start async conversion
                    Task<byte[]> convertTask = htmlToPdfConverter.ConvertUrlAsync(url);
                    conversionTasks.Add(convertTask);
                }

                // Wait for all conversions to finish
                byte[][] pdfBytesArray = await Task.WhenAll(conversionTasks);

                // Add PDFs in the original order
                foreach (byte[] pdfBytes in pdfBytesArray)
                {
                    pdfMerge.AddPdf(pdfBytes);
                }
            }
            else
            {
                foreach (string url in urlsToConvert)
                {
                    // Create the HTML to PDF Converter
                    HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

                    // Set the HTML to PDF Converter options
                    SetHtmlToPdfConverterOptions(htmlToPdfConverter, model);

                    // Convert HTML to PDF
                    byte[] pdfBytes = htmlToPdfConverter.ConvertUrl(url);

                    // Add the PDF to the merger
                    int pdfPageCount = pdfMerge.AddPdf(pdfBytes);
                }
            }

            // Merge the PDF documents
            byte[] mergedPdf = pdfMerge.Save();

            // Send the merged PDF file to browser
            FileResult fileResult = new FileContentResult(mergedPdf, "application/pdf");
            fileResult.FileDownloadName = "Convert_Multipe_HTML_in_Parallel.pdf";

            return fileResult;
        }

        private void SetHtmlToPdfConverterOptions(HtmlToPdfConverter htmlToPdfConverter, Convert_Multiple_HTML_to_PDF_in_Parallel_ViewModel model)
        {
            // Set the initial HTML viewer height in pixels
            if (model.HtmlViewerHeight.HasValue)
                htmlToPdfConverter.HtmlViewerHeight = model.HtmlViewerHeight.Value;

            // Set the PDF page margins in points. The default is 0
            htmlToPdfConverter.PdfDocumentOptions.LeftMargin = model.LeftMargin;
            htmlToPdfConverter.PdfDocumentOptions.RightMargin = model.RightMargin;
            htmlToPdfConverter.PdfDocumentOptions.TopMargin = model.TopMargin;
            htmlToPdfConverter.PdfDocumentOptions.BottomMargin = model.BottomMargin;

            // Set the page layout: how the width at which the HTML is laid out relates to the PDF page width
            PdfPageSize pageSize = SelectedPdfPageSize(model.PdfPageSize);
            PdfPageOrientation pageOrientation = SelectedPdfPageOrientation(model.PdfPageOrientation);

            switch (model.PageLayout)
            {
                case "FitBrowserWindowToPage":
                    // Fixed page size: the HTML is laid out as in a browser window of the given width and the result
                    // is scaled to the content width of the page, so a responsive site keeps its desktop layout.
                    // This is the default layout of the converter, with an A4 page and a 1024 pixel window
                    htmlToPdfConverter.FitBrowserWindowToPage(pageSize, pageOrientation, windowWidth: model.HtmlViewerWidth);
                    break;

                case "LayoutAtPageWidth":
                    // Fixed page size: the HTML is laid out at the content width of the page, one CSS pixel
                    // being 0.75 points. For HTML templates designed for the paper size
                    htmlToPdfConverter.LayoutAtPageWidth(pageSize, pageOrientation);
                    break;

                default:
                    // The PDF page width follows the browser window width and the HTML is drawn 1:1;
                    // the page height comes from the page size and the orientation
                    htmlToPdfConverter.PageWidthFromBrowserWindow(model.HtmlViewerWidth, singlePage: false, zoom: model.HtmlViewerZoom);
                    htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = pageSize;
                    htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = pageOrientation;
                    break;
            }

            // Enable the creation of a hierarchy of bookmarks from H1 to H6 tags
            htmlToPdfConverter.PdfDocumentOptions.GenerateDocumentOutline = model.AutoBookmarks;

            // Set the maximum time in seconds to wait for HTML page to be loaded 
            // Leave it not set for a default 120 seconds maximum wait time
            htmlToPdfConverter.NavigationTimeout = model.NavigationTimeout;

            // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
            if (model.ConversionDelay.HasValue)
                htmlToPdfConverter.ConversionDelay = model.ConversionDelay.Value;
        }

        private void SetPdfMergeOptions(PdfMerge pdfMerge, Convert_Multiple_HTML_to_PDF_in_Parallel_ViewModel model)
        {
            // Set the encryption algorithm and the encryption key size if they are not the default ones
            if (model.EncryptionKey != "Bit128" || model.EncryptionType != "RC4")
            {
                // set the encryption algorithm
                pdfMerge.PdfSecurityOptions.EncryptionAlgorithm = model.EncryptionType == "RC4" ? EncryptionAlgorithm.RC4 : EncryptionAlgorithm.AES;

                // set the encryption key size
                if (model.EncryptionKey == "Bit40")
                    pdfMerge.PdfSecurityOptions.KeySize = EncryptionKeySize.EncryptKey40Bit;
                else if (model.EncryptionKey == "Bit128")
                    pdfMerge.PdfSecurityOptions.KeySize = EncryptionKeySize.EncryptKey128Bit;
                else if (model.EncryptionKey == "Bit256")
                    pdfMerge.PdfSecurityOptions.KeySize = EncryptionKeySize.EncryptKey256Bit;
            }

            // Set user and owner passwords
            if (!string.IsNullOrEmpty(model.UserPassword))
                pdfMerge.PdfSecurityOptions.UserPassword = model.UserPassword;

            if (!string.IsNullOrEmpty(model.OwnerPassword))
                pdfMerge.PdfSecurityOptions.OwnerPassword = model.OwnerPassword;

            // Set PDF document permissions
            pdfMerge.PdfSecurityOptions.CanPrint = model.PrintEnabled;
            pdfMerge.PdfSecurityOptions.CanCopyContent = model.CopyContentEnabled;
            pdfMerge.PdfSecurityOptions.CanCopyAccessibilityContent = model.CopyAccessibilityContentEnabled;
            pdfMerge.PdfSecurityOptions.CanEditContent = model.EditContentEnabled;
            pdfMerge.PdfSecurityOptions.CanEditAnnotations = model.EditAnnotationsEnabled;
            pdfMerge.PdfSecurityOptions.CanFillFormFields = model.FillFormFieldsEnabled;

            if ((PermissionsChanged(pdfMerge) || pdfMerge.PdfSecurityOptions.UserPassword.Length > 0) &&
                pdfMerge.PdfSecurityOptions.OwnerPassword.Length == 0)
            {
                // A user password is set but the owner password is not set or the permissions are not the default ones
                // Set a default owner password
                pdfMerge.PdfSecurityOptions.OwnerPassword = "owner";
            }
        }

        private bool PermissionsChanged(PdfMerge pdfMerge)
        {
            return !pdfMerge.PdfSecurityOptions.CanPrint ||
                    !pdfMerge.PdfSecurityOptions.CanCopyContent || !pdfMerge.PdfSecurityOptions.CanCopyAccessibilityContent ||
                    !pdfMerge.PdfSecurityOptions.CanEditContent || !pdfMerge.PdfSecurityOptions.CanEditAnnotations ||
                    !pdfMerge.PdfSecurityOptions.CanFillFormFields;
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
