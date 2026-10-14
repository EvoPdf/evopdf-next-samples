HtmlToPdfConverter converter = new HtmlToPdfConverter();

converter.PrintLikeChrome(PdfPageSize.A4);

byte[] pdf = converter.ConvertUrl(url);

// The same settings made one by one
converter.PdfDocumentOptions.AutoResizePdfPageWidth = false;
converter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
// Chrome prints with 1 cm margins, print media type and no backgrounds
converter.PdfDocumentOptions.LeftMargin = 28;
converter.PdfDocumentOptions.RightMargin = 28;
converter.PdfDocumentOptions.TopMargin = 28;
converter.PdfDocumentOptions.BottomMargin = 28;
converter.MediaType = "print";
converter.PdfDocumentOptions.PrintBackgrounds = false;
// Viewer width equal to the content width: (595 - 56) * 4 / 3 = 719 pixels
converter.HtmlViewerWidth = 719;
