FindTextLocation[] findTextLocations = await pdfToTextConverter.FindTextAsync(inputPdfBytes, textToFindString, startPageNumber, caseSensitive, wholeWord);
