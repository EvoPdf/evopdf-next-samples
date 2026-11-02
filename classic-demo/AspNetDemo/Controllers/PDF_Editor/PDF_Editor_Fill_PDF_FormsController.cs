using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Editor
{
    public class PDF_Editor_Fill_PDF_FormsController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public PDF_Editor_Fill_PDF_FormsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        private void SetCurrentViewData()
        {
            ViewData["ContentRootPath"] = m_hostingEnvironment.ContentRootPath + "/wwwroot";

            HttpRequest request = this.ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            ViewData["CurrentPageUrl"] = uriBuilder.Uri.AbsoluteUri;
        }

        // GET: PDF_Editor_Fill_PDF_Forms
        public ActionResult Index()
        {
            SetCurrentViewData();

            return View();
        }

        [HttpPost]
        public ActionResult FillPdf(IFormCollection collection)
        {
            Document pdfDocument = null;
            try
            {
                // Load the PDF document with a form to fill
                string pdfFileWithForm = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/PDF_Files/Fill_PDF_Forms.pdf";
                pdfDocument = new Document(pdfFileWithForm);

                // Set license key received after purchase to use the converter in licensed mode
                // Leave it not set to use the converter in demo mode
                pdfDocument.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

                // Get the First Name text box field by name from PDF form fields collection and fill a text value
                PdfFormField firstNameTextBoxField = pdfDocument.Form.Fields["firstName"];
                if (firstNameTextBoxField != null)
                    firstNameTextBoxField.Value = collection["firstNameTextBox"][0];

                // Get the Last Name text box field by name from PDF form fields collection and fill a text value
                PdfFormField lastNameTextBoxField = pdfDocument.Form.Fields["lastName"];
                if (lastNameTextBoxField != null)
                    lastNameTextBoxField.Value = collection["lastNameTextBox"][0];

                // Get the Password text box field by name from PDF form fields collection and fill a text value
                PdfFormField passwordTextBoxField = pdfDocument.Form.Fields["password"];
                if (passwordTextBoxField != null)
                    passwordTextBoxField.Value = collection["passwordTextBox"][0];

                // Get the Gender radio buttons group field by name from form fields collection and set the selected value
                PdfFormField genderRadioGroupField = pdfDocument.Form.Fields["gender"];
                if (genderRadioGroupField != null)
                    genderRadioGroupField.Value = collection["Gender"] == "maleRadioButton" ? "male" : "female";

                // Get the 'I have a car' check box field by name from form fields collection and set the checked status
                PdfFormField haveCarCheckBoxField = pdfDocument.Form.Fields["haveCar"];
                if (haveCarCheckBoxField != null)
                    haveCarCheckBoxField.Value = collection["haveCarCheckBox"].Count > 0;

                // Get the 'Vehicle Type' combo box field by name from form fields collection and set the selected value
                PdfFormField vehicleTypeComboBoxField = pdfDocument.Form.Fields["vehicleType"];
                if (vehicleTypeComboBoxField != null)
                    vehicleTypeComboBoxField.Value = collection["carTypeComboBox"][0];

                // Get the Comments multi line text box field by name from PDF form fields collection and fill a text value
                PdfFormField commentsTextBoxField = pdfDocument.Form.Fields["comments"];
                if (commentsTextBoxField != null)
                    commentsTextBoxField.Value = collection["commentsTextBox"][0];

                // Flatten PDF form fields if this was requested
                if (collection["flattenPdfFormCheckBox"].Count > 0)
                    pdfDocument.Form.FlattenFields();

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Filled_PDF_Form.pdf";

                return fileResult;
            }
            finally
            {
                // Close the document to stamp
                if (pdfDocument != null)
                    pdfDocument.Close();
            }
        }
    }
}