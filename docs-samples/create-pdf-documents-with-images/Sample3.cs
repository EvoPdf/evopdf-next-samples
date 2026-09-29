PdfImageRenderInfo imageRenderInfo = pdfDocument.AddImage(pdfPngImage);

// Use the rendered geometry to position the next element
crtYPos = imageRenderInfo.BoundingBox.Bottom + ySeparator;
