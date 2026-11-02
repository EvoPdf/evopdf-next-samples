using PdfRedactor pdfRedactor = new PdfRedactor(inputPdfBytes, password);

// Every occurrence of a name, as a whole word
pdfRedactor.RedactText("Jane Smith", false, true);

// The texts that match a regular expression, for example the customer numbers
pdfRedactor.RedactText(new Regex(@"C-\d{4}-\d{5}"));

byte[] redactedPdf = pdfRedactor.Save();
