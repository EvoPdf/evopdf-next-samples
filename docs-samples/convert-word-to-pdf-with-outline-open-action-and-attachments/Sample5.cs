// Use EVO PDF Namespace
using EvoPdf.Next;

public byte[] ConvertWordToPdfWithOutlineAndAttachments(byte[] inputWordBytes, string startPdfPath)
{
    // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
    // Licensing.LicenseKey = "your-license-key";

    // Create a Word to PDF converter object with default settings
    WordToPdfConverter wordToPdfConverter = new WordToPdfConverter();

    // The same bookmarks, open action and attachments options are available in the Excel, RTF
    // and Markdown to PDF converters

    // Create the outline of the PDF document from the headings of the Word document, with a
    // bookmark for each heading, nested by the heading level
    wordToPdfConverter.PdfDocumentOptions.GenerateDocumentOutline = true;

    // The pages of an existing PDF document are placed before the converted content. The
    // page numbers of the bookmarks and of the open action count these pages
    wordToPdfConverter.PdfDocumentOptions.AddStartPdf(startPdfPath);

    int firstContentPage = 1;
    using (PdfSplit startPdf = new PdfSplit(startPdfPath))
        firstContentPage += startPdf.PageCount;

    // Bookmarks added to the outline after the ones created from the headings: a bookmark to
    // a page location with a child bookmark and a bookmark opening a web address
    PdfBookmark contentBookmark = wordToPdfConverter.PdfDocumentOptions.Bookmarks.Add(
        "Converted Word document", firstContentPage, PdfLinkPageLocation.FitWidth(0));
    contentBookmark.Bold = true;
    contentBookmark.Color = PdfColor.DarkBlue;
    contentBookmark.Children.Add("Top of the first page, zoomed", firstContentPage, PdfLinkPageLocation.AtCoordinates(0, 0, 1.5f));

    wordToPdfConverter.PdfDocumentOptions.Bookmarks.AddUrl("Library web site", "https://www.evopdf.com");

    // The action executed when the document is opened: go to the first page of the
    // converted content, fit to the page width
    wordToPdfConverter.PdfDocumentOptions.OpenAction = new PdfGoToPageAction(firstContentPage, PdfLinkPageLocation.FitWidth(0));

    // Attach the source Word document to the generated PDF. The attachment appears in the
    // Attachments panel of the PDF viewer
    PdfFileAttachment attachment = PdfFileAttachment.FromBytes(inputWordBytes, "Source_Document.docx");
    attachment.Description = "The Word document the PDF was generated from";
    wordToPdfConverter.PdfDocumentOptions.AddFileAttachment(attachment);

    // The panel shown by the viewer when the document is opened: the outline or the attachments
    wordToPdfConverter.PdfViewerPreferences.PageMode = ViewerPageMode.UseOutlines;

    // The buffer to receive the generated PDF document
    byte[] outPdfBuffer = wordToPdfConverter.ConvertToPdf(inputWordBytes);

    return outPdfBuffer;
}
