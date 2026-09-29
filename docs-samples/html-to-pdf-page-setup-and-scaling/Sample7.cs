HtmlToPdfConverter converter = new HtmlToPdfConverter();

// A4 portrait page, the 1024 pixel browser window scaled to it
converter.FitBrowserWindowToPage(PdfPageSize.A4);

byte[] pdf = converter.ConvertUrl("https://www.evopdf.com");

// The same settings made one by one
converter.PdfDocumentOptions.AutoResizePdfPageWidth = false;
converter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
converter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
// Lay the page out at 793 / 0.7747 = 1024 pixels and draw it at 77.47 percent
converter.HtmlViewerZoom = 77.47;
converter.HtmlViewerWidth = 1024;
