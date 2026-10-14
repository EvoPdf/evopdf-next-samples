PdfPageImage[] pdfPageImages = await pdfToImageConverter.ConvertToImagesAsync(inputPdfStream, startPageNumber, endPageNumber);
PdfPageImage[] pdfPageImages = await pdfToImageConverter.ConvertToImagesAsync(inputPdfFile, startPageNumber, endPageNumber);
