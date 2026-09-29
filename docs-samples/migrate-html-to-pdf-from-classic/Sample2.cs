using EvoPdf.Next;

Licensing.LicenseKey = "...";
HtmlToPdfConverter converter = new HtmlToPdfConverter();

byte[] pdf = converter.ConvertUrl(url);

// The same settings made explicitly
converter.FitBrowserWindowToPage(PdfPageSize.A4);

// The same settings made one by one
converter.PdfDocumentOptions.AutoResizePdfPageWidth = false;
converter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
converter.HtmlViewerZoom = 77.47;
converter.HtmlViewerWidth = 1024;
