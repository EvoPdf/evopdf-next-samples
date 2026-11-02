using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.PDF_Viewer_Preferences
{
    public class Set_Viewer_PreferencesController : Controller
    {
        [HttpPost]
        public ActionResult ConvertHtmlToPdf(IFormCollection collection)
        {
            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            htmlToPdfConverter.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
            htmlToPdfConverter.ConversionDelay = 2;

            // Set the PDF Viewer Preferences

            // Set page layout to continuous one column, single page, two column left, two column right
            htmlToPdfConverter.PdfViewerPreferences.PageLayout = SelectedPageLayout(collection["pageLayoutComboBox"]);
            // Set page mode to default, display bookmarks, display thumbnails, display attachments
            htmlToPdfConverter.PdfViewerPreferences.PageMode = SelectedPageMode(collection["pageModeComboBox"]);

            // Hide the viewer menu
            htmlToPdfConverter.PdfViewerPreferences.HideMenuBar = collection["hideMenuBarCheckBox"].Count > 0;
            // Hide the viewer toolbar
            htmlToPdfConverter.PdfViewerPreferences.HideToolbar = collection["hideToolbarCheckBox"].Count > 0;
            // Hide scroll bars and navigation controls
            htmlToPdfConverter.PdfViewerPreferences.HideWindowUI = collection["hideWindowUICheckBox"].Count > 0;

            // Display the document title in viewer title bar
            htmlToPdfConverter.PdfViewerPreferences.DisplayDocTitle = collection["displayDocTitleCheckBox"].Count > 0;

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(collection["urlTextBox"]);
            
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