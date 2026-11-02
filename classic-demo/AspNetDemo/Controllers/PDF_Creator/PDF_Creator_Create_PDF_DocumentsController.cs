using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator
{
    public class PDF_Creator_Create_PDF_DocumentsController : Controller
    {
        // GET: PDF_Creator_Create_PDF_Documents
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePdf(IFormCollection collection)
        {
            // Get the PDF Standard
            // By default the Full PDF standard is used
            PdfStandardSubset pdfStandard = PdfStandardSubset.Full;
            if (collection["PdfStandard"] == "pdfARadioButton")
                pdfStandard = PdfStandardSubset.Pdf_A_1b;

            // Create the PDF document with the selected standard
            Document pdfDocument = pdfStandard == PdfStandardSubset.Full ? new Document() : new Document(pdfStandard);

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            pdfDocument.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Get the selected PDF page size
            PdfPageSize pdfPageSize = SelectedPdfPageSize(collection["pdfPageSizeDropDownList"]);

            // Get the selected PDF page orientation
            PdfPageOrientation pdfPageOrientation = SelectedPdfPageOrientation(collection["pdfPageOrientationDropDownList"]);

            // Get the PDF page margins
            Margins pdfPageMargins = new Margins(float.Parse(collection["leftMarginTextBox"]), float.Parse(collection["rightMarginTextBox"]),
                    float.Parse(collection["topMarginTextBox"]), float.Parse(collection["bottomMarginTextBox"]));

            // Create a PDF page in PDF document
            PdfPage firstPdfPage = pdfDocument.AddPage(pdfPageSize, pdfPageMargins, pdfPageOrientation);

            try
            {
                // The URL of the HTML page to convert to PDF
                string urlToConvert = "http://www.evopdf.com";

                // Create the HTML to PDF element
                HtmlToPdfElement htmlToPdfElement = new HtmlToPdfElement(urlToConvert);

                // Optionally set a delay before conversion to allow asynchonous scripts to finish
                htmlToPdfElement.ConversionDelay = 2;

                // Add the HTML to PDF element to PDF document
                firstPdfPage.AddElement(htmlToPdfElement);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Create_PDF_Documents.pdf";

                return fileResult;
            }
            finally
            {
                // Close the PDF document
                pdfDocument.Close();
            }            
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
            return (selectedValue == "Portrait") ? PdfPageOrientation.Portrait : PdfPageOrientation.Landscape;
        }
    }
}