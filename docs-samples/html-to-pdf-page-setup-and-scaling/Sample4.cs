// The same as converter.PrintLikeChrome(PdfPageSize.Letter)
converter.PdfDocumentOptions.LeftMargin = 28;
converter.PdfDocumentOptions.RightMargin = 28;
converter.PdfDocumentOptions.TopMargin = 28;
converter.PdfDocumentOptions.BottomMargin = 28;
converter.PdfDocumentOptions.PrintBackgrounds = false;
converter.LayoutAtPageWidth(PdfPageSize.Letter, PdfPageOrientation.Portrait, false, "print");
