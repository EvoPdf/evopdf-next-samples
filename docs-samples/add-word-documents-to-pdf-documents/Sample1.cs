PdfWordElement wordElement = new PdfWordElement(inputWordBytes)
{
    X = 0,
    Y = crtYPos,
    // 0 extends the content to the right margin of the page
    Width = 0,
    Zoom = 100,
    GenerateDocumentOutline = true
};

PdfContentRenderInfo wordRenderInfo = pdfDocument.AddWord(wordElement);
