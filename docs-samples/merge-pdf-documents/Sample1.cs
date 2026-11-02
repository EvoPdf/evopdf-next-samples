using PdfMerge pdfMerge = new PdfMerge();

// The add methods return the number of the first added page in the merged document
int firstDocumentPage = pdfMerge.AddPdf(firstPdfBytes);

// A blank page between the documents
int blankPage = pdfMerge.AddBlankPage(PdfPageSize.A4, PdfPageOrientation.Portrait);

// Selected pages of the second document, in the order of the ranges
int secondDocumentPage = pdfMerge.AddPdfPages(secondPdfBytes, "1,3-", null);

byte[] mergedPdf = pdfMerge.Save();
