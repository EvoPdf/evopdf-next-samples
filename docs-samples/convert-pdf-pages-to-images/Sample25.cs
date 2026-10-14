await pdfToImageConverter.ConvertToImageFilesAsync(inputPdfBytes, startPageNumber, endPageNumber, outputDirectory, imageFileName);
await pdfToImageConverter.ConvertToImageFilesAsync(inputPdfStream, startPageNumber, endPageNumber, outputDirectory, imageFileName);
await pdfToImageConverter.ConvertToImageFilesAsync(inputPdfFile, startPageNumber, endPageNumber, outputDirectory, imageFileName);
