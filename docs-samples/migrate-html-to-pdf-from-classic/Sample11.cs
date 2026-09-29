converter.PdfDocumentOptions.PdfHtmlHeader.OnPageRendering = (placement) =>
{
    return placement.DocumentPageNumber >= 3 && placement.DocumentPageNumber <= 10;
};
