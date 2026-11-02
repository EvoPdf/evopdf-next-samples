// Use EVO PDF Namespace
using EvoPdf.Next;

public byte[] ConvertRtfToPdfWithDocumentHeaders(byte[] inputRtfBytes, bool useHeaderFooterFromRtf,
    string headerHtml, string footerHtml, string htmlBaseUrl)
{
    // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
    // Licensing.LicenseKey = "your-license-key";

    // Create a RTF to PDF converter object with default settings
    RtfToPdfConverter rtfToPdfConverter = new RtfToPdfConverter();

    // Render the header and the footer of the RTF document on the PDF pages, with the page
    // number and page count fields they contain and with a different first page when the
    // document has one. A HTML header or footer set in the converter takes the place of the
    // document one
    rtfToPdfConverter.PdfDocumentOptions.UseHeaderFooterFromRtf = useHeaderFooterFromRtf;

    if (!string.IsNullOrEmpty(headerHtml))
    {
        // A HTML header set in the converter takes the place of the document header
        rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Html = headerHtml;
        rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlBaseUrl = htmlBaseUrl;
        rtfToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoSizeContentHeight = true;
    }

    if (!string.IsNullOrEmpty(footerHtml))
    {
        // A HTML footer set in the converter takes the place of the document footer
        rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Html = footerHtml;
        rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlBaseUrl = htmlBaseUrl;
        rtfToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoSizeContentHeight = true;
    }

    // The buffer to receive the generated PDF document
    byte[] outPdfBuffer = rtfToPdfConverter.ConvertToPdf(inputRtfBytes);

    return outPdfBuffer;
}
