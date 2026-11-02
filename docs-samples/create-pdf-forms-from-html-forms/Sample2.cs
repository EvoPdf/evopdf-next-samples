using System;
using System.IO;
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
    public class Create_PDF_Forms_from_HTML_FormsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Create_PDF_Forms_from_HTML_FormsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Create_PDF_Forms_from_HTML_Forms_ViewModel model)
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

            // Specifies whether HTML form controls are converted to interactive PDF form fields
            htmlToPdfConverter.PdfDocumentOptions.GeneratePdfFormFields = model.GeneratePdfFormFields;

            PdfFormOptions formOptions = htmlToPdfConverter.PdfDocumentOptions.PdfFormOptions;

            // The names of the PDF fields: the HTML names when they are unique, the HTML names as they are, or
            // names qualified with the form and the position of the control
            formOptions.FieldNamingMode = (PdfFieldNamingMode)Enum.Parse(typeof(PdfFieldNamingMode), model.FieldNamingMode);

            // The actions of the generated buttons: the HTML submit and reset buttons submit and reset the PDF form
            formOptions.EnableButtonActions = model.EnableButtonActions;
            formOptions.MapHtmlSubmitButtonsToPdfActions = model.MapHtmlSubmitButtonsToPdfActions;
            formOptions.MapHtmlResetButtonsToPdfActions = model.MapHtmlResetButtonsToPdfActions;

            // The fields submitted or reset by a button: only the fields of its HTML form, or all the fields
            formOptions.SubmitOnlyFieldsFromButtonForm = model.SubmitOnlyFieldsFromButtonForm;
            formOptions.ResetOnlyFieldsFromButtonForm = model.ResetOnlyFieldsFromButtonForm;
            formOptions.ExcludeDisabledFieldsFromSubmit = model.ExcludeDisabledFieldsFromSubmit;
            formOptions.IncludeReadOnlyFieldsInSubmit = model.IncludeReadOnlyFieldsInSubmit;

            // The HTML required attribute marks the PDF field as required
            formOptions.ApplyRequiredAsPdfRequiredFlag = model.ApplyRequiredAsPdfRequiredFlag;

            // The submit action is a native PDF action or a script that submits the form like a browser does;
            // the format is the format of the submitted data
            formOptions.SubmitBehavior = (PdfSubmitBehavior)Enum.Parse(typeof(PdfSubmitBehavior), model.SubmitBehavior);
            formOptions.SubmitFormat = (PdfSubmitFormat)Enum.Parse(typeof(PdfSubmitFormat), model.SubmitFormat);

            // A basic validation script can be generated from the HTML validation attributes
            formOptions.ValidationBehavior = (PdfValidationBehavior)Enum.Parse(typeof(PdfValidationBehavior), model.ValidationBehavior);

            if (model.UseFormFieldFont)
            {
                // The font of the text displayed in the fields; a standard font is used otherwise
                string fontFilePath = Path.Combine(m_hostingEnvironment.ContentRootPath, "wwwroot", "DemoAppFiles", "Input", "Font_Files", "DejaVuSerif.ttf");
                formOptions.FormFieldFont = PdfFontManager.CreateBaseFont(fontFilePath);
            }

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = null;

            if (model.HtmlPageSource == "Html")
            {
                string htmlString = model.HtmlString;
                string baseUrl = model.BaseUrl;

                // Convert a HTML string with a base URL to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlString, baseUrl);
            }
            else
            {
                string url = model.Url;

                // Convert the HTML page given by an URL to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
            }

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Create_PDF_Forms.pdf";

            return fileResult;
        }

        private Create_PDF_Forms_from_HTML_Forms_ViewModel SetViewModel()
        {
            var model = new Create_PDF_Forms_from_HTML_Forms_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Create_PDF_Forms_from_HTML_Forms".Length);

            model.HtmlString = System.IO.File.ReadAllText(System.IO.Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/HTML_Form.html"));
            model.BaseUrl = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.Url = rootUrl + "DemoAppFiles/Input/HTML_Files/HTML_Form.html";

            return model;
        }
    }
}
