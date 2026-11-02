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
    public class PDF_Creator_PDF_FormsController : Controller
    {
        // GET: PDF_Creator_PDF_Forms
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

            // Add a page to PDF document
            PdfPage pdfPage = pdfDocument.AddPage();

            try
            {
                // The font used for titles in PDF document
                PdfFont titlesFont = pdfDocument.AddFont(new Font("Times New Roman", 10, FontStyle.Bold, GraphicsUnit.Point));
                // The font used for field names in PDF document
                PdfFont fieldNameFont = pdfDocument.AddFont(new Font("Times New Roman", 10, FontStyle.Regular, GraphicsUnit.Point));
                // The font used for buttons text in PDF document
                PdfFont buttonTextFont = pdfDocument.AddFont(new Font("Times New Roman", 10, FontStyle.Regular, GraphicsUnit.Point));
                // The font used for PDF form text box fields
                PdfFont textFieldFont = pdfDocument.AddFont(StdFontBaseFamily.Helvetica);
                textFieldFont.Size = 8;
                // The font used for PDF form combo box fields
                PdfFont comboBoxFieldFont = pdfDocument.AddFont(StdFontBaseFamily.Helvetica);
                comboBoxFieldFont.Size = 8;

                float xLocation = 5;
                float yLocation = 5;

                // Add document title
                TextElement titleTextElement = new TextElement(xLocation, yLocation, "Create PDF Forms", titlesFont);
                AddElementResult addElementResult = pdfPage.AddElement(titleTextElement);

                yLocation = addElementResult.EndPageBounds.Bottom + 15;

                // Add a text box field to PDF form
                TextElement fieldNameTextElement = new TextElement(xLocation, yLocation, 60, "First name:", fieldNameFont);
                addElementResult = pdfPage.AddElement(fieldNameTextElement);
                RectangleF fieldNameRectangle = addElementResult.EndPageBounds;
                RectangleF fieldBoundingRectangle = new RectangleF(fieldNameRectangle.Right + 10, yLocation, 150, 15);
                // Create the form field
                PdfFormTextBox textBoxField = pdfDocument.Form.AddTextBox(pdfPage, fieldBoundingRectangle, "Enter First Name", textFieldFont);
                // Set unique form field name used when the form is submitted
                textBoxField.Name = "firstName";
                // Set the form field default value
                textBoxField.DefaultValue = "A default first name";
                // Set form field style 
                textBoxField.Style.BackColor = Color.AliceBlue;

                yLocation = fieldNameRectangle.Bottom + 10;

                // Add a text box field to PDF form
                fieldNameTextElement = new TextElement(xLocation, yLocation, 60, "Last name:", fieldNameFont);
                addElementResult = pdfPage.AddElement(fieldNameTextElement);
                fieldNameRectangle = addElementResult.EndPageBounds;
                fieldBoundingRectangle = new RectangleF(fieldNameRectangle.Right + 10, yLocation, 150, 15);
                // Create the form field
                textBoxField = pdfDocument.Form.AddTextBox(pdfPage, fieldBoundingRectangle, "Enter Last Name", textFieldFont);
                // Set unique form field name used when the form is submitted
                textBoxField.Name = "lastName";
                // Set the form field default value
                textBoxField.DefaultValue = "A default last name";
                // Set form field style 
                textBoxField.Style.BackColor = Color.MistyRose;

                yLocation = fieldNameRectangle.Bottom + 10;

                // Add a password text box field to PDF form
                fieldNameTextElement = new TextElement(xLocation, yLocation, 60, "Password:", fieldNameFont);
                addElementResult = pdfPage.AddElement(fieldNameTextElement);
                fieldNameRectangle = addElementResult.EndPageBounds;
                fieldBoundingRectangle = new RectangleF(fieldNameRectangle.Right + 10, yLocation, 150, 15);
                // Create the form field
                PdfFormTextBox passwordTextBoxField = pdfDocument.Form.AddTextBox(pdfPage, fieldBoundingRectangle, "", textFieldFont);
                // Set unique form field name used when the form is submitted
                passwordTextBoxField.Name = "password";
                // Set form field style 
                passwordTextBoxField.Style.BackColor = Color.AliceBlue;
                // Set the password mode for the text box
                passwordTextBoxField.IsPassword = true;

                yLocation = fieldNameRectangle.Bottom + 10;

                // Add a radio buttons group to PDF form
                fieldNameTextElement = new TextElement(xLocation, yLocation, 60, "Gender:", fieldNameFont);
                addElementResult = pdfPage.AddElement(fieldNameTextElement);
                fieldNameRectangle = addElementResult.EndPageBounds;
                // Create the radio buttons group
                PdfFormRadioButtonsGroup radioButtonsGroup = pdfDocument.Form.AddRadioButtonsGroup(pdfPage);
                // Set unique form field name used when the form is submitted
                radioButtonsGroup.Name = "gender";
                // Set style of the radio buttons in this group
                radioButtonsGroup.Style.BackColor = Color.AntiqueWhite;

                // Add the first radio button to group
                RectangleF radioButtonRectangle = new RectangleF(fieldNameRectangle.Right + 10, yLocation, 50, 10);
                // Create the form field
                PdfFormRadioButton radioButtonField = radioButtonsGroup.AddRadioButton(radioButtonRectangle, "male", pdfPage);

                fieldNameTextElement = new TextElement(fieldNameRectangle.Right + 22, yLocation, 30, "Male", fieldNameFont);
                pdfPage.AddElement(fieldNameTextElement);

                // Add the second radio button to group
                radioButtonRectangle = new RectangleF(fieldNameRectangle.Right + 60, yLocation, 50, 10);
                // Create the form field
                radioButtonField = radioButtonsGroup.AddRadioButton(radioButtonRectangle, "female", pdfPage);

                fieldNameTextElement = new TextElement(fieldNameRectangle.Right + 72, yLocation, 30, "Female", fieldNameFont);
                pdfPage.AddElement(fieldNameTextElement);

                // Set the selected radio btton in group
                radioButtonsGroup.SetCheckedRadioButton("male");

                yLocation = fieldNameRectangle.Bottom + 10;

                // Add a checkbox field to PDF form
                fieldNameTextElement = new TextElement(xLocation, yLocation, 60, "Vehicle:", fieldNameFont);
                addElementResult = pdfPage.AddElement(fieldNameTextElement);
                fieldNameRectangle = addElementResult.EndPageBounds;
                fieldBoundingRectangle = new RectangleF(fieldNameRectangle.Right + 10, yLocation, 10, 10);
                // Create the form field
                PdfFormCheckBox checkBoxField = pdfDocument.Form.AddCheckBox(pdfPage, fieldBoundingRectangle);
                // Set unique form field name used when the form is submitted
                checkBoxField.Name = "haveCar";
                // Set form field style 
                checkBoxField.Style.BackColor = Color.AntiqueWhite;
                // Set checkbox field checked state
                checkBoxField.Checked = true;

                fieldNameTextElement = new TextElement(fieldNameRectangle.Right + 22, yLocation, 50, "I have a car", fieldNameFont);
                pdfPage.AddElement(fieldNameTextElement);

                yLocation = fieldNameRectangle.Bottom + 10;

                // Add a combo box list field to PDF form
                fieldNameTextElement = new TextElement(xLocation, yLocation, 60, "Vehicle Type:", fieldNameFont);
                addElementResult = pdfPage.AddElement(fieldNameTextElement);
                fieldNameRectangle = addElementResult.EndPageBounds;
                fieldBoundingRectangle = new RectangleF(fieldNameRectangle.Right + 10, yLocation, 50, 15);
                string[] comboBoxItems = new string[] { "Volvo", "Saab", "Audi", "Opel" };
                // Create the form field
                PdfFormComboBox comboBoxField = pdfDocument.Form.AddComboBox(pdfPage, fieldBoundingRectangle, comboBoxItems, comboBoxFieldFont);
                // Set unique form field name used when the form is submitted
                comboBoxField.Name = "vehicleType";
                // Set the form field default value
                comboBoxField.DefaultValue = "Audi";
                // Set form field style 
                comboBoxField.Style.BackColor = Color.LightCyan;
                // Set selected item in combo box
                comboBoxField.Value = "Audi";

                yLocation = fieldNameRectangle.Bottom + 10;

                // Add a multiline text box field to PDF form
                fieldNameTextElement = new TextElement(xLocation, yLocation + 20, 60, "Comments:", fieldNameFont);
                addElementResult = pdfPage.AddElement(fieldNameTextElement);
                fieldNameRectangle = addElementResult.EndPageBounds;
                fieldBoundingRectangle = new RectangleF(fieldNameRectangle.Right + 10, yLocation, 150, 60);
                // Create the form field
                PdfFormTextBox multilineTextBoxField = pdfDocument.Form.AddTextBox(pdfPage, fieldBoundingRectangle,
                        "Enter your comments here:\r\nFirst comment line\r\nSecond comment line", textFieldFont);
                // Set unique form field name used when the form is submitted
                multilineTextBoxField.Name = "comments";
                // Set form field style 
                multilineTextBoxField.Style.BackColor = Color.AliceBlue;
                // Set the multiline mode for text box field
                multilineTextBoxField.IsMultiLine = true;

                yLocation = yLocation + 70;

                // Add a form submit button to PDF form
                fieldBoundingRectangle = new RectangleF(xLocation, yLocation, 75, 15);
                PdfFormButton submitFormButton = pdfDocument.Form.AddButton(pdfPage, fieldBoundingRectangle, "Submit", buttonTextFont);
                // Set unique form field name used when the form is submitted
                submitFormButton.Name = "submitFormButton";
                // Set form field style 
                submitFormButton.Style.BackColor = Color.Beige;
                // Create the form submit action
                PdfSubmitFormAction submitFormAction = new PdfSubmitFormAction(collection["submitUrlTextBox"]);
                // Form values will be submitted in HTML form format
                submitFormAction.Flags |= PdfFormSubmitFlags.ExportFormat;
                if (collection["HttpMethod"] == "getMethodRadioButton")
                    submitFormAction.Flags |= PdfFormSubmitFlags.GetMethod;
                // Set the form submit button action
                submitFormButton.Action = submitFormAction;

                // Add a form reset button to PDF form
                fieldBoundingRectangle = new RectangleF(xLocation + 100, yLocation, 75, 15);
                PdfFormButton resetFormButton = pdfDocument.Form.AddButton(pdfPage, fieldBoundingRectangle, "Reset", buttonTextFont);
                // Set unique form field name used when the form is submitted
                resetFormButton.Name = "resetFormButton";
                // Set form field style 
                resetFormButton.Style.BackColor = Color.Beige;
                // Create the form reset action
                PdfResetFormAction resetFormAction = new PdfResetFormAction();
                // Set the form reset button action
                resetFormButton.Action = resetFormAction;

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Create_PDF_Forms.pdf";

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