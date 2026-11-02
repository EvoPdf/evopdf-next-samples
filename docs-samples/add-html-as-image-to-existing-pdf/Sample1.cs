using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, null);

int pageNumber = pdfEditor.AddPage();
PdfHtmlImageElement htmlImage = new PdfHtmlImageElement(htmlCode, baseUrl)
{
    X = 36,
    Y = 72,
    Resolution = 150,
    ContinueOnNextPage = true,
    // The pages where the image continues are inserted after the current page
    InsertContinuationPages = true,
    ContinuationTopMargin = 36,
    ContinuationBottomMargin = 36
};
PdfImageRenderInfo htmlImageInfo = pdfEditor.AddHtmlImage(pageNumber, htmlImage);

byte[] outPdfBuffer = pdfEditor.Save();
