await pdfImagesExtractor.ExtractImagesToFileAsync(inputPdfBytes, startPageNumber, outputDirectory, imageFileName);
await pdfImagesExtractor.ExtractImagesToFileAsync(inputPdfStream, startPageNumber, outputDirectory, imageFileName);
await pdfImagesExtractor.ExtractImagesToFileAsync(inputPdfFile, startPageNumber, outputDirectory, imageFileName);
