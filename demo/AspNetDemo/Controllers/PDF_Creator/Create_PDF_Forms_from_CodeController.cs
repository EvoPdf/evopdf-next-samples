using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_Creator;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_Creator
{
    public class Create_PDF_Forms_from_CodeController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Create_PDF_Forms_from_CodeController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = new Create_PDF_Forms_from_Code_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePdf(Create_PDF_Forms_from_Code_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
            {
                PageSize = PdfPageSize.A4,
                PageOrientation = PdfPageOrientation.Portrait,
                Margins = new PdfMargins(40, 40, 40, 40),
                PdfStandard = model.PdfStandard,
                Language = "en-US"
            };

            using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);
            pdfDocument.PdfDocumentInfo.Title = "Conference Registration";

            // The fields are drawn as static content instead of interactive fields
            pdfDocument.Flatten = model.FlattenFields;

            // PDF/A documents have no actions and no scripts
            bool pdfA = PdfStandardIsPdfA(model.PdfStandard);
            bool addActions = model.AddFieldActions && !pdfA;

            string fontsPath = GetDemoFontsPath();
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(Path.Combine(fontsPath, "DejaVuSerif.ttf"));
            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 18f, PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont labelFont = PdfFontManager.CreateFont(baseFont, 11f, PdfFontStyle.Normal, PdfColor.Black);
            PdfFont noteFont = PdfFontManager.CreateFont(baseFont, 9f, PdfFontStyle.Normal, PdfColor.DarkGray);

            // The text of the fields is written with a standard font in a document without a standard and with
            // the embedded font in a PDF/A or PDF/UA document
            PdfFont fieldFont = model.PdfStandard == PdfStandard.None
                ? PdfFontManager.CreateStandardFont(PdfStandardFont.Helvetica, 10f, PdfColor.Black)
                : PdfFontManager.CreateFont(PdfFontManager.CreateBaseFont(Path.Combine(fontsPath, "DejaVuSerif.ttf"), false), 10f, PdfColor.Black);

            PdfColor borderColor = PdfColor.Gray;
            const float fieldLeft = 130;
            const float fieldWidth = 260;
            float crtYPos = 0;

            pdfDocument.AddText(new PdfTextElement("Conference Registration", titleFont) { X = 0, Y = crtYPos });
            crtYPos += 40;

            // The field positions are relative to the content area of the page, like the positions of the text elements
            AddLabel(pdfDocument, "Full name", labelFont, crtYPos);
            PdfTextField fullNameField = new PdfTextField("fullName", 1, fieldLeft, crtYPos, fieldWidth, 22)
            {
                Font = fieldFont,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                BackgroundColor = PdfColor.FromHex("#F4F6F8"),
                Description = "Full name",
                Required = model.RequiredFields,
                Text = "Jane Doe"
            };
            if (addActions)
            {
                // The name is shown in capitals while it is typed and when the field loses the focus
                fullNameField.Actions.Keystroke = new PdfJavaScriptAction("event.change = event.change.toUpperCase();");
                fullNameField.Actions.Format = new PdfJavaScriptAction("event.value = event.value.toUpperCase();");
            }
            pdfDocument.AddFormField(fullNameField);
            crtYPos += 34;

            AddLabel(pdfDocument, "Email", labelFont, crtYPos);
            PdfTextField emailField = new PdfTextField("email", 1, fieldLeft, crtYPos, fieldWidth, 22)
            {
                Font = fieldFont,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                Description = "Email address",
                Required = model.RequiredFields
            };
            if (addActions)
            {
                // The address is validated when the field is committed. The validation uses the function
                // defined by the document level script when the script is added to the document
                string validateScript = model.AddDocumentScript
                    ? "if (!isValidEmail(event.value)) { app.alert('The email address is not valid'); event.rc = false; }"
                    : "if (event.value.length > 0 && event.value.indexOf('@') < 0) { app.alert('The email address is not valid'); event.rc = false; }";
                emailField.Actions.Validate = new PdfJavaScriptAction(validateScript);
            }
            pdfDocument.AddFormField(emailField);
            crtYPos += 34;

            AddLabel(pdfDocument, "Access code", labelFont, crtYPos);
            pdfDocument.AddFormField(new PdfTextField("accessCode", 1, fieldLeft, crtYPos, 120, 22)
            {
                Font = fieldFont,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                MaxLength = 6,
                Comb = true,
                Description = "Six character access code"
            });
            crtYPos += 34;

            AddLabel(pdfDocument, "Country", labelFont, crtYPos);
            PdfComboBoxField countryField = new PdfComboBoxField("country", 1, fieldLeft, crtYPos, 180, 22)
            {
                Font = fieldFont,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                Description = "Country"
            };
            countryField.AddOption("Austria", "AT");
            countryField.AddOption("France", "FR");
            countryField.AddOption("Germany", "DE");
            countryField.AddOption("Italy", "IT");
            countryField.AddOption("Spain", "ES");
            countryField.SelectedValue = "DE";
            pdfDocument.AddFormField(countryField);
            crtYPos += 34;

            AddLabel(pdfDocument, "Ticket type", labelFont, crtYPos);
            PdfRadioGroupField ticketField = new PdfRadioGroupField("ticketType")
            {
                SelectedValue = "standard",
                SymbolColor = PdfColor.DarkBlue,
                Description = "Ticket type"
            };
            string[] ticketValues = { "standard", "premium", "student" };
            string[] ticketCaptions = { "Standard", "Premium", "Student" };
            for (int i = 0; i < ticketValues.Length; i++)
            {
                PdfRadioButton button = ticketField.AddButton(ticketValues[i], 1, fieldLeft + i * 100, crtYPos + 3, 14, 14);
                button.BorderColor = borderColor;
                button.BorderStyle = model.BorderStyle;
                pdfDocument.AddText(new PdfTextElement(ticketCaptions[i], labelFont) { X = fieldLeft + 20 + i * 100, Y = crtYPos + 3 });
            }
            pdfDocument.AddFormField(ticketField);
            crtYPos += 34;

            AddLabel(pdfDocument, "Sessions", labelFont, crtYPos);
            PdfListBoxField sessionsField = new PdfListBoxField("sessions", 1, fieldLeft, crtYPos, fieldWidth, 70)
            {
                Font = fieldFont,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                MultiSelect = true,
                Description = "Sessions to attend"
            };
            sessionsField.AddOption("Keynote");
            sessionsField.AddOption("Workshop: PDF generation");
            sessionsField.AddOption("Workshop: Accessibility");
            sessionsField.AddOption("Panel discussion");
            sessionsField.AddOption("Networking dinner");
            sessionsField.SelectedValues.Add("Keynote");
            pdfDocument.AddFormField(sessionsField);
            crtYPos += 82;

            AddLabel(pdfDocument, "Options", labelFont, crtYPos);
            pdfDocument.AddFormField(new PdfCheckBoxField("newsletter", 1, fieldLeft, crtYPos + 3, 14, 14)
            {
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                Style = model.CheckBoxStyle,
                Checked = true,
                Description = "Receive the newsletter"
            });
            pdfDocument.AddText(new PdfTextElement("Receive the newsletter", labelFont) { X = fieldLeft + 20, Y = crtYPos + 3 });
            pdfDocument.AddFormField(new PdfCheckBoxField("terms", 1, fieldLeft, crtYPos + 25, 14, 14)
            {
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                Style = model.CheckBoxStyle,
                Required = model.RequiredFields,
                Description = "Accept the terms"
            });
            pdfDocument.AddText(new PdfTextElement("I accept the terms and conditions", labelFont) { X = fieldLeft + 20, Y = crtYPos + 25 });
            crtYPos += 56;

            AddLabel(pdfDocument, "Comments", labelFont, crtYPos);
            pdfDocument.AddFormField(new PdfTextField("comments", 1, fieldLeft, crtYPos, fieldWidth, 70)
            {
                Font = fieldFont,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                Multiline = true,
                DoNotSpellCheck = true,
                Description = "Comments"
            });
            crtYPos += 82;

            AddLabel(pdfDocument, "Total (EUR)", labelFont, crtYPos);
            PdfTextField totalField = new PdfTextField("total", 1, fieldLeft, crtYPos, 100, 22)
            {
                Font = fieldFont,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                ReadOnly = true,
                Alignment = PdfTextAlignment.Right,
                Text = "350",
                Description = "Total amount"
            };
            if (addActions)
            {
                // The total follows the ticket type and the number of sessions
                totalField.Actions.Calculate = new PdfJavaScriptAction(
                    "var prices = { standard: 350, premium: 500, student: 120 };" +
                    "var ticket = this.getField('ticketType').value;" +
                    "var sessions = this.getField('sessions').value;" +
                    "var count = sessions == null ? 0 : (typeof sessions == 'string' ? 1 : sessions.length);" +
                    "event.value = (prices[ticket] || 0) + count * 25;");
            }
            pdfDocument.AddFormField(totalField);
            crtYPos += 40;

            // The buttons: submit, reset, print and the navigation to the second page
            PdfPushButtonField submitButton = new PdfPushButtonField("submit", 1, fieldLeft, crtYPos, 100, 26, "Submit")
            {
                Font = fieldFont,
                BackgroundColor = PdfColor.LightGray,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                Description = "Submit the form"
            };
            PdfPushButtonField resetButton = new PdfPushButtonField("reset", 1, fieldLeft + 110, crtYPos, 100, 26, "Reset")
            {
                Font = fieldFont,
                BackgroundColor = PdfColor.LightGray,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                Description = "Reset the form"
            };
            PdfPushButtonField printButton = new PdfPushButtonField("print", 1, fieldLeft + 220, crtYPos, 100, 26, "Print")
            {
                Font = fieldFont,
                BackgroundColor = PdfColor.LightGray,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                Description = "Print the form"
            };
            if (addActions)
            {
                // The form data is sent as FDF to the given address; the reset action clears all the fields
                // and the named action opens the print dialog of the viewer
                submitButton.Action = new PdfSubmitFormAction("https://www.example.com/register") { Format = PdfSubmitFormat.Fdf };
                resetButton.Action = new PdfResetFormAction(null);
                printButton.Action = new PdfNamedAction(PdfNamedActionType.Print);
            }
            pdfDocument.AddFormField(submitButton);
            pdfDocument.AddFormField(resetButton);
            pdfDocument.AddFormField(printButton);
            crtYPos += 40;

            PdfPushButtonField hideButton = new PdfPushButtonField("hideComments", 1, fieldLeft, crtYPos, 100, 26, "Hide comments")
            {
                Font = fieldFont,
                BackgroundColor = PdfColor.LightGray,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                Exportable = false
            };
            PdfPushButtonField nextPageButton = new PdfPushButtonField("nextPage", 1, fieldLeft + 110, crtYPos, 100, 26, "Next page")
            {
                Font = fieldFont,
                BackgroundColor = PdfColor.LightGray,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                Exportable = false
            };
            PdfPushButtonField siteButton = new PdfPushButtonField("website", 1, fieldLeft + 220, crtYPos, 100, 26, "Web site")
            {
                Font = fieldFont,
                BackgroundColor = PdfColor.LightGray,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                Exportable = false
            };
            if (addActions)
            {
                hideButton.Action = new PdfHideFieldsAction(new[] { "comments" }, true);
                nextPageButton.Action = new PdfGoToPageAction(2, PdfLinkPageLocation.FitPage());
                siteButton.Action = new PdfOpenUrlAction("https://www.example.com");
            }
            pdfDocument.AddFormField(hideButton);
            pdfDocument.AddFormField(nextPageButton);
            pdfDocument.AddFormField(siteButton);
            crtYPos += 44;

            pdfDocument.AddText(new PdfTextElement("The fields marked as required must be filled before the form is submitted", noteFont) { X = 0, Y = crtYPos });

            // The second page holds a field of the same radio group and a hidden field
            pdfDocument.AddPage();
            pdfDocument.AddText(new PdfTextElement("Accommodation", titleFont) { X = 0, Y = 0 });
            AddLabel(pdfDocument, "Hotel room", labelFont, 40);
            PdfRadioGroupField roomField = new PdfRadioGroupField("hotelRoom")
            {
                SymbolColor = PdfColor.DarkBlue,
                AllowDeselect = true,
                Description = "Hotel room"
            };
            roomField.AddButton("single", 2, fieldLeft, 43, 14, 14).BorderColor = borderColor;
            pdfDocument.AddText(new PdfTextElement("Single", labelFont) { X = fieldLeft + 20, Y = 43 });
            roomField.AddButton("double", 2, fieldLeft + 100, 43, 14, 14).BorderColor = borderColor;
            pdfDocument.AddText(new PdfTextElement("Double", labelFont) { X = fieldLeft + 120, Y = 43 });
            pdfDocument.AddFormField(roomField);

            AddLabel(pdfDocument, "Arrival date", labelFont, 74);
            pdfDocument.AddFormField(new PdfTextField("arrivalDate", 2, fieldLeft, 74, 120, 22)
            {
                Font = fieldFont,
                BorderColor = borderColor,
                BorderStyle = model.BorderStyle,
                Description = "Arrival date"
            });

            if (model.AddDocumentScript && !pdfA)
            {
                // The document level script defines the functions used by the scripts of the fields.
                // The script does not change the document when it is opened
                pdfDocument.AddJavaScript("function isValidEmail(value) { return value.length == 0 || value.indexOf('@') > 0; }");
            }

            byte[] outPdfBuffer = pdfDocument.Save();

            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Create_PDF_Forms_from_Code.pdf";
            return fileResult;
        }

        private static void AddLabel(PdfDocument pdfDocument, string text, PdfFont font, float yPos)
        {
            pdfDocument.AddText(new PdfTextElement(text, font) { X = 0, Y = yPos + 4 });
        }

        private static bool PdfStandardIsPdfA(PdfStandard pdfStandard)
        {
            return pdfStandard.ToString().Contains("PdfA");
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
