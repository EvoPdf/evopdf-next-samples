// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/set-pdf-viewer-preferences.htm
// Documentation page: Set PDF Viewer Preferences for the Generated PDF Document

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class PDF_Viewer_PreferencesController : Controller
    {
        public ActionResult Index()
        {
            var model = new PDF_Viewer_Preferences_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(PDF_Viewer_Preferences_ViewModel model)
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

            // Set the PDF Viewer Preferences

            // Set page layout to continuous one column, single page, two column left, two column right
            htmlToPdfConverter.PdfViewerPreferences.PageLayout = SelectedPageLayout(model.PageLayout);
            // Set page mode to default, display bookmarks, display thumbnails, display attachments
            htmlToPdfConverter.PdfViewerPreferences.PageMode = SelectedPageMode(model.PageMode);

            // Hide the viewer menu
            htmlToPdfConverter.PdfViewerPreferences.HideMenuBar = model.HideMenuBar;
            // Hide the viewer toolbar
            htmlToPdfConverter.PdfViewerPreferences.HideToolbar = model.HideToolbar;
            // Hide scroll bars and navigation controls
            htmlToPdfConverter.PdfViewerPreferences.HideWindowUI = model.HideWindowUI;

            // Display the document title in viewer title bar
            htmlToPdfConverter.PdfViewerPreferences.DisplayDocTitle = model.DisplayDocTitle;

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(model.Url);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Set_PDF_Viewer_Preferences.pdf";

            return fileResult;
        }

        private ViewerPageLayout SelectedPageLayout(string selectedValue)
        {
            switch (selectedValue)
            {
                case "Single Page":
                    return ViewerPageLayout.SinglePage;
                case "One Column":
                    return ViewerPageLayout.OneColumn;
                case "Two Column Left":
                    return ViewerPageLayout.TwoColumnLeft;
                case "Two Column Right":
                    return ViewerPageLayout.TwoColumnRight;
                default:
                    return ViewerPageLayout.OneColumn;
            }
        }

        private ViewerPageMode SelectedPageMode(string selectedValue)
        {
            switch (selectedValue)
            {
                case "Default":
                    return ViewerPageMode.UseNone;
                case "Display Outlines":
                    return ViewerPageMode.UseOutlines;
                case "Display Thumbnails":
                    return ViewerPageMode.UseThumbs;
                case "Display Full Screen":
                    return ViewerPageMode.FullScreen;
                case "Display Optional Content Group":
                    return ViewerPageMode.UseOC;
                case "Display Attachments":
                    return ViewerPageMode.UseAttachments;
                default:
                    return ViewerPageMode.UseNone;
            }
        }
    }
}
