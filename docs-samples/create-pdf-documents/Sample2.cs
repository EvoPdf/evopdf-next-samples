PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
{
  PageSize = PdfPageSize.A4,
  PageOrientation = PdfPageOrientation.Portrait,
  Margins = new PdfMargins(36, 36, 36, 36)
};

// Create a new PDF document with the specified settings
using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);
