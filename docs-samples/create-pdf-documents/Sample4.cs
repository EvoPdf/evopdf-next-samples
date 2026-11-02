// The margins of the pages added next
pdfDocument.Margins = new PdfMargins(100, 100, 100, 100);

// A landscape A4 page; the pages added next keep this size
pdfDocument.AddPage(PdfPageSize.A4, PdfPageOrientation.Landscape);
