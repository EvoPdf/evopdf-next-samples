await pdfImagesExtractor.ExtractImagesToFileAsync(inputPdfBytes, startPageNumber, endPageNumber, outputDirectory, imageFileName);
await pdfImagesExtractor.ExtractImagesToFileAsync(inputPdfStream, startPageNumber, endPageNumber, outputDirectory, imageFileName);
await pdfImagesExtractor.ExtractImagesToFileAsync(inputPdfFile, startPageNumber, endPageNumber, outputDirectory, imageFileName);
