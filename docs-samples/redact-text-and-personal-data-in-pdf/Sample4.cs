using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.Redact_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.Redact_PDF
{
    public class Redact_Text_and_Personal_DataController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Redact_Text_and_Personal_DataController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RedactText(Redact_Text_and_Personal_Data_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            byte[] inputPdfBytes = await ReadInputPdf(model);
            string password = !string.IsNullOrEmpty(model.OwnerPassword) ? model.OwnerPassword :
                !string.IsNullOrEmpty(model.UserPassword) ? model.UserPassword : null;

            using PdfRedactor pdfRedactor = new PdfRedactor(inputPdfBytes, password);

            // The appearance of the redacted areas: a fill color and a text written over each area
            pdfRedactor.FillColor = GetFillColor(model.FillColor);
            if (!string.IsNullOrWhiteSpace(model.OverlayText))
            {
                pdfRedactor.OverlayText = model.OverlayText.Trim();
                pdfRedactor.OverlayTextColor = model.FillColor == "White" ? PdfColor.Black : PdfColor.White;
                pdfRedactor.RepeatOverlayText = model.RepeatOverlayText;
            }

            // Each text is searched in all the pages and every occurrence is marked for redaction. The same texts
            // are also replaced in the document information, bookmarks, annotations and form field values
            if (!string.IsNullOrWhiteSpace(model.TextsToRedact))
            {
                string[] texts = model.TextsToRedact.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string text in texts)
                {
                    if (!string.IsNullOrWhiteSpace(text))
                        pdfRedactor.RedactText(text.Trim(), model.MatchCase, model.WholeWord);
                }
            }

            // The texts that match a regular expression, for example the customer numbers
            if (model.UseRegularExpression && !string.IsNullOrWhiteSpace(model.RegularExpression))
            {
                Regex pattern;
                try
                {
                    pattern = new Regex(model.RegularExpression, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(5));
                }
                catch (ArgumentException ex)
                {
                    throw new Exception("The regular expression is not valid", ex);
                }

                pdfRedactor.RedactText(pattern);
            }

            // The personal data is found by its structure and validated, for example by the check digits of
            // bank account and payment card numbers
            PdfPersonalDataTypes personalDataTypes = GetPersonalDataTypes(model);
            if (personalDataTypes != PdfPersonalDataTypes.None)
                pdfRedactor.RedactPersonalData(personalDataTypes);

            // Remove the information that can remain in the document outside the redacted areas
            pdfRedactor.RemoveMetadata = model.RemoveMetadata;
            pdfRedactor.RemoveJavaScript = model.RemoveJavaScript;
            pdfRedactor.RemoveEmbeddedFiles = model.RemoveEmbeddedFiles;
            pdfRedactor.RemoveHiddenContent = model.RemoveHiddenContent;

            byte[] outPdfBuffer;
            string fileName;
            if (model.SaveRedactionMarks)
            {
                // The marked areas are saved as redaction annotations, for a review in a PDF viewer before the
                // content is removed by applying them
                outPdfBuffer = pdfRedactor.SaveRedactionMarks();
                fileName = "Redaction_Marks.pdf";
            }
            else
            {
                // Apply the redactions. The removed content cannot be recovered from the saved document and
                // pdfRedactor.Result gives the number of removed characters, images, annotations and form fields
                outPdfBuffer = pdfRedactor.Save();
                fileName = "Redact_Text_and_Personal_Data.pdf";
            }

            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = fileName;
            return fileResult;
        }

        private static PdfColor GetFillColor(string fillColor)
        {
            switch (fillColor)
            {
                case "Black":
                    return PdfColor.Black;
                case "DarkGray":
                    return PdfColor.DarkGray;
                case "White":
                    return PdfColor.White;
                case "None":
                    // The content is removed and the areas are left empty
                    return null;
                default:
                    return PdfColor.Black;
            }
        }

        private static PdfPersonalDataTypes GetPersonalDataTypes(Redact_Text_and_Personal_Data_ViewModel model)
        {
            PdfPersonalDataTypes types = PdfPersonalDataTypes.None;
            if (model.RedactEmailAddresses)
                types |= PdfPersonalDataTypes.EmailAddress;
            if (model.RedactPhoneNumbers)
                types |= PdfPersonalDataTypes.PhoneNumber;
            if (model.RedactIbans)
                types |= PdfPersonalDataTypes.Iban;
            if (model.RedactPaymentCardNumbers)
                types |= PdfPersonalDataTypes.PaymentCardNumber;
            if (model.RedactIpAddresses)
                types |= PdfPersonalDataTypes.IpAddress;
            if (model.RedactSocialSecurityNumbers)
                types |= PdfPersonalDataTypes.UsSocialSecurityNumber;
            if (model.RedactPersonalNumericCodes)
                types |= PdfPersonalDataTypes.RomanianPersonalNumericCode;
            return types;
        }

        private async Task<byte[]> ReadInputPdf(Redact_Text_and_Personal_Data_ViewModel model)
        {
            // If an uploaded file exists, use it with priority
            if (model.PdfFile != null && model.PdfFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await model.PdfFile.CopyToAsync(ms);
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded PDF file", ex);
                }
            }

            // Otherwise, fall back to the URL
            string pdfUrl = model.PdfFileUrl?.Trim();
            if (string.IsNullOrWhiteSpace(pdfUrl))
                throw new Exception("No PDF file provided: upload a file or specify a URL");

            try
            {
                if (pdfUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    string localPath = new Uri(pdfUrl).LocalPath;
                    return await System.IO.File.ReadAllBytesAsync(localPath);
                }

                using var httpClient = new System.Net.Http.HttpClient();
                return await httpClient.GetByteArrayAsync(pdfUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not download the PDF file from URL", ex);
            }
        }

        private Redact_Text_and_Personal_Data_ViewModel SetViewModel()
        {
            var model = new Redact_Text_and_Personal_Data_ViewModel();

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Path = request.PathBase.ToString() + request.Path.ToString(),
                Query = request.QueryString.ToString()
            };
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Redact_Text_and_Personal_Data".Length);

            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/Personal_Data_Document.pdf";

            return model;
        }
    }
}
