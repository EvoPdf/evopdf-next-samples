// First step: the marks, saved for a review
using (PdfRedactor marker = new PdfRedactor(inputPdfBytes, password))
{
    marker.OverlayText = "REDACTED";
    marker.RedactPersonalData(PdfPersonalDataTypes.All);
    marker.SaveRedactionMarksToFile("Document_with_Redaction_Marks.pdf");
}

// Second step: the reviewed marks, applied
using (PdfRedactor pdfRedactor = new PdfRedactor("Document_with_Redaction_Marks.pdf"))
{
    pdfRedactor.AddRedactionAnnotations();
    pdfRedactor.SaveToFile("Redacted_Document.pdf");
}
