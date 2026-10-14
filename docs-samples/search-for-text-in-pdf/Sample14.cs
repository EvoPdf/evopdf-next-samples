FindTextLocation[] findTextLocations = await pdfToTextConverter.FindTextAsync(inputPdfStream, textToFindString, caseSensitive, wholeWord);
FindTextLocation[] findTextLocations = await pdfToTextConverter.FindTextAsync(inputPdfFile, textToFindString, caseSensitive, wholeWord);
