using PdfRedactor pdfRedactor = new PdfRedactor(inputPdfBytes, password);

// The payment details table of the first page
pdfRedactor.AddRedaction(1, new PdfRectangleF(36, 288, 525, 62));

// The signature, covered with an image that keeps its aspect ratio
PdfRedactionArea signatureArea = pdfRedactor.AddRedaction(1, new PdfRectangleF(50, 505, 200, 60));
signatureArea.SetOverlayImage(System.IO.File.ReadAllBytes("Redacted_Stamp.png"), true);

byte[] redactedPdf = pdfRedactor.Save();
