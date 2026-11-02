using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using System.Drawing;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.PDF_Forms
{
    public class Define_in_HTML_PDF_Form_FieldsController : Controller
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

            // Set the submit buttons style
            htmlToPdfConverter.PdfFormOptions.SubmitButtonStyle.BackColor = Color.Beige;

            // Set the style of various types of text boxes
            htmlToPdfConverter.PdfFormOptions.TextBoxStyle.BackColor = Color.AliceBlue;
            htmlToPdfConverter.PdfFormOptions.PasswordTextBoxStyle.BackColor = Color.MistyRose;
            htmlToPdfConverter.PdfFormOptions.MultilineTextBoxStyle.BackColor = Color.AliceBlue;

            // Set the radio buttons style
            htmlToPdfConverter.PdfFormOptions.RadioButtonsGroupStyle.BackColor = Color.AntiqueWhite;

            // Set the checkboxes styles
            htmlToPdfConverter.PdfFormOptions.CheckBoxStyle.BackColor = Color.AntiqueWhite;

            // set the drop down lists style
            htmlToPdfConverter.PdfFormOptions.ComboBoxStyle.BackColor = Color.LightCyan;

            byte[] outPdfBuffer = null;

            if (collection["HtmlPageSource"] == "convertHtmlRadioButton")
            {
                // Convert a HTML string to a PDF document with form fields
                string htmlWithForm = collection["htmlStringTextBox"];
                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlWithForm, String.Empty);
            }
            else
            {
                // Convert the HTML page to a PDF document with form fields
                string url = collection["urlTextBox"];
                outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
            }
            
            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Define_in_HTML_PDF_Form_Fields.pdf";

            return fileResult;
        }
    }
}