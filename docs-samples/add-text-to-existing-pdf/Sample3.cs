PdfTextElement longText = new PdfTextElement(text, font)
{
    X = 36,
    Y = crtYPos,
    Width = contentWidth,
    ContinueOnNextPage = true,
    // The pages where the text continues are inserted after the current page
    InsertContinuationPages = true,
    ContinuationTopMargin = 36,
    ContinuationBottomMargin = 36
};
PdfTextRenderInfo info = pdfEditor.AddText(currentPage, longText);
currentPage = info.LastPageRectangle.PageNumber;
