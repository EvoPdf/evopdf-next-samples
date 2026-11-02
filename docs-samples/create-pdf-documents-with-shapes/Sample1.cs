PdfLineElement dashedLine = new PdfLineElement(0, 100, 380, 100)
{
    LineColor = PdfColor.DarkRed,
    LineOpacity = 0.5f,
    LineStyle = new PdfLineStyle { LineWidth = 2f, CustomDashPattern = new float[] { 10f, 4f } }
};
pdfDocument.AddLine(dashedLine);
