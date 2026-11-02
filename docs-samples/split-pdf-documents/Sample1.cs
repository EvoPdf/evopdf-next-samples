using PdfSplit pdfSplit = new PdfSplit(inputPdfBytes, null);

// One page, a range of pages and the pages of several ranges
byte[] pagePdf = pdfSplit.ExtractPage(2);
byte[] pagesPdf = pdfSplit.ExtractPages(2, 4);
byte[] rangesPdf = pdfSplit.ExtractPages("1,3-4,6-");

// The loaded document without some pages
byte[] remainingPdf = pdfSplit.RemovePages("2,5");
