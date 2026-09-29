ExtractedImage[][] extractedImages = await pdfImagesExtractor.ExtractImagesAsync(inputPdfStream, startPageNumber, endPageNumber);
ExtractedImage[][] extractedImages = await pdfImagesExtractor.ExtractImagesAsync(inputPdfFile, startPageNumber, endPageNumber);
