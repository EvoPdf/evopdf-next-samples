// The width of the text written with the font gives the width of its badge
float textWidth = badgeFont.GetTextWidth("GRADIENT FILL");
float badgeWidth = textWidth + 2 * badgePadding;

pdfDocument.AddRoundedRectangle(new PdfRoundedRectangleElement(badgeX, badgeY, badgeWidth, badgeHeight, badgeHeight / 2)
{
    GradientFill = new PdfGradientFill(PdfColor.LightSkyBlue, PdfColor.DarkBlue, PdfGradientDirection.Vertical),
    BorderColor = PdfColor.DarkBlue,
    Border = new PdfLineStyle { LineWidth = 0.5f }
});

// The text is centered in the badge from the measured width
PdfTextElement badgeText = new PdfTextElement("GRADIENT FILL", badgeFont)
{
    X = badgeX + badgePadding,
    Y = badgeY + (badgeHeight - badgeFont.Size) / 2 - 1,
    Width = textWidth + 2
};
pdfDocument.AddText(badgeText);
