using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using System.Drawing;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator
{
    public class PDF_Creator_BookmarksController : Controller
    {
        // GET: PDF_Creator_Bookmarks
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePdf(IFormCollection collection)
        {
            // Create a PDF document
            Document pdfDocument = new Document();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            pdfDocument.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Display the bookmarks panel when the PDF document is opened in a PDF viewer
            pdfDocument.ViewerPreferences.PageMode = ViewerPageMode.UseOutlines;

            try
            {
                // The titles font used to mark various sections of the PDF document
                PdfFont titleFont = pdfDocument.AddFont(new Font("Times New Roman", 12, FontStyle.Regular, GraphicsUnit.Point));

                // Add a new PDF page to PDF document
                PdfPage page1 = pdfDocument.AddPage();
                TextElement pageText = new TextElement(0, 0, "Page 1. Destination of a Top Bookmark with Fit Width View Mode.", titleFont);
                page1.AddElement(pageText);

                // Add a new PDF page to PDF document
                PdfPage page2 = pdfDocument.AddPage();
                pageText = new TextElement(0, 0, "Page 2. Destination of a Top Bookmark with Custom Zoom Level.", titleFont);
                page2.AddElement(pageText);

                // Add a new PDF page to PDF document
                PdfPage page3 = pdfDocument.AddPage();
                pageText = new TextElement(0, 0, "Page 3. Destination of a Child Bookmark with Fit Width and Height View Mode.", titleFont);
                page3.AddElement(pageText);

                // Add a new PDF page to PDF document
                PdfPage page4 = pdfDocument.AddPage();
                pageText = new TextElement(0, page4.PageSize.Height / 2 - 20, "Page 4. Destination of a Top Bookmark for the Middle of the Page.", titleFont);
                page4.AddElement(pageText);

                // Add a new PDF page to PDF document
                PdfPage page5 = pdfDocument.AddPage();
                pageText = new TextElement(0, 0, "Page 5. Destination of a Child Bookmark with Colored Title.", titleFont);
                page5.AddElement(pageText);

                // Add a new PDF page to PDF document
                PdfPage page6 = pdfDocument.AddPage();
                pageText = new TextElement(0, 0, "Page 6. Destination of a Child Bookmark with Italic Style Title.", titleFont);
                page6.AddElement(pageText);

                // Add a top level bookmark for first page setting destination view mode to fit viewer window horizontally
                ExplicitDestination page1Destination = new ExplicitDestination(page1, new PointF(0, 0), DestinationViewMode.FitH);
                Bookmark page1TopBookmark = pdfDocument.Bookmarks.AddNewBookmark("Top Bookmark with Fit Width View Mode", page1Destination);
                page1TopBookmark.Style = PdfBookmarkStyle.Bold;

                // Add a top level bookmark for second page setting the zoom level to 125%
                ExplicitDestination page2Destination = new ExplicitDestination(page2, new PointF(0, 0), DestinationViewMode.XYZ);
                page2Destination.ZoomPercentage = 125;
                Bookmark page2TopBookmark = pdfDocument.Bookmarks.AddNewBookmark("Top Bookmark with Custom Zoom Level", page2Destination);
                page2TopBookmark.Style = PdfBookmarkStyle.Normal;

                // Add a child bookmark for third page setting destination view mode to fit viewer window horizontally and vertically
                ExplicitDestination page3Destination = new ExplicitDestination(page3, new PointF(0, 0), DestinationViewMode.Fit);
                Bookmark page3ChildBookmark = page2TopBookmark.DescendantBookmarks.AddNewBookmark("Child Bookmark with Fit Width and Height View Mode", page3Destination);

                // Add a top level bookmark for fourth page with destination point in the middle of the PDF page 
                ExplicitDestination page4Destination = new ExplicitDestination(page4, new PointF(0, page4.PageSize.Height / 2 - 20));
                Bookmark page4TopBookmark = pdfDocument.Bookmarks.AddNewBookmark("Top Bookmark for the Middle of the Page", page4Destination);
                page4TopBookmark.Style = PdfBookmarkStyle.Bold;
                page4TopBookmark.Color = Color.Blue;

                // Add a child bookmark with colored text
                ExplicitDestination page5Destination = new ExplicitDestination(page5, new PointF(0, 0));
                Bookmark page5ChildBookmark = page4TopBookmark.DescendantBookmarks.AddNewBookmark("Child Bookmark with Colored Title", page5Destination);
                page5ChildBookmark.Color = Color.Red;

                // Add a child bookmark with italic style text
                ExplicitDestination page6Destination = new ExplicitDestination(page6, new PointF(0, 0));
                Bookmark page6ChildBookmark = page4TopBookmark.DescendantBookmarks.AddNewBookmark("Child Bookmark with Italic Colored Title", page6Destination);
                page6ChildBookmark.Style = PdfBookmarkStyle.Italic;
                page6ChildBookmark.Color = Color.Green;

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();

                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Bookmarks.pdf";

                return fileResult;
            }
            finally
            {
                // Close the PDF document
                pdfDocument.Close();
            }
        }
    }
}