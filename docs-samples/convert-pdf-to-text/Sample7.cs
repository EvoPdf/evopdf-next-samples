string extractedText = await pdfToTextConverter.ConvertToTextAsync(inputPdfStream, startPageNumber);
string extractedText = await pdfToTextConverter.ConvertToTextAsync(inputPdfFile, startPageNumber);
