HtmlToPdfConverter converter = new HtmlToPdfConverter();

// One page 227 points (80 mm) wide with 14 point (5 mm) margins, as tall as the content
converter.SinglePageOfWidth(227, 14);

byte[] pdf = converter.ConvertHtml(receiptHtml, baseUrl);

// The same settings made one by one: 5 mm margins are 14 points
converter.PdfDocumentOptions.LeftMargin = 14;
converter.PdfDocumentOptions.RightMargin = 14;
converter.PdfDocumentOptions.TopMargin = 14;
converter.PdfDocumentOptions.BottomMargin = 14;
converter.PdfDocumentOptions.PdfPageSize = new PdfPageSize(227, 227);
converter.PdfDocumentOptions.AutoResizePdfPageWidth = false;
converter.PdfDocumentOptions.AutoResizePdfPageHeight = true;
// the content width in pixels: (227 - 2 * 14) * 4 / 3, rounded down
converter.HtmlViewerWidth = 265;
converter.HtmlViewerZoom = 100;
