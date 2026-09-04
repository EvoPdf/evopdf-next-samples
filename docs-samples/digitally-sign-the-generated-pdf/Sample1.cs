// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/digitally-sign-the-generated-pdf.htm
// Documentation page: Add a Digital Signature to Generated PDF Document

using System;
using System.ComponentModel.DataAnnotations;
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

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            string certificateFilePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Certificates/evopdf.pfx";
            htmlToPdfConverter.DigitalSignature = new PdfDigitalSignature(certificateFilePath, "evopdf");

            // optionally set digital signature field name in PDF document
            htmlToPdfConverter.DigitalSignature.FieldName = "EvoPdf Signature Field";

            // set the digital signature information that will be displayed in the signatures panel in Adobe Reader
            // and also in the signature appearance in PDF if a custom signature text was not explicitly set
            htmlToPdfConverter.DigitalSignature.Reason = model.SignatureReason;
            htmlToPdfConverter.DigitalSignature.Location = model.SignatureLocation;
            htmlToPdfConverter.DigitalSignature.ContactInfo = model.SignatureContact;

            // Uncomment the line below to optionally set a timestamp from a certified timestamp server
            //htmlToPdfConverter.DigitalSignature.TimestampServerUrl = "http://tsa.belgium.be/connect";

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
}
