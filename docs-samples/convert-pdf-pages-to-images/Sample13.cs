await pdfToImageConverter.ConvertToImageFilesAsync(inputPdfBytes, startPageNumber, outputDirectory, imageFileName);
await pdfToImageConverter.ConvertToImageFilesAsync(inputPdfStream, startPageNumber, outputDirectory, imageFileName);
await pdfToImageConverter.ConvertToImageFilesAsync(inputPdfFile, startPageNumber, outputDirectory, imageFileName);
