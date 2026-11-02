PdfHtmlHeaderFooter footer = htmlToPdfConverter.PdfDocumentOptions.PdfHtmlFooter;
footer.PageNumberOffset = 10;
footer.TotalPagesOffset = 10;
footer.PageCounterFontScale = 1.2f;
footer.PageNumbersFont = PdfFontManager.CreateBaseFont(fontFilePath);
footer.ShowOnlyInHtmlToPdfPages = false;
