// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/create-pdf-documents.htm
// Documentation page: Create PDF Documents

PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
{
  PageSize = PdfPageSize.A4,
  PageOrientation = PdfPageOrientation.Portrait,
  Margins = new PdfMargins(36, 36, 36, 36)
};

// Create a new PDF document with the specified settings
using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);
