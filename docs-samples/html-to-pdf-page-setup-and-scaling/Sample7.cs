HtmlToPdfConverter converter = new HtmlToPdfConverter();

// The page width follows the viewer width, the page height follows the content
converter.PageWidthFromBrowserWindow(1024, singlePage: true);

byte[] pdf = converter.ConvertUrl(url);

// The same settings made one by one
converter.HtmlViewerWidth = 1024;
converter.PdfDocumentOptions.AutoResizePdfPageWidth = true;
converter.PdfDocumentOptions.AutoResizePdfPageHeight = true;
