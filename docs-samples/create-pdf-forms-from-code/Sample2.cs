// The total is computed by the viewer from the other fields
totalField.Actions.Calculate = new PdfJavaScriptAction(
    "var prices = { standard: 350, premium: 500, student: 120 };" +
    "event.value = prices[this.getField('ticketType').value] || 0;");

// The buttons submit the form as FDF, reset it and print the document
submitButton.Action = new PdfSubmitFormAction("https://www.example.com/register") { Format = PdfSubmitFormat.Fdf };
resetButton.Action = new PdfResetFormAction(null);
printButton.Action = new PdfNamedAction(PdfNamedActionType.Print);

// The document level script defines a function used by the validate script of the email field
pdfDocument.AddJavaScript("function isValidEmail(value) { return value.length == 0 || value.indexOf('@') > 0; }");
emailField.Actions.Validate = new PdfJavaScriptAction(
    "if (!isValidEmail(event.value)) { app.alert('The email address is not valid'); event.rc = false; }");
