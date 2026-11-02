// Use EVO PDF Namespace
using EvoPdf.Next;

public byte[] AddWordToPdfDocument(byte[] inputWordBytes, string fontFilePath)
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
    pdfDocument.PdfDocumentInfo.Title = "Word Content in a PDF Document";

    PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
    PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 16f, PdfFontStyle.Bold, PdfColor.DarkBlue);
    PdfFont textFont = PdfFontManager.CreateFont(baseFont, 10f, PdfFontStyle.Normal, PdfColor.Black);

    // Elements created with the PDF creator before the document content
    PdfTextElement titleElement = new PdfTextElement("Word Content in a PDF Document", titleFont)
    {
        X = 0,
        Y = 0,
        Width = pdfDocument.ContentWidth,
        Alignment = PdfTextAlignment.Center
    };
    titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
    float crtYPos = pdfDocument.AddText(titleElement).LastPageRectangle.Bounds.Bottom + 10;

    PdfTextElement introElement = new PdfTextElement(
        "The text above and this paragraph are text elements of the PDF document. The content of the Word " +
        "document starts below them and continues on new pages when it does not fit in this one. " +
        "The elements added after the Word content go to its last page, below it.", textFont)
    {
        X = 0,
        Y = crtYPos,
        Width = pdfDocument.ContentWidth
    };
    crtYPos = pdfDocument.AddText(introElement).LastPageRectangle.Bounds.Bottom + 15;

    // The Word document rendered in the PDF document, from the given position of the current
    // page and inside the page margins. The X position is kept on the next pages
    PdfWordElement wordElement = new PdfWordElement(inputWordBytes)
    {
        X = 0,
        Y = crtYPos,
        // 0 extends the content to the right margin of the page
        Width = 0,
        Zoom = 100,
        // The headings of the Word document join the outline of the PDF document
        GenerateDocumentOutline = true,
        // The fonts stored in the Word document are used; FontFiles and FallbackFontFiles can give
        // more fonts to the element, as for the converter
        UseEmbeddedFonts = true
    };

    PdfContentRenderInfo wordRenderInfo = pdfDocument.AddWord(wordElement);

    // The rendered area reported for the last page places the elements after the content
    crtYPos = wordRenderInfo.LastPageRectangle.Bounds.Bottom + 15;

    PdfRectangleElement separator = new PdfRectangleElement(0, crtYPos, pdfDocument.ContentWidth, 1)
    {
        FillColor = PdfColor.DarkBlue,
        BorderColor = null
    };
    pdfDocument.AddRectangle(separator);

    PdfTextElement closingElement = new PdfTextElement(
        string.Format("The Word content was rendered on {0} page(s), from page {1} to page {2}. This text was added after it.",
            wordRenderInfo.PagesCount, wordRenderInfo.FirstPageNumber, wordRenderInfo.LastPageNumber), textFont)
    {
        X = 0,
        Y = crtYPos + 8,
        Width = pdfDocument.ContentWidth
    };
    pdfDocument.AddText(closingElement);

    // The buffer to receive the generated PDF document
    byte[] outPdfBuffer = pdfDocument.Save();

    return outPdfBuffer;
}
