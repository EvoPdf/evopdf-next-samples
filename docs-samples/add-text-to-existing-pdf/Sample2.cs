PdfTextElement element = new PdfTextElement(text, font) { X = 0, Y = crtYPos, Width = contentWidth };
var info = pdfEditor.AddText(currentPage, element);
currentPage = info.LastPageRectangle.PageNumber;        // follow the engine if it overflowed
crtYPos = (int)info.LastPageRectangle.Bounds.Bottom + ySeparator;
