HtmlToPdfConverter converter = new HtmlToPdfConverter();

// Margins in points; a @page rule in the template overrides them
converter.PdfDocumentOptions.LeftMargin = 36;
converter.PdfDocumentOptions.RightMargin = 36;
converter.PdfDocumentOptions.TopMargin = 36;
converter.PdfDocumentOptions.BottomMargin = 36;

// The exact page size, the template laid out at the page width with its print style sheet;
// preferCssPageSize lets a @page { size } rule in the template decide the page size
converter.LayoutAtPageWidth(PdfPageSize.A4, PdfPageOrientation.Portrait, preferCssPageSize: true, mediaType: "print");

byte[] pdf = converter.ConvertHtml(invoiceHtml, baseUrl);

// The same settings made one by one
converter.PdfDocumentOptions.AutoResizePdfPageWidth = false;
converter.PdfDocumentOptions.AutoResizePdfPageHeight = false;
converter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
converter.MediaType = "print";
// Viewer width equal to the content width: (595 - 72) * 4 / 3 = 697 pixels
converter.HtmlViewerWidth = 697;
converter.PdfDocumentOptions.PreferCssPageSize = true;
