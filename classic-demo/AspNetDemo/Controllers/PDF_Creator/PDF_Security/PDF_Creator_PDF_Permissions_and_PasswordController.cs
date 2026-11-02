using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator.PDF_Security
{
    public class PDF_Creator_PDF_Permissions_and_PasswordController : Controller
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

            // Set the encryption algorithm and the encryption key size if they are not the default ones
            if (collection["EncryptionKey"] != "bit128RadioButton" || collection["EncryptionType"] != "rc4RadioButton")
            {
                // set the encryption algorithm
                pdfDocument.Security.EncryptionAlgorithm = collection["EncryptionType"] == "rc4RadioButton" ? EncryptionAlgorithm.RC4 : EncryptionAlgorithm.AES;

                // set the encryption key size
                if (collection["EncryptionKey"] == "bit40RadioButton")
                    pdfDocument.Security.KeySize = EncryptionKeySize.EncryptKey40Bit;
                else if (collection["EncryptionKey"] == "bit128RadioButton")
                    pdfDocument.Security.KeySize = EncryptionKeySize.EncryptKey128Bit;
                else if (collection["EncryptionKey"] == "bit256RadioButton")
                    pdfDocument.Security.KeySize = EncryptionKeySize.EncryptKey256Bit;
            }

            // Set user and owner passwords
            if (collection["userPasswordTextBox"][0].Length > 0)
                pdfDocument.Security.UserPassword = collection["userPasswordTextBox"];

            if (collection["ownerPasswordTextBox"][0].Length > 0)
                pdfDocument.Security.OwnerPassword = collection["ownerPasswordTextBox"];

            // Set PDF document permissions
            pdfDocument.Security.CanPrint = collection["printEnabledCheckBox"].Count > 0;
            pdfDocument.Security.CanPrintHighResolution = collection["highResolutionPrintEnabledCheckBox"].Count > 0;
            pdfDocument.Security.CanCopyContent = collection["copyContentEnabledCheckBox"].Count > 0;
            pdfDocument.Security.CanCopyAccessibilityContent = collection["copyAccessibilityContentEnabledCheckBox"].Count > 0;
            pdfDocument.Security.CanEditContent = collection["editContentEnabledCheckBox"].Count > 0;
            pdfDocument.Security.CanEditAnnotations = collection["editAnnotationsEnabledCheckBox"].Count > 0;
            pdfDocument.Security.CanFillFormFields = collection["fillFormFieldsEnabledCheckBox"].Count > 0;

            if ((PermissionsChanged(pdfDocument) || pdfDocument.Security.UserPassword.Length > 0) &&
                pdfDocument.Security.OwnerPassword.Length == 0)
            {
                // A user password is set but the owner password is not set or the permissions are not the default ones
                // Set a default owner password
                pdfDocument.Security.OwnerPassword = "owner";
            }

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
                fileResult.FileDownloadName = "Set_Permissions_Password.pdf";

                return fileResult;
            }
            finally
            {
                // Close the PDF document
                pdfDocument.Close();
            }
        }

        private bool PermissionsChanged(Document pdfDocument)
        {
            return !pdfDocument.Security.CanPrint || !pdfDocument.Security.CanPrintHighResolution ||
                    !pdfDocument.Security.CanCopyContent || !pdfDocument.Security.CanCopyAccessibilityContent ||
                    !pdfDocument.Security.CanEditContent || !pdfDocument.Security.CanEditAnnotations ||
                    !pdfDocument.Security.CanFillFormFields;
        }
    }
}