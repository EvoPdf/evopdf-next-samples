PdfBaseFont subsetFont = PdfFontManager.CreateBaseFont(fontFilePath);
PdfBaseFont fullFont = PdfFontManager.CreateBaseFont(fontFilePath, false);
PdfBaseFont latinFont = PdfFontManager.CreateBaseFont(fontFilePath, false, new[] { PdfUnicodeRanges.BasicLatin });
