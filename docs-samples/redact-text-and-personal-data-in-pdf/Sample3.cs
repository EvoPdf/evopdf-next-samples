using PdfRedactor pdfRedactor = new PdfRedactor(inputPdfBytes, password);
pdfRedactor.RedactText("Jane Smith", false, true);
pdfRedactor.RedactPersonalData(PdfPersonalDataTypes.All);
// The document with the redaction marks, to review before the content is removed
byte[] markedPdf = pdfRedactor.SaveRedactionMarks();
