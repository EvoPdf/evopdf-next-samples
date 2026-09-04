// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/create-pdf-documents-with-text.htm
// Documentation page: Create PDF Documents with Text

string rtlFontFilePath = Path.Combine(fontsPath, "NotoSansArabic-Regular.ttf");

// Two-step font creation also works for one-off use
PdfBaseFont rtlBaseFont = PdfFontManager.CreateBaseFont(rtlFontFilePath);
PdfFont rtlTrueTypeFont = PdfFontManager.CreateFont(
    rtlBaseFont, 16f,
    PdfFontStyle.Normal, PdfColor.Black);

string rtlString = System.IO.File.ReadAllText(
    Path.Combine(textsPath, "RightToLeft.txt"));

PdfTextElement pdfTextRtl = new PdfTextElement(rtlString, rtlTrueTypeFont)
{
    X = crtXPos,
    Y = crtYPos,
    Direction = PdfTextDirection.RightToLeft
};
pdfDocument.AddText(pdfTextRtl);
