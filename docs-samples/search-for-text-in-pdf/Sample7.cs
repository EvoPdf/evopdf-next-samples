FindTextLocation[] findTextLocations = await pdfToTextConverter.FindTextAsync(inputPdfStream, textToFindString, startPageNumber, caseSensitive, wholeWord);
FindTextLocation[] findTextLocations = await pdfToTextConverter.FindTextAsync(inputPdfFile, textToFindString, startPageNumber, caseSensitive, wholeWord);
