// Insert a blank page of the chosen size before the first page. The existing pages
// are shifted and keep their content, links and bookmarks
int coverPageNumber = pdfEditor.InsertPage(1, PdfPageSize.A4, PdfPageOrientation.Portrait);

// Add a blank page at the end of the document, with the size of the last page
int lastPageNumber = pdfEditor.AddPage();
