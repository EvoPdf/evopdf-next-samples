HtmlToPdfConverter converter = new HtmlToPdfConverter();

// The page width follows a 1000 pixel window: 1000 * 0.75 = 750 points
converter.PageWidthFromBrowserWindow(1000);

// A4 height for each page; PageWidthFromBrowserWindow(1000, singlePage: true) gives one page instead
converter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;

byte[] pdf = converter.ConvertUrl(url);

// The same settings made one by one
converter.PdfDocumentOptions.AutoResizePdfPageWidth = true;
converter.HtmlViewerWidth = 1000;
