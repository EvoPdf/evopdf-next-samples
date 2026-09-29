FindTextLocation[] findTextLocations = await pdfToTextConverter.FindTextAsync(inputPdfStream, textToFindString, startPageNumber, endPageNumber, caseSensitive, wholeWord);
FindTextLocation[] findTextLocations = await pdfToTextConverter.FindTextAsync(inputPdfFile, textToFindString, startPageNumber, endPageNumber, caseSensitive, wholeWord);
