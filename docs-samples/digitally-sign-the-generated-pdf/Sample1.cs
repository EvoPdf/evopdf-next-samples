using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class PDF_Digital_SignaturesController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public PDF_Digital_SignaturesController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(PDF_Digital_Signatures_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            string certificateFilePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Certificates/evopdf.pfx";

            if (model.KeySource == "Certificate")
            {
                // An X509Certificate2 with access to its private key: a certificate from the Windows certificate
                // store, a smartcard or a USB token. The private key stays in its store and the certificate
                // chain is built from the system stores. The sample loads the PFX as a certificate object
                X509Certificate2 certificate = LoadCertificate(certificateFilePath, "evopdf");
                htmlToPdfConverter.DigitalSignature = new PdfDigitalSignature(certificate);
            }
            else if (model.KeySource == "External")
            {
                // External signing: the library prepares the signature and asks an IExternalDigitalSignature
                // implementation for the signature value, so the private key can stay in a hardware security
                // module or a cloud key service. The signer certificate chain is given separately.
                // This sample implementation signs with the key of the PFX certificate to show the interface
                X509Certificate2 certificate = LoadCertificate(certificateFilePath, "evopdf");
                htmlToPdfConverter.DigitalSignature = new PdfDigitalSignature(new CertificateKeySignature(certificate), new[] { certificate.RawData });
            }
            else
            {
                htmlToPdfConverter.DigitalSignature = new PdfDigitalSignature(certificateFilePath, "evopdf");
            }

            // optionally set digital signature field name in PDF document
            htmlToPdfConverter.DigitalSignature.FieldName = "EvoPdf Signature Field";

            // set the digital signature information that will be displayed in the signatures panel in Adobe Reader
            // and also in the signature appearance in PDF if a custom signature text was not explicitly set
            htmlToPdfConverter.DigitalSignature.Reason = model.SignatureReason;
            htmlToPdfConverter.DigitalSignature.Location = model.SignatureLocation;
            htmlToPdfConverter.DigitalSignature.ContactInfo = model.SignatureContact;

            // The PAdES signature level. Auto produces PAdES B-T when a timestamp server is set and
            // PAdES B-B otherwise
            htmlToPdfConverter.DigitalSignature.Profile = model.Profile;

            // An approval signature by default; a certification signature declares the
            // changes the document accepts after signing: none, form filling and signing, or also annotations
            htmlToPdfConverter.DigitalSignature.CertificationLevel = model.CertificationLevel;

            // The timestamp server, for example http://timestamp.digicert.com
            if (!string.IsNullOrWhiteSpace(model.TimestampServerUrl))
                htmlToPdfConverter.DigitalSignature.TimestampServerUrl = model.TimestampServerUrl.Trim();

            // enable signature appearance in PDF page
            htmlToPdfConverter.DigitalSignature.AppearanceEnabled = model.EnableAppearance;

            if (htmlToPdfConverter.DigitalSignature.AppearanceEnabled)
            {
                // set the digital signature appearance position in PDF document
                if (model.DisplayOnLastPage)
                {
                    // set the appearance to be displayed at the bottom of the last page
                    htmlToPdfConverter.DigitalSignature.Appearance.DisplayOnLastPage = true;
                    htmlToPdfConverter.DigitalSignature.Appearance.BoundsRectangle =
                            new PdfRectangle(0, htmlToPdfConverter.PdfDocumentOptions.PdfPageSize.Height - 50, 200, 50);

                    // optionally reserve space for signature appearance at the bottom of the PDF page
                    htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 50;
                }
                else
                {
                    // set the appearance to be displayed at the top of the first page
                    htmlToPdfConverter.DigitalSignature.Appearance.PageNumber = 1;
                    htmlToPdfConverter.DigitalSignature.Appearance.BoundsRectangle = new PdfRectangle(0, 0, 200, 50);

                    // optionally reserve space for signature appearance at the top of the PDF page
                    htmlToPdfConverter.PdfDocumentOptions.TopMargin = 50;
                }

                // set the signature text in appearance or leave it null or empty to display the default signature information
                if (model.AddSignatureText && !string.IsNullOrEmpty(model.SignatureText))
                    htmlToPdfConverter.DigitalSignature.Appearance.Text = model.SignatureText;

                // set the signature image in appearance
                if (model.AddSignatureImage)
                {
                    string imageFilePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Images/evologo.png";
                    htmlToPdfConverter.DigitalSignature.Appearance.SetImage(imageFilePath, true);
                }
            }

            byte[] outPdfBuffer = null;

            if (model.HtmlPageSource == "Html")
            {
                string htmlWithForm = model.HtmlString;
                string baseUrl = model.BaseUrl;

                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlWithForm, baseUrl);
            }
            else
            {
                string url = model.Url;

                outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
            }

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Digital_Signatures.pdf";

            return fileResult;
        }

        // Loads the sample PFX as a certificate object. On Windows the private key goes to the machine key
        // store, which needs no user profile, so the code runs in IIS application pools and in Azure App Service;
        // macOS does not support in-memory keys and uses its default key storage
        private static X509Certificate2 LoadCertificate(string certificateFilePath, string password)
        {
            X509KeyStorageFlags keyStorageFlags = OperatingSystem.IsWindows() ? X509KeyStorageFlags.MachineKeySet :
                OperatingSystem.IsMacOS() ? X509KeyStorageFlags.DefaultKeySet : X509KeyStorageFlags.EphemeralKeySet;
#if NET9_0_OR_GREATER
            return X509CertificateLoader.LoadPkcs12FromFile(certificateFilePath, password, keyStorageFlags);
#else
            return new X509Certificate2(certificateFilePath, password, keyStorageFlags);
#endif
        }

        private PDF_Digital_Signatures_ViewModel SetViewModel()
        {
            var model = new PDF_Digital_Signatures_ViewModel();

            var contentRootPath = m_hostingEnvironment.ContentRootPath + "/wwwroot";

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "PDF_Digital_Signatures".Length);

            model.Url = "http://www.evopdf.com";
            model.HtmlString = "Enter the <b>HTML String to Convert</b> and optionally set a <b>Base URL</b> if the HTML string references external resources by relative URLs";
            model.BaseUrl = rootUrl;

            model.SignatureReason = "My Signature Reason";
            model.SignatureLocation = "My Signature Location";
            model.SignatureContact = "My Contact Information";
            model.EnableAppearance = true;
            model.DisplayOnLastPage = false;
            model.AddSignatureText = true;
            model.SignatureText = "Signed by EVO PDF Software";
            model.AddSignatureImage = true;

            return model;
        }
    }

    // Sample IExternalDigitalSignature implementation: produces the RSA signature value with the
    // private key of an X509Certificate2. A production implementation forwards the message to the device or
    // service that holds the key and returns the signature value it produces
    public class CertificateKeySignature : IExternalDigitalSignature
    {
        private readonly X509Certificate2 m_certificate;

        public CertificateKeySignature(X509Certificate2 certificate)
        {
            m_certificate = certificate;
        }

        public string GetSignatureAlgorithmName()
        {
            return "RSA";
        }

        public byte[] Sign(byte[] message)
        {
            using (RSA rsa = m_certificate.GetRSAPrivateKey())
            {
                return rsa.SignData(message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    }
}
