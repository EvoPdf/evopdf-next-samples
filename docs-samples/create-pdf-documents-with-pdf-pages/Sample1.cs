// The letterhead page covers the whole current page, under the content added later
PdfPageElement letterhead = new PdfPageElement(letterheadPath, 1)
{
    X = -margins.Left,
    Y = -margins.Top,
    RenderUnderContent = true
};
pdfDocument.AddPdfPage(letterhead);

// A page of another document scaled to fit a rectangle, keeping its proportions
PdfPageElement pageElement = new PdfPageElement(sourcePdfBytes, 2)
{
    X = 0,
    Y = crtYPos,
    Width = 150,
    Height = 210,
    Opacity = 1,
    RotationDegrees = 0,
    IncludeAnnotations = true
};
PdfElementRenderInfo renderInfo = pdfDocument.AddPdfPage(pageElement);
PdfRectangleF bounds = renderInfo.LastPageRectangle.Bounds;
