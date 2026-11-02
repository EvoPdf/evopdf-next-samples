PdfHtmlElement htmlElement = new PdfHtmlElement(htmlString, baseUrl)
{
    X = 0,
    Y = crtYPos,
    // 0 extends the content to the right margin of the page
    Width = 0
};
PdfHtmlRenderInfo htmlRenderInfo = pdfDocument.AddHtml(htmlElement);

// The elements added after the content go below it, in its last page
crtYPos = htmlRenderInfo.LastPageRectangle.Bounds.Bottom + 15;
