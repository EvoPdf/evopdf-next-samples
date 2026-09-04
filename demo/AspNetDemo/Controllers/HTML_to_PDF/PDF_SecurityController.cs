using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class PDF_SecurityController : Controller
    {
        public ActionResult Index()
        {
            var model = new PDF_Security_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(PDF_Security_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Set the encryption algorithm and the encryption key size if they are not the default ones
            if (model.EncryptionKey != "Bit128" || model.EncryptionType != "RC4")
            {
                // set the encryption algorithm
                htmlToPdfConverter.PdfSecurityOptions.EncryptionAlgorithm = model.EncryptionType == "RC4" ? EncryptionAlgorithm.RC4 : EncryptionAlgorithm.AES;

                // set the encryption key size
                if (model.EncryptionKey == "Bit40")
                    htmlToPdfConverter.PdfSecurityOptions.KeySize = EncryptionKeySize.EncryptKey40Bit;
                else if (model.EncryptionKey == "Bit128")
                    htmlToPdfConverter.PdfSecurityOptions.KeySize = EncryptionKeySize.EncryptKey128Bit;
                else if (model.EncryptionKey == "Bit256")
                    htmlToPdfConverter.PdfSecurityOptions.KeySize = EncryptionKeySize.EncryptKey256Bit;
            }

            // Set user and owner passwords
            if (!string.IsNullOrEmpty(model.UserPassword))
                htmlToPdfConverter.PdfSecurityOptions.UserPassword = model.UserPassword;

            if (!string.IsNullOrEmpty(model.OwnerPassword))
                htmlToPdfConverter.PdfSecurityOptions.OwnerPassword = model.OwnerPassword;

            // Set PDF document permissions
            htmlToPdfConverter.PdfSecurityOptions.CanPrint = model.PrintEnabled;
            htmlToPdfConverter.PdfSecurityOptions.CanCopyContent = model.CopyContentEnabled;
            htmlToPdfConverter.PdfSecurityOptions.CanCopyAccessibilityContent = model.CopyAccessibilityContentEnabled;
            htmlToPdfConverter.PdfSecurityOptions.CanEditContent = model.EditContentEnabled;
            htmlToPdfConverter.PdfSecurityOptions.CanEditAnnotations = model.EditAnnotationsEnabled;
            htmlToPdfConverter.PdfSecurityOptions.CanFillFormFields = model.FillFormFieldsEnabled;

            if ((PermissionsChanged(htmlToPdfConverter) || htmlToPdfConverter.PdfSecurityOptions.UserPassword.Length > 0) &&
                htmlToPdfConverter.PdfSecurityOptions.OwnerPassword.Length == 0)
            {
                // A user password is set but the owner password is not set or the permissions are not the default ones
                // Set a default owner password
                htmlToPdfConverter.PdfSecurityOptions.OwnerPassword = "owner";
            }

            // Convert the HTML page to a PDF document in a memory buffer
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(model.Url);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Set_Permissions_Password.pdf";

            return fileResult;
        }

        private bool PermissionsChanged(HtmlToPdfConverter htmlToPdfConverter)
        {
            return !htmlToPdfConverter.PdfSecurityOptions.CanPrint ||
                    !htmlToPdfConverter.PdfSecurityOptions.CanCopyContent || !htmlToPdfConverter.PdfSecurityOptions.CanCopyAccessibilityContent ||
                    !htmlToPdfConverter.PdfSecurityOptions.CanEditContent || !htmlToPdfConverter.PdfSecurityOptions.CanEditAnnotations ||
                    !htmlToPdfConverter.PdfSecurityOptions.CanFillFormFields;
        }
    }
}