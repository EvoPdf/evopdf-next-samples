// An invoice template designed for A4
converter.LayoutAtPageWidth(PdfPageSize.A4);

// A template whose @page rule sets the paper size
converter.LayoutAtPageWidth(PdfPageSize.A4, PdfPageOrientation.Portrait, preferCssPageSize: true);

// A template with a print style sheet
converter.LayoutAtPageWidth(PdfPageSize.A4, PdfPageOrientation.Portrait, mediaType: "print");

// A web page laid out at the width of a landscape page
converter.LayoutAtPageWidth(PdfPageSize.A4, PdfPageOrientation.Landscape);
