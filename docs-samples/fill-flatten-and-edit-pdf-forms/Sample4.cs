using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    public class Fill_PDF_FormsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Fill_PDF_FormsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FillForm(Fill_PDF_Forms_ViewModel model)
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

            using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, password);

            if (!pdfEditor.ContainsForm)
                throw new Exception("The loaded document does not contain a form");

            PdfForm form = pdfEditor.Form;

            // The appearance of a field is regenerated when its value changes, so the new value is displayed
            // by any viewer; a viewer regenerates the appearances itself when the flag is false
            form.GenerateAppearances = model.GenerateAppearances;

            if (model.UseSubstitutionFont)
            {
                // The characters missing from the font of a field are displayed with this font
                form.AddSubstitutionFont(PdfFontManager.CreateBaseFont(Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf")));
            }

            if (model.ReplaceCountryOptions && form.ContainsField("country"))
            {
                // The options of a combo box or list box are replaced; the value selected below must be an export value of the new options
                form.SetOptions("country", new List<PdfChoiceOption>
                {
                    new PdfChoiceOption("Canada", "CA"),
                    new PdfChoiceOption("Germany", "DE"),
                    new PdfChoiceOption("Japan", "JP"),
                    new PdfChoiceOption("United States", "US")
                });
            }

            if (model.ImportSampleData || (model.DataFile != null && model.DataFile.Length > 0))
            {
                // The values of a FDF or XFDF file fill the fields with the same names
                byte[] formData;
                if (model.DataFile != null && model.DataFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await model.DataFile.CopyToAsync(ms);
                    formData = ms.ToArray();
                }
                else
                {
                    formData = await System.IO.File.ReadAllBytesAsync(Path.Combine(GetDemoFilesPath(), "PDF_Files", "PDF_Form_Data.fdf"));
                }
                form.ImportData(formData);
            }

            if (model.FillFields)
            {
                // The values typed in the demo page are set field by field; a field missing from the loaded document is skipped
                SetValue(form, "fullName", model.FullName);
                SetValue(form, "email", model.Email);
                SetValue(form, "accessCode", "A1B2C3");
                SetValue(form, "country", model.Country);
                SetValue(form, "ticketType", model.TicketType);
                SetValue(form, "comments", model.Comments);

                if (form.ContainsField("newsletter"))
                    form.SetChecked("newsletter", model.Newsletter);
                if (form.ContainsField("terms"))
                    form.SetChecked("terms", model.Terms);

                if (form.ContainsField("sessions"))
                {
                    // Several options of a multiple selection list box are selected at once
                    string[] sessions = (model.Sessions ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                    form.SetValues("sessions", sessions);
                }

                if (form.ContainsField("total"))
                {
                    // The total is computed by the demo from the ticket type and the number of sessions
                    int ticketPrice = model.TicketType == "premium" ? 500 : model.TicketType == "student" ? 120 : 350;
                    int sessionsCount = form.GetValues("sessions")?.Length ?? 0;
                    form.SetValue("total", (ticketPrice + sessionsCount * 25).ToString());
                }
            }

            if (model.SetFieldColors)
            {
                foreach (string fieldName in form.FieldNames)
                {
                    PdfFormFieldInfo field = form.GetField(fieldName);
                    if (field.Type == PdfFormFieldType.Text || field.Type == PdfFormFieldType.ComboBox || field.Type == PdfFormFieldType.ListBox)
                    {
                        form.SetBackgroundColor(fieldName, PdfColor.FromHex("#FFF8E1"));
                        form.SetBorderColor(fieldName, PdfColor.DarkOrange);
                        form.SetTextColor(fieldName, PdfColor.DarkBlue);
                    }
                }
            }

            if (model.SetFieldFont)
            {
                // The font is created with a positive size; the size of the field text is set separately below
                PdfFont fieldFont = PdfFontManager.CreateFont(Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf"),
                    model.FieldFontSize > 0 ? model.FieldFontSize : 10, PdfFontStyle.Normal, PdfColor.Black);
                foreach (string fieldName in form.FieldNames)
                {
                    PdfFormFieldInfo field = form.GetField(fieldName);
                    if (field.Type == PdfFormFieldType.Text || field.Type == PdfFormFieldType.ComboBox || field.Type == PdfFormFieldType.ListBox)
                    {
                        // The font replaces the font of the field; a size of 0 sizes the text to fit the field
                        form.SetFont(fieldName, fieldFont);
                        form.SetFontSize(fieldName, model.FieldFontSize);
                    }
                }
            }

            if (model.ReadOnlyFields)
            {
                foreach (string fieldName in form.FieldNames)
                {
                    if (form.GetField(fieldName).Type != PdfFormFieldType.PushButton)
                    {
                        form.SetReadOnly(fieldName, true);
                        form.SetRequired(fieldName, false);
                    }
                }
            }

            if (form.ContainsField("comments"))
            {
                PdfFormFieldVisibility visibility = (PdfFormFieldVisibility)Enum.Parse(typeof(PdfFormFieldVisibility), model.CommentsVisibility);
                if (visibility != PdfFormFieldVisibility.Visible)
                    form.SetVisibility("comments", visibility);

                if (model.RenameCommentsField)
                    form.RenameField("comments", "remarks");
            }

            if (model.RemoveAccessCodeField && form.ContainsField("accessCode"))
            {
                // The field and its appearance in the page are removed
                form.RemoveField("accessCode");
            }

            if (model.OutputFormat == "Fdf")
            {
                // The current values of the fields, with the names of the fields and the path of the document
                byte[] fdfData = form.ExportFdf();
                FileResult fdfResult = new FileContentResult(fdfData, "application/vnd.fdf");
                fdfResult.FileDownloadName = "Fill_PDF_Forms.fdf";
                return fdfResult;
            }

            if (model.AddFieldsListPage)
                AddFieldsListPage(pdfEditor);

            if (model.FlattenMode == "FilledFields")
            {
                // Only the fields with a value become static content; the other fields stay editable
                foreach (string fieldName in form.FieldNames)
                {
                    PdfFormFieldInfo field = form.GetField(fieldName);
                    string value = form.GetValue(fieldName);
                    if (field.Type != PdfFormFieldType.PushButton && !string.IsNullOrEmpty(value) && value != "Off")
                        form.FlattenField(fieldName);
                }
            }
            else if (model.FlattenMode == "AllFields")
            {
                form.FlattenAllFields();
            }

            byte[] outPdfBuffer = pdfEditor.Save();

            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Fill_PDF_Forms.pdf";
            return fileResult;
        }

        private static void SetValue(PdfForm form, string fieldName, string value)
        {
            if (form.ContainsField(fieldName))
                form.SetValue(fieldName, value ?? string.Empty);
        }

        // Lists the fields of the form with their type and value on a page added at the end of the document
        private void AddFieldsListPage(PdfEditor pdfEditor)
        {
            int pageNumber = pdfEditor.AddPage(PdfPageSize.A4, PdfPageOrientation.Portrait);

            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf"));
            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 14f, PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont textFont = PdfFontManager.CreateFont(baseFont, 9f, PdfFontStyle.Normal, PdfColor.Black);

            StringBuilder text = new StringBuilder();
            foreach (PdfFormFieldInfo field in pdfEditor.Form.Fields)
            {
                string value = field.Type == PdfFormFieldType.ListBox && field.Values != null && field.Values.Count > 0
                    ? string.Join(", ", field.Values) : field.Value;
                text.Append(field.Name).Append(" (").Append(field.Type).Append(")");
                if (field.ReadOnly)
                    text.Append(" read-only");
                if (field.Required)
                    text.Append(" required");
                text.Append(": ").Append(value ?? string.Empty).Append('\n');
            }

            // The positions are relative to the page in an edited document
            pdfEditor.AddText(pageNumber, new PdfTextElement("Form Fields", titleFont) { X = 40, Y = 40 });
            pdfEditor.AddText(pageNumber, new PdfTextElement(text.ToString(), textFont) { X = 40, Y = 70, Width = 515 });
        }

        private async Task<byte[]> ReadInputPdf(Fill_PDF_Forms_ViewModel model)
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

        private Fill_PDF_Forms_ViewModel SetViewModel()
        {
            var model = new Fill_PDF_Forms_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Fill_PDF_Forms".Length);

            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/PDF_Form.pdf";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
