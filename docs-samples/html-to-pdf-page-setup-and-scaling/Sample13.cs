HtmlToPdfConverter converter = new HtmlToPdfConverter();

// A phone window of 412 pixels enlarged to an A4 page
converter.FitBrowserWindowToPage(PdfPageSize.A4, PdfPageOrientation.Portrait, 412);

byte[] pdf = converter.ConvertUrl(url);

// The same settings made one by one
converter.PdfDocumentOptions.AutoResizePdfPageWidth = false;
converter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
// Lay the page out at 793 / 1.9256 = 412 pixels and draw it at 192.56 percent
converter.HtmlViewerZoom = 192.56;
converter.HtmlViewerWidth = 412;
