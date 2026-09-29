PdfTextRenderInfo textRenderInfo = pdfDocument.AddText(pdfText);

// Use the rendered geometry to position the next element
crtYPos = (int)textRenderInfo.LastPageRectangle.Bounds.Bottom + ySeparator;
