using System;

// Use EVO PDF Namespace
using EvoPdf.Next;

public byte[] CreatePdfWithGradientFills(string fontFilePath, float fillOpacity)
{
    // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
    // Licensing.LicenseKey = "your-license-key";

    PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
    {
        PageSize = PdfPageSize.A4,
        PageOrientation = PdfPageOrientation.Portrait,
        Margins = new PdfMargins(36, 36, 36, 36),
        Language = "en-US"
    };

    using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);
    pdfDocument.PdfDocumentInfo.Title = "PDF Gradient Fills Demo";

    PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
    PdfFont badgeFont = PdfFontManager.CreateFont(baseFont, 11f, PdfFontStyle.Bold, PdfColor.White);

    const int xLeft = 0;
    int crtYPos = 0;

    // ===== Section 1: the four gradient directions on rectangles =====
    PdfGradientDirection[] directions =
    {
        PdfGradientDirection.Vertical, PdfGradientDirection.Horizontal,
        PdfGradientDirection.ForwardDiagonal, PdfGradientDirection.BackwardDiagonal
    };
    for (int i = 0; i < directions.Length; i++)
    {
        int boxX = xLeft + i * 130;

        // A gradient fill takes the place of the solid fill color: the shape is painted
        // from the start color to the end color along the given direction
        var rectangle = new PdfRectangleElement(boxX, crtYPos, 110, 60)
        {
            GradientFill = new PdfGradientFill(PdfColor.LightSkyBlue, PdfColor.DarkBlue, directions[i]),
            FillOpacity = fillOpacity,
            BorderColor = PdfColor.DarkBlue,
            Border = new PdfLineStyle { LineWidth = 1f }
        };
        pdfDocument.AddRectangle(rectangle);
    }
    crtYPos += 80;

    // ===== Section 2: rounded rectangles, circles and ellipses =====
    pdfDocument.AddRoundedRectangle(new PdfRoundedRectangleElement(xLeft, crtYPos, 140, 70, 16f)
    {
        GradientFill = new PdfGradientFill(PdfColor.Gold, PdfColor.DarkOrange, PdfGradientDirection.ForwardDiagonal),
        FillOpacity = fillOpacity,
        BorderColor = PdfColor.DarkOrange,
        Border = new PdfLineStyle { LineWidth = 1f }
    });

    pdfDocument.AddCircle(new PdfCircleElement(xLeft + 215, crtYPos + 35, 35)
    {
        GradientFill = new PdfGradientFill(PdfColor.White, PdfColor.Crimson, PdfGradientDirection.Vertical),
        FillOpacity = fillOpacity,
        BorderColor = PdfColor.Crimson,
        Border = new PdfLineStyle { LineWidth = 1f }
    });

    pdfDocument.AddEllipse(new PdfEllipseElement(xLeft + 300, crtYPos, 150, 70)
    {
        GradientFill = new PdfGradientFill(PdfColor.LightGreen, PdfColor.DarkGreen, PdfGradientDirection.Horizontal),
        FillOpacity = fillOpacity,
        BorderColor = PdfColor.DarkGreen,
        Border = new PdfLineStyle { LineWidth = 1f },
        RotationDegrees = 20,
        RotationPivot = PdfRotationPivot.Center
    });
    crtYPos += 100;

    // ===== Section 3: arcs, polygons and paths =====

    // Only a closed arc (chord or pie) has an area to fill
    pdfDocument.AddArc(new PdfArcElement(xLeft, crtYPos, 120, 90, startAngleDegrees: 30, sweepAngleDegrees: 240)
    {
        Closure = PdfArcClosureType.Pie,
        GradientFill = new PdfGradientFill(PdfColor.Plum, PdfColor.Purple, PdfGradientDirection.BackwardDiagonal),
        FillOpacity = fillOpacity,
        BorderColor = PdfColor.Purple,
        Border = new PdfLineStyle { LineWidth = 1f }
    });

    float hexagonCenterX = xLeft + 220, hexagonCenterY = crtYPos + 45, hexagonRadius = 45;
    var hexagonPoints = new PdfPointF[6];
    for (int i = 0; i < 6; i++)
    {
        double angle = Math.PI / 3 * i;
        hexagonPoints[i] = new PdfPointF(hexagonCenterX + (float)(hexagonRadius * Math.Cos(angle)),
            hexagonCenterY + (float)(hexagonRadius * Math.Sin(angle)));
    }
    pdfDocument.AddPolygon(new PdfPolygonElement(hexagonPoints)
    {
        GradientFill = new PdfGradientFill(PdfColor.LightCyan, PdfColor.Teal, PdfGradientDirection.Vertical),
        FillOpacity = fillOpacity,
        BorderColor = PdfColor.Teal,
        Border = new PdfLineStyle { LineWidth = 1f }
    });

    // A heart drawn with two cubic curves; the gradient fills the closed path
    float cx = xLeft + 400, cy = crtYPos + 50, s = 55;
    PdfPathElement heart = new PdfPathElement
    {
        GradientFill = new PdfGradientFill(PdfColor.LightPink, PdfColor.DarkRed, PdfGradientDirection.Vertical),
        FillOpacity = fillOpacity,
        BorderColor = PdfColor.DarkRed,
        Border = new PdfLineStyle { LineWidth = 1f, LineJoin = PdfLineJoinStyle.Round }
    };
    heart.MoveTo(cx, cy + s * 0.25f)
        .CurveTo(cx - s * 0.55f, cy + s * 0.55f, cx - s * 1.10f, cy - s * 0.10f, cx, cy - s * 0.35f)
        .CurveTo(cx + s * 1.10f, cy - s * 0.10f, cx + s * 0.55f, cy + s * 0.55f, cx, cy + s * 0.25f)
        .Close();
    pdfDocument.AddPath(heart);
    crtYPos += 120;

    // ===== Section 4: badges sized with the measured text width =====
    string[] badgeTexts = { "NEW", "GRADIENT FILL", "PdfFont.GetTextWidth", "PDF" };
    PdfColor[] badgeStartColors = { PdfColor.LightGreen, PdfColor.LightSkyBlue, PdfColor.Gold, PdfColor.Plum };
    PdfColor[] badgeEndColors = { PdfColor.DarkGreen, PdfColor.DarkBlue, PdfColor.DarkOrange, PdfColor.Purple };
    float badgeX = xLeft;
    const float badgeHeight = 24f, badgePadding = 10f;
    for (int i = 0; i < badgeTexts.Length; i++)
    {
        // The width of the text written with the font gives the width of its badge
        float textWidth = badgeFont.GetTextWidth(badgeTexts[i]);
        float badgeWidth = textWidth + 2 * badgePadding;

        pdfDocument.AddRoundedRectangle(new PdfRoundedRectangleElement(badgeX, crtYPos, badgeWidth, badgeHeight, badgeHeight / 2)
        {
            GradientFill = new PdfGradientFill(badgeStartColors[i], badgeEndColors[i], PdfGradientDirection.Vertical),
            FillOpacity = fillOpacity,
            BorderColor = badgeEndColors[i],
            Border = new PdfLineStyle { LineWidth = 0.5f }
        });

        // The text is centered in the badge from the measured width
        PdfTextElement badgeText = new PdfTextElement(badgeTexts[i], badgeFont)
        {
            X = badgeX + badgePadding,
            Y = crtYPos + (badgeHeight - badgeFont.Size) / 2 - 1,
            Width = textWidth + 2
        };
        pdfDocument.AddText(badgeText);

        badgeX += badgeWidth + 12;
    }

    byte[] outPdfBuffer = pdfDocument.Save();

    return outPdfBuffer;
}
