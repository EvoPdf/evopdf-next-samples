using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Signature_Fields_from_HTMLController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Signature_Fields_from_HTMLController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Signature_Fields_from_HTML_ViewModel model)
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

            // The HTML elements that mark the place of the signature fields. Their position in the
            // PDF pages is available in HtmlElementsInfo after the conversion
            htmlToPdfConverter.HtmlElementsInfoSelector = model.SignatureFieldsSelector;

            // Convert the HTML page to a PDF document
            byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(model.HtmlString, model.BaseUrl);

            if (htmlToPdfConverter.HtmlElementsInfo == null || htmlToPdfConverter.HtmlElementsInfo.Elements.Count == 0)
                throw new Exception("No HTML element matches the selector " + model.SignatureFieldsSelector);

            // Add an empty signature field where each placeholder was rendered
            string firstFieldName = null;
            using (PdfEditor pdfEditor = new PdfEditor(outPdfBuffer))
            {
                int fieldIndex = 1;
                foreach (HtmlElementInfo element in htmlToPdfConverter.HtmlElementsInfo.Elements)
                {
                    // The id of the placeholder is the name of the field. A digital signature with this
                    // field name is placed in the field
                    string fieldName = !string.IsNullOrEmpty(element.Id) ? element.Id : "Signature" + fieldIndex;
                    fieldIndex++;
                    if (firstFieldName == null)
                        firstFieldName = fieldName;

                    // The page and the rectangle where the placeholder was rendered
                    PdfRenderedRectangle renderedRectangle = element.RenderedRectangles[0];

                    pdfEditor.AddSignatureField(new PdfSignatureField(fieldName, renderedRectangle.PageNumber, renderedRectangle.Bounds));
                }

                outPdfBuffer = pdfEditor.Save();
            }

            // Sign the first field. The other fields stay empty, each waiting for its signer; the
            // Digitally Sign Existing PDF demo signs them by field name
            if (model.SignFirstField)
            {
                string certificateFilePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Certificates/evopdf.pfx";

                PdfDigitalSignature digitalSignature = new PdfDigitalSignature(certificateFilePath, "evopdf");
                digitalSignature.FieldName = firstFieldName;
                digitalSignature.Reason = "Approved";
                digitalSignature.Location = "My Signature Location";
                digitalSignature.ContactInfo = "My Contact Information";

                using PdfEditor signingEditor = new PdfEditor(outPdfBuffer, null, digitalSignature);

                // The appearance fills the rectangle of the field
                digitalSignature.AppearanceEnabled = true;
                digitalSignature.Appearance.Text = "Approved by EVO PDF Software";

                outPdfBuffer = signingEditor.Save();
            }

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Signature_Fields_from_HTML.pdf";

            return fileResult;
        }

        private Signature_Fields_from_HTML_ViewModel SetViewModel()
        {
            var model = new Signature_Fields_from_HTML_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Signature_Fields_from_HTML".Length);

            model.HtmlString = System.IO.File.ReadAllText(System.IO.Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Signature_Fields.html"));
            model.BaseUrl = rootUrl + "DemoAppFiles/Input/HTML_Files/";

            return model;
        }
    }
}
