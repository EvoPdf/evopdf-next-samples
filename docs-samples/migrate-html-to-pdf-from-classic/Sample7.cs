// Classic
converter.PdfDocumentOptions.FitWidth = true;
converter.PdfDocumentOptions.StretchToFit = true;

// Next: a 600 pixel window enlarged to the A4 page, drawn at 132 percent
converter.FitBrowserWindowToPage(PdfPageSize.A4, PdfPageOrientation.Portrait, 600);
