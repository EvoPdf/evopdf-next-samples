FindTextLocation[] findTextLocations = await pdfToTextConverter.FindTextAsync(inputPdfBytes, textToFindString, startPageNumber, endPageNumber, caseSensitive, wholeWord);
