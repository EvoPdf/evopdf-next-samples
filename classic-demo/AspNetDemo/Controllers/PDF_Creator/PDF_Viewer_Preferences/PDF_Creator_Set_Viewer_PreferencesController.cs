using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator.PDF_Viewer_Preferences
{
    public class PDF_Creator_Set_Viewer_PreferencesController : Controller
    {
        [HttpPost]
        public ActionResult CreatePdf(IFormCollection collection)
        {
            // Create a PDF document
            Document pdfDocument = new Document();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            pdfDocument.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Add a page to PDF document
            PdfPage pdfPage = pdfDocument.AddPage();

            // Set the PDF Viewer Preferences

            // Set page layout to continuous one column, single page, two column left, two column right
            pdfDocument.ViewerPreferences.PageLayout = SelectedPageLayout(collection["pageLayoutComboBox"]);
            // Set page mode to default, display attachments, display thumbnails, display attachments
            pdfDocument.ViewerPreferences.PageMode = SelectedPageMode(collection["pageModeComboBox"]);

            // Hide the viewer menu
            pdfDocument.ViewerPreferences.HideMenuBar = collection["hideMenuBarCheckBox"].Count > 0;
            // Hide the viewer toolbar
            pdfDocument.ViewerPreferences.HideToolbar = collection["hideToolbarCheckBox"].Count > 0;
            // Hide scroll bars and navigation controls
            pdfDocument.ViewerPreferences.HideWindowUI = collection["hideWindowUICheckBox"].Count > 0;

            // Display the document title in viewer title bar
            pdfDocument.ViewerPreferences.DisplayDocTitle = collection["displayDocTitleCheckBox"].Count > 0;

            try
            {
                // Create a HTML to PDF element to add to document
                HtmlToPdfElement htmlToPdfElement = new HtmlToPdfElement(collection["urlTextBox"]);

                // Optionally set a delay before conversion to allow asynchonous scripts to finish
                htmlToPdfElement.ConversionDelay = 2;

                // Add the HTML to PDF element to document
                pdfPage.AddElement(htmlToPdfElement);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Set_PDF_Viewer_Preferences.pdf";

                return fileResult;
            }
            finally
            {
                // Close the PDF document
                pdfDocument.Close();
            }
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