string fontFilePath = Path.Combine(fontsPath, "DejaVuSerif.ttf");

// Load the font file into a base font (cached by physical path)
PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);

// Derive a styled PdfFont from the base font
PdfFont trueTypeFont = PdfFontManager.CreateFont(
    baseFont, 16f,
    PdfFontStyle.Normal, PdfColor.Black);
