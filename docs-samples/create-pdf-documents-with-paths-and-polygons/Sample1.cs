PdfPathElement heart = new PdfPathElement
{
    FillColor = PdfColor.Crimson,
    FillOpacity = 0.8f,
    LineColor = PdfColor.DarkRed,
    LineStyle = new PdfLineStyle { LineWidth = 1.5f, LineJoin = PdfLineJoinStyle.Round }
};
heart
    .MoveTo(cx, cy + s * 0.25f)
    .CurveTo(cx - s * 0.55f, cy + s * 0.55f,
             cx - s * 1.10f, cy - s * 0.10f,
             cx, cy - s * 0.35f)
    .CurveTo(cx + s * 1.10f, cy - s * 0.10f,
             cx + s * 0.55f, cy + s * 0.55f,
             cx, cy + s * 0.25f)
    .Close();
pdfDocument.AddPath(heart);
