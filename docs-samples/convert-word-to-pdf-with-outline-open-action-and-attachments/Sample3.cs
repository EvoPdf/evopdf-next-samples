// The action executed when the document is opened: go to the first page of the
// converted content, fit to the page width
wordToPdfConverter.PdfDocumentOptions.OpenAction = new PdfGoToPageAction(firstContentPage, PdfLinkPageLocation.FitWidth(0));
