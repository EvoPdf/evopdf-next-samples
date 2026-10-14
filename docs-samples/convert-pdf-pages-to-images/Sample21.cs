PdfPageImage[] pdfPageImages = await pdfToImageConverter.ConvertToImagesAsync(inputPdfStream, startPageNumber);
PdfPageImage[] pdfPageImages = await pdfToImageConverter.ConvertToImagesAsync(inputPdfFile, startPageNumber);
