using PdfRedactor pdfRedactor = new PdfRedactor(inputPdfBytes, password);

// The areas marked in a PDF viewer
int markedCount = pdfRedactor.AddRedactionAnnotations();

// Other redactions can be added to the same operation
pdfRedactor.RedactPersonalData(PdfPersonalDataTypes.All);

byte[] redactedPdf = pdfRedactor.Save();
