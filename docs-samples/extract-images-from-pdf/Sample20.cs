await pdfImagesExtractor.ExtractImagesToFileAsync(inputPdfBytes, outputDirectory, imageFileName);
await pdfImagesExtractor.ExtractImagesToFileAsync(inputPdfStream, outputDirectory, imageFileName);
await pdfImagesExtractor.ExtractImagesToFileAsync(inputPdfFile, outputDirectory, imageFileName);
