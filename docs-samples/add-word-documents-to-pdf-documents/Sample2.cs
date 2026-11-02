// The rendered area reported for the last page places the elements after the content
crtYPos = wordRenderInfo.LastPageRectangle.Bounds.Bottom + 15;

PdfTextElement closingElement = new PdfTextElement("This text was added after the Word content", textFont)
{
    X = 0,
    Y = crtYPos,
    Width = pdfDocument.ContentWidth
};
pdfDocument.AddText(closingElement);
