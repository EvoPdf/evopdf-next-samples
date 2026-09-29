PdfTextElement highlighted = new PdfTextElement(text, bodyFont)
{
    X = 0, Y = crtYPos, Width = pdfDocument.ContentWidth,
    BackgroundColor = PdfColor.Yellow,
    BackgroundOpacity = 0.4f
};
pdfDocument.AddText(highlighted);
