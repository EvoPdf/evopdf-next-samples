// Use EVO PDF Namespace
using EvoPdf.Next;

public byte[] AddRtfToPdfDocument(byte[] inputRtfBytes, string fontFilePath)
{
    // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
    // Licensing.LicenseKey = "your-license-key";

    // The RTF content from a file read into memory; a RTF string can also be given to the element
    PdfRtfElement rtfElement = new PdfRtfElement(inputRtfBytes);

    PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
    {
        PageSize = PdfPageSize.A4,
        PageOrientation = PdfPageOrientation.Portrait,
        Margins = new PdfMargins(36, 36, 36, 36),
        Language = "en-US"
    };

    using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);
    pdfDocument.PdfDocumentInfo.Title = "RTF Content in a PDF Document";

    PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
    PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 16f, PdfFontStyle.Bold, PdfColor.DarkBlue);
    PdfFont textFont = PdfFontManager.CreateFont(baseFont, 10f, PdfFontStyle.Normal, PdfColor.Black);

    // Elements created with the PDF creator before the document content
    PdfTextElement titleElement = new PdfTextElement("RTF Content in a PDF Document", titleFont)
    {
        X = 0,
        Y = 0,
        Width = pdfDocument.ContentWidth,
        Alignment = PdfTextAlignment.Center
    };
    titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
    float crtYPos = pdfDocument.AddText(titleElement).LastPageRectangle.Bounds.Bottom + 10;

    PdfTextElement introElement = new PdfTextElement(
        "The text above and this paragraph are text elements of the PDF document. The RTF content " +
        "starts below them and continues on new pages when it does not fit in this one. " +
        "The elements added after the RTF content go to its last page, below it.", textFont)
    {
        X = 0,
        Y = crtYPos,
        Width = pdfDocument.ContentWidth
    };
    crtYPos = pdfDocument.AddText(introElement).LastPageRectangle.Bounds.Bottom + 15;

    // The RTF content is rendered in the PDF document from the given position of the current
    // page and inside the page margins. The X position is kept on the next pages
    rtfElement.X = 0;
    rtfElement.Y = crtYPos;
    // 0 extends the content to the right margin of the page
    rtfElement.Width = 0;
    rtfElement.Zoom = 100;
    // The headings of the RTF document join the outline of the PDF document
    rtfElement.GenerateDocumentOutline = true;

    PdfContentRenderInfo rtfRenderInfo = pdfDocument.AddRtf(rtfElement);

    // The rendered area reported for the last page places the elements after the content
    crtYPos = rtfRenderInfo.LastPageRectangle.Bounds.Bottom + 15;

    PdfRectangleElement separator = new PdfRectangleElement(0, crtYPos, pdfDocument.ContentWidth, 1)
    {
        FillColor = PdfColor.DarkBlue,
        BorderColor = null
    };
    pdfDocument.AddRectangle(separator);

    PdfTextElement closingElement = new PdfTextElement(
        string.Format("The RTF content was rendered on {0} page(s), from page {1} to page {2}. This text was added after it.",
            rtfRenderInfo.PagesCount, rtfRenderInfo.FirstPageNumber, rtfRenderInfo.LastPageNumber), textFont)
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
