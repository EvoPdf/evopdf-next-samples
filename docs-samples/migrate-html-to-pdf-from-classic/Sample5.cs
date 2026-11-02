HtmlToPdfConverter converter = new HtmlToPdfConverter();

// Same as Classic: A4 page, a 1600 pixel window scaled to it, drawn at 49.58 percent
converter.FitBrowserWindowToPage(PdfPageSize.A4, PdfPageOrientation.Portrait, 1600);

// Or a 1200 point wide page with the content at 1:1
converter.PageWidthFromBrowserWindow(1600);
