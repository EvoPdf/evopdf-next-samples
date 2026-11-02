PdfTextElement multipage = new PdfTextElement(longText, bodyFont)
{
    X = 0,
    Y = crtYPos,
    Width = pdfDocument.ContentWidth,
    ContinueOnNextPage = true,
    Leading = 1.3f,
    OnPageRendered = (pageNumber, bounds) => Console.WriteLine("page {0}: {1}", pageNumber, bounds)
};
pdfDocument.AddText(multipage);

// The page numbers in a template repeated in all the pages
PdfTemplate footerTemplate = pdfDocument.AddPdfTemplate(0, pdfDocument.PageSize.Height - 30, pdfDocument.PageSize.Width, 20);
footerTemplate.AddText(new PdfTextElement("Page {page_number} of {total_pages}", smallFont)
{
    X = 0,
    Y = 0,
    Width = pdfDocument.PageSize.Width,
    Alignment = PdfTextAlignment.Center,
    PageNumberOffset = 0,
    TotalPagesOffset = 0
});
