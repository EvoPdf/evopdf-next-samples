using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_Editor;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_Editor
{
    public class Sign_Existing_PDFController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Sign_Existing_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPdf(Sign_Existing_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            byte[] inputPdfBytes = null;

            // If an uploaded file exists, use it with priority
            if (model.PdfFile != null && model.PdfFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await model.PdfFile.CopyToAsync(ms);
                    inputPdfBytes = ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded PDF file", ex);
                }
            }
            else
            {
                // Otherwise, fall back to the URL
                string pdfUrl = model.PdfFileUrl?.Trim();
                if (string.IsNullOrWhiteSpace(pdfUrl))
                    throw new Exception("No PDF file provided: upload a file or specify a URL");

                try
                {
                    if (pdfUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                    {
                        string localPath = new Uri(pdfUrl).LocalPath;
                        inputPdfBytes = await System.IO.File.ReadAllBytesAsync(localPath);
                    }
                    else
                    {
                        using var httpClient = new System.Net.Http.HttpClient();
                        inputPdfBytes = await httpClient.GetByteArrayAsync(pdfUrl);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not download the PDF file from URL", ex);
                }
            }

            string certificateFilePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Certificates/evopdf.pfx";

            // The digital signature added to the loaded document when it is saved
            PdfDigitalSignature digitalSignature = new PdfDigitalSignature(certificateFilePath, "evopdf");
            digitalSignature.Reason = model.SignatureReason;
            digitalSignature.Location = model.SignatureLocation;
            digitalSignature.ContactInfo = model.SignatureContact;

            // The PAdES signature level. Auto produces PAdES B-T when a timestamp server is set and
            // PAdES B-B otherwise
            digitalSignature.Profile = model.Profile;

            // A certification signature must be the first signature of the document, so it
            // applies to documents that are not signed yet
            digitalSignature.CertificationLevel = model.CertificationLevel;

            // The timestamp server, for example http://timestamp.digicert.com
            if (!string.IsNullOrWhiteSpace(model.TimestampServerUrl))
                digitalSignature.TimestampServerUrl = model.TimestampServerUrl.Trim();

            // When the document already has an empty signature field with this name, for example a form
            // prepared with a field for each signer, the signature is placed in that field, on its page and
            // in its rectangle; otherwise a new field with this name is created
            if (!string.IsNullOrWhiteSpace(model.SignatureFieldName))
                digitalSignature.FieldName = model.SignatureFieldName.Trim();

            // Open the loaded PDF for editing with the digital signature. A document that already
            // carries signatures is saved as an incremental update, so the existing signatures stay valid
            string password = string.IsNullOrEmpty(model.OwnerPassword) ? model.UserPassword : model.OwnerPassword;
            using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, password, digitalSignature);

            digitalSignature.AppearanceEnabled = model.EnableAppearance;
            if (digitalSignature.AppearanceEnabled)
            {
                // set the signature text in appearance or leave it null or empty to display the default signature information
                if (!string.IsNullOrEmpty(model.SignatureText))
                    digitalSignature.Appearance.Text = model.SignatureText;

                // Position of a new signature field: the bottom of the first page, to the right of the
                // signatures already there. An existing signature field keeps its own position
                PdfPageInfo pageInfo = pdfEditor.GetPdfPageInfo(1);
                int signatureCount = pdfEditor.PdfSignatureInfo.SignatureCount;
                int x = 20 + 210 * (signatureCount % 2);
                digitalSignature.Appearance.PageNumber = 1;
                digitalSignature.Appearance.BoundsRectangle = new PdfRectangle(x, (int)pageInfo.MediaBox.Height - 70, 200, 50);
            }

            byte[] outPdfBuffer = pdfEditor.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Sign_Existing_PDF.pdf";
            return fileResult;
        }

        private Sign_Existing_PDF_ViewModel SetViewModel()
        {
            var model = new Sign_Existing_PDF_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(
                0, currentPageUrl.Length - "Sign_Existing_PDF".Length);

            // The default input is a document that already carries a digital signature
            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/Signed_Document.pdf";

            return model;
        }
    }
}
