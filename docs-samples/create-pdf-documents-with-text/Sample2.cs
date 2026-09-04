// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/create-pdf-documents-with-text.htm
// Documentation page: Create PDF Documents with Text

PdfTextElement highlighted = new PdfTextElement(text, bodyFont)
{
    X = 0, Y = crtYPos, Width = pdfDocument.ContentWidth,
    BackgroundColor = PdfColor.Yellow,
    BackgroundOpacity = 0.4f
};
pdfDocument.AddText(highlighted);
