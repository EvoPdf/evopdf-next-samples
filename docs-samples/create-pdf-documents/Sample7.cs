// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/create-pdf-documents.htm
// Documentation page: Create PDF Documents

PdfDocument pdfDocument = new PdfDocument();
byte[] buffer = pdfDocument.Save();
pdfDocument.Dispose();
