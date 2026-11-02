using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, null);

for (int pageNumber = 1; pageNumber <= pdfEditor.PageCount; pageNumber++)
{
    // The letterhead page under the content of each page, at its own size
    PdfPageElement pageElement = new PdfPageElement(letterheadPdfBytes, 1)
    {
        X = 0,
        Y = 0,
        Opacity = 1,
        RenderUnderContent = true,
        IncludeAnnotations = true
    };
    PdfElementRenderInfo renderInfo = pdfEditor.AddPdfPage(pageNumber, pageElement);
}

byte[] outPdfBuffer = pdfEditor.Save();
