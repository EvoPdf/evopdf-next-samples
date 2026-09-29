// A4 portrait, the desktop layout of the page scaled to the page width: the settings of a new converter
converter.FitBrowserWindowToPage(PdfPageSize.A4);

// Letter landscape, a 1280 pixel window
converter.FitBrowserWindowToPage(PdfPageSize.Letter, PdfPageOrientation.Landscape, 1280);
