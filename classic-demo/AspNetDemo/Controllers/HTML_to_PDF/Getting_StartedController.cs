using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers
{
    public class Getting_StartedController : Controller
    {
        // GET: Getting_Started
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(IFormCollection collection)
        {
            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            htmlToPdfConverter.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Set HTML Viewer width in pixels which is the equivalent in converter of the browser window width
            htmlToPdfConverter.HtmlViewerWidth = int.Parse(collection["htmlViewerWidthTextBox"]);

            // Set HTML viewer height in pixels to convert the top part of a HTML page 
            // Leave it not set to convert the entire HTML
            if (collection["htmlViewerHeightTextBox"][0].Length > 0)
                htmlToPdfConverter.HtmlViewerHeight = int.Parse(collection["htmlViewerHeightTextBox"]);

            // Set PDF page size which can be a predefined size like A4 or a custom size in points 
            // Leave it not set to have a default A4 PDF page
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = SelectedPdfPageSize(collection["pdfPageSizeDropDownList"]);

            // Set PDF page orientation to Portrait or Landscape
            // Leave it not set to have a default Portrait orientation for PDF page
            htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = SelectedPdfPageOrientation(collection["pdfPageOrientationDropDownList"]);

            // Set the maximum time in seconds to wait for HTML page to be loaded 
            // Leave it not set for a default 60 seconds maximum wait time
            htmlToPdfConverter.NavigationTimeout = int.Parse(collection["navigationTimeoutTextBox"]);

            // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
            if (collection["conversionDelayTextBox"][0].Length > 0)
                htmlToPdfConverter.ConversionDelay = int.Parse(collection["conversionDelayTextBox"]);

            // The classic converter runs over an EvoPdf.Next converter, created for each conversion and exposed by
            // the Next property. The NextConverterCreated event sets on it the options that have no classic
            // counterpart, here the PDF standards beyond PDF/A-1b: PDF/A-2, PDF/A-3, PDF/A-4, PDF/UA and WTPDF
            // Leave the standard not set to have a regular PDF document
            EvoPdf.Next.PdfStandard pdfStandard = SelectedPdfStandard(collection["pdfStandardDropDownList"]);
            if (pdfStandard != EvoPdf.Next.PdfStandard.None)
                htmlToPdfConverter.NextConverterCreated += nextConverter => nextConverter.PdfDocumentOptions.PdfStandard = pdfStandard;

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = null;

            if (collection["HtmlPageSource"] == "convertUrlRadioButton")
            {

                string url = collection["urlTextBox"];

                // Convert the HTML page given by an URL to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
            }
            else
            {
                string htmlString = collection["htmlStringTextBox"];
                string baseUrl = collection["baseUrlTextBox"];

                // Convert a HTML string with a base URL to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlString, baseUrl);
            }

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            if (collection["openInlineCheckBox"].Count == 0)
            {
                // send as attachment
                fileResult.FileDownloadName = "Getting_Started.pdf";
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
            return (selectedValue == "Portrait") ? PdfPageOrientation.Portrait : PdfPageOrientation.Landscape;
        }

        private EvoPdf.Next.PdfStandard SelectedPdfStandard(string selectedValue)
        {
            EvoPdf.Next.PdfStandard pdfStandard;
            return Enum.TryParse(selectedValue, out pdfStandard) ? pdfStandard : EvoPdf.Next.PdfStandard.None;
        }
    }
}