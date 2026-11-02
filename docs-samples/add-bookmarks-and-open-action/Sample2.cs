// The viewer opens the document at the second page, fitted to the window width
htmlToPdfConverter.PdfDocumentOptions.OpenAction = new PdfGoToPageAction(2, PdfLinkPageLocation.FitWidth(0));
