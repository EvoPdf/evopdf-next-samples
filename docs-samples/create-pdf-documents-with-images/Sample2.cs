string imagesPath = GetDemoImagesPath();

// Add a transparent PNG image with a custom width
PdfImageElement pdfPngImage = new PdfImageElement(
    Path.Combine(imagesPath, "transparent.png"))
{
    X = crtXPos,
    Y = crtYPos,
    Width = 150
};
