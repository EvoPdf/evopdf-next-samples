using EvoPdf.Next;

PdfHtmlHeaderFooter header = converter.PdfDocumentOptions.PdfHtmlHeader;
header.HtmlSourceUrl = headerHtmlUrl;
header.Height = 60;
header.FitHeight = true;

PdfHtmlHeaderFooter footer = converter.PdfDocumentOptions.PdfHtmlFooter;
footer.Html = "<div style=\"font-family: 'Times New Roman'; font-size: 10pt; text-align: right\">" +
    "Page {page_number} of {total_pages}</div>";
footer.HtmlBaseUrl = baseUrl;
footer.Height = 40;
