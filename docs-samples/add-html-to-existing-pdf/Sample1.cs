using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, null);

PdfHtmlElement htmlElement = new PdfHtmlElement(htmlString, baseUrl)
{
    X = 36,
    Y = 120,
    Width = 523,
    // The pages where the content continues are inserted after the first page
    InsertContinuationPages = true,
    ContinuationTopMargin = 72,
    ContinuationBottomMargin = 72
};
PdfHtmlRenderInfo htmlRenderInfo = pdfEditor.AddHtml(1, htmlElement);

// The elements added after the content go below it, in its last page
int lastPage = htmlRenderInfo.LastPageNumber;
float crtYPos = htmlRenderInfo.LastPageRectangle.Bounds.Bottom + 15;
