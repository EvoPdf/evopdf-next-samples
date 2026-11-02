// Use EVO PDF Namespace
using EvoPdf.Next;

public byte[] ConvertWordToPdfWithDocumentHeaders(byte[] inputWordBytes, bool useHeaderFooterFromWord,
    string headerHtml, string footerHtml, string htmlBaseUrl)
{
    // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
    // Licensing.LicenseKey = "your-license-key";

    // Create a Word to PDF converter object with default settings
    WordToPdfConverter wordToPdfConverter = new WordToPdfConverter();

    // Render the header and the footer of the Word document on the PDF pages, with the page
    // number and page count fields they contain and with a different first page when the
    // document has one. A HTML header or footer set in the converter takes the place of the
    // document one
    wordToPdfConverter.PdfDocumentOptions.UseHeaderFooterFromWord = useHeaderFooterFromWord;

    if (!string.IsNullOrEmpty(headerHtml))
    {
        // A HTML header set in the converter takes the place of the document header
        wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Html = headerHtml;
        wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlBaseUrl = htmlBaseUrl;
        wordToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoSizeContentHeight = true;
    }

    if (!string.IsNullOrEmpty(footerHtml))
    {
        // A HTML footer set in the converter takes the place of the document footer
        wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Html = footerHtml;
        wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlBaseUrl = htmlBaseUrl;
        wordToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoSizeContentHeight = true;
    }

    // The buffer to receive the generated PDF document
    byte[] outPdfBuffer = wordToPdfConverter.ConvertToPdf(inputWordBytes);

    return outPdfBuffer;
}
