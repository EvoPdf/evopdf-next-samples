await pdfToImageConverter.ConvertToImageFilesAsync(inputPdfBytes, outputDirectory, imageFileName);
await pdfToImageConverter.ConvertToImageFilesAsync(inputPdfStream, outputDirectory, imageFileName);
await pdfToImageConverter.ConvertToImageFilesAsync(inputPdfFile, outputDirectory, imageFileName);
