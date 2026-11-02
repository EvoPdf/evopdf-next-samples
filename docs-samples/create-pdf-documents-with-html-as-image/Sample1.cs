PdfHtmlImageElement htmlImage = new PdfHtmlImageElement(htmlCode, baseUrl)
{
    X = 0,
    Y = crtYPos,
    // 0 draws the image at the size of the content on screen, scaled down to the right margin when it is wider
    Width = 0,
    // 0 gives no height limit
    Height = 0,
    // The image has the same size in the page at any resolution; a higher resolution gives a sharper image
    Resolution = 150
};
htmlImage.Accessibility.AlternateText = "Sales table rendered from HTML";

PdfImageRenderInfo htmlImageInfo = pdfDocument.AddHtmlImage(htmlImage);
