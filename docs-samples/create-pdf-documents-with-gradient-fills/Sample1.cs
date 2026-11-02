PdfRectangleElement rectangle = new PdfRectangleElement(0, 0, 110, 60)
{
    GradientFill = new PdfGradientFill(PdfColor.LightSkyBlue, PdfColor.DarkBlue, PdfGradientDirection.Horizontal),
    BorderColor = PdfColor.DarkBlue,
    Border = new PdfLineStyle { LineWidth = 1f }
};
pdfDocument.AddRectangle(rectangle);
