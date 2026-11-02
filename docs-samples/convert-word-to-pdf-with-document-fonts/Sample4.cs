using System.IO;

// Use EVO PDF Namespace
using EvoPdf.Next;

public byte[] ConvertWordToPdfWithFonts(byte[] inputWordBytes, string fontsPath)
{
    // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
    // Licensing.LicenseKey = "your-license-key";

    // Create a Word to PDF converter object with default settings
    WordToPdfConverter wordToPdfConverter = new WordToPdfConverter();

    // Use the fonts stored in the Word document. Word embeds the fonts of a document when the
    // option to embed fonts is on when the document is saved, so the text keeps its look where
    // the fonts are not installed. When this option is off the stored fonts are ignored and the
    // text is rendered with the installed fonts
    wordToPdfConverter.UseEmbeddedFonts = true;

    // A font file given to the converter is used for the text set in its font family, in
    // place of an installed font with the same family name or when that font is not installed
    wordToPdfConverter.FontFiles.Add(Path.Combine(fontsPath, "DejaVuSerif.ttf"));

    // A fallback font file is used for the characters that no font of the document covers,
    // such as the Arabic text of the sample document
    wordToPdfConverter.FallbackFontFiles.Add(Path.Combine(fontsPath, "NotoSansArabic-Regular.ttf"));

    // The buffer to receive the generated PDF document
    byte[] outPdfBuffer = wordToPdfConverter.ConvertToPdf(inputWordBytes);

    return outPdfBuffer;
}
