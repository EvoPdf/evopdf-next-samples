string extractedText = await pdfToTextConverter.ConvertToTextAsync(inputPdfStream, startPageNumber, endPageNumber);
string extractedText = await pdfToTextConverter.ConvertToTextAsync(inputPdfFile, startPageNumber, endPageNumber);
