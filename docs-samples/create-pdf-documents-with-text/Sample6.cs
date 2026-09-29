PdfTextElement pdfText1 = new PdfTextElement(alfabetString, trueTypeFont)
{
    X = crtXPos,
    Y = crtYPos,
    Alignment = PdfTextAlignment.Left,
    ContinueOnNextPage = true
};

// Draw a blue border around the text rendered on each page
pdfText1.OnAfterPageRender = info =>
{
    var bounds = info.RenderedRectangle.Bounds;
    PdfRectangleElement border = new PdfRectangleElement(
        bounds.X, bounds.Y,
        bounds.Width, bounds.Height + 5)
    {
        BorderColor = PdfColor.Blue,
    };
    pdfDocument.AddRectangle(border);
};

pdfDocument.AddText(pdfText1);
