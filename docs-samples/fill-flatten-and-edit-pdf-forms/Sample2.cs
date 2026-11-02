PdfForm form = pdfEditor.Form;

form.SetValue("fullName", "John Smith");
form.SetValue("country", "DE");
form.SetValue("ticketType", "premium");
form.SetChecked("newsletter", true);
form.SetValues("sessions", new[] { "Keynote", "Panel discussion" });

// The values of a FDF file fill the fields with the same names
form.ImportData(fdfData);
