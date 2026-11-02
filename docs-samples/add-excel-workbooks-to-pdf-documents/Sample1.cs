PdfExcelElement excelElement = new PdfExcelElement(inputExcelBytes)
{
    X = 0,
    Y = crtYPos,
    // 0 extends the content to the right margin of the page
    Width = 0,
    Zoom = 100
};

PdfContentRenderInfo excelRenderInfo = pdfDocument.AddExcel(excelElement);
