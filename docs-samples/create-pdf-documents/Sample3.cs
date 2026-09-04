// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/create-pdf-documents.htm
// Documentation page: Create PDF Documents

// Set the next page to landscape A4
pdfDocument.SetPageSize(PdfPageSize.A4, PdfPageOrientation.Landscape);

// Set the next page margins
pdfDocument.Margins = new PdfMargins(100, 100, 100, 100);

// Add a new PDF page with the modified page settings
pdfDocument.AddPage();
