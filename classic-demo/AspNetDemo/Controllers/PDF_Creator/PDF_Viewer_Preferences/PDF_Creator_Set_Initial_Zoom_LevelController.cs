using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using System.Drawing;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator.PDF_Viewer_Preferences
{
    public class PDF_Creator_Set_Initial_Zoom_LevelController : Controller
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

            try
            {
                // Create a HTML to PDF element to add to document
                HtmlToPdfElement htmlToPdfElement = new HtmlToPdfElement(collection["urlTextBox"]);

                // Optionally set a delay before conversion to allow asynchonous scripts to finish
                htmlToPdfElement.ConversionDelay = 2;

                // Add the HTML to PDF element to document
                pdfPage.AddElement(htmlToPdfElement);

                int goToPageNumber = int.Parse(collection["pageNumberTextBox"]);
                if (goToPageNumber > pdfDocument.Pages.Count)
                {
                    goToPageNumber = 1;
                }

                // Get destination PDF page
                PdfPage goToPage = pdfDocument.Pages[goToPageNumber - 1];

                // Get the destination point in PDF page
                float goToX = float.Parse(collection["xLocationTextBox"]);
                float goToY = float.Parse(collection["yLocationTextBox"]);

                PointF goToLocation = new PointF(goToX, goToY);

                // Get the destination view mode
                DestinationViewMode viewMode = SelectedViewMode(collection["viewModeComboBox"]);

                // Create the destination in PDF document
                ExplicitDestination goToDestination = new ExplicitDestination(goToPage, goToLocation, viewMode);

                // Set the zoom level when the destination is displayed
                if (viewMode == DestinationViewMode.XYZ)
                    goToDestination.ZoomPercentage = int.Parse(collection["zoomLevelTextBox"]);

                // Set the document Go To open action
                pdfDocument.OpenAction.Action = new PdfActionGoTo(goToDestination);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Set_Initial_Zoom_Level.pdf";

                return fileResult;
            }
            finally
            {
                // Close the PDF document
                pdfDocument.Close();
            }
        }

        private DestinationViewMode SelectedViewMode(string selectedValue)
        {
            switch (selectedValue)
            {
                case "X, Y and Zoom":
                    return DestinationViewMode.XYZ;
                case "Fit Window":
                    return DestinationViewMode.Fit;
                case "Fit Horizontally":
                    return DestinationViewMode.FitH;
                case "Fit Vertically":
                    return DestinationViewMode.FitV;
                default:
                    return DestinationViewMode.XYZ;
            }
        }
    }
}