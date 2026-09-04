// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-text-to-existing-pdf.htm
// Documentation page: Add Text to Existing PDF

PdfTextElement element = new PdfTextElement(text, font) { X = 0, Y = crtYPos, Width = contentWidth };
var info = pdfEditor.AddText(currentPage, element);
currentPage = info.LastPageRectangle.PageNumber;        // follow the engine if it overflowed
crtYPos = (int)info.LastPageRectangle.Bounds.Bottom + ySeparator;
