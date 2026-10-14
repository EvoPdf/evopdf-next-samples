// A4 landscape in the default layout, with margins set after the call:
// the zoom is computed for the landscape page and the margins
converter.FitBrowserWindowToPage(PdfPageSize.A4);
converter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Landscape;
converter.PdfDocumentOptions.LeftMargin = 36;
converter.PdfDocumentOptions.RightMargin = 36;

// Setting the zoom ends the automatic layout: the page is laid out for zoom 90, whatever its size
converter.HtmlViewerZoom = 90;
