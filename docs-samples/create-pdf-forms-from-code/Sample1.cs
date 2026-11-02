// A text field with a maximum length displayed in separate cells
PdfTextField accessCodeField = new PdfTextField("accessCode", 1, 130, 110, 120, 22)
{
    Font = fieldFont,
    BorderColor = PdfColor.Gray,
    MaxLength = 6,
    Comb = true,
    Description = "Six character access code"
};
pdfDocument.AddFormField(accessCodeField);

// A combo box with a selected option
PdfComboBoxField countryField = new PdfComboBoxField("country", 1, 130, 144, 180, 22) { Font = fieldFont };
countryField.AddOption("Austria", "AT");
countryField.AddOption("Romania", "RO");
countryField.SelectedValue = "RO";
pdfDocument.AddFormField(countryField);

// A group of radio buttons; each button is placed in a page
PdfRadioGroupField ticketField = new PdfRadioGroupField("ticketType") { SelectedValue = "standard" };
ticketField.AddButton("standard", 1, 130, 181, 14, 14);
ticketField.AddButton("premium", 1, 230, 181, 14, 14);
pdfDocument.AddFormField(ticketField);
