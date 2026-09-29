HtmlToPdfConverter converter = new HtmlToPdfConverter();

// A4 landscape, the HTML laid out at its 1123 pixel content width
converter.LayoutAtPageWidth(PdfPageSize.A4, PdfPageOrientation.Landscape);

// A table built for 1280 pixels: a 1280 pixel window scaled to the page, zoom 87.73
// converter.FitBrowserWindowToPage(PdfPageSize.A4, PdfPageOrientation.Landscape, 1280);

byte[] pdf = converter.ConvertUrl(url);
