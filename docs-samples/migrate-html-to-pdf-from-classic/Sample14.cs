HtmlToPdfConverter converter = new HtmlToPdfConverter();

// A fixed page size, so that the page width is known for the template below
converter.PdfDocumentOptions.AutoResizePdfPageWidth = false;
converter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;

// The document header, hidden on the first page, with its space kept on every page
PdfHtmlHeaderFooter header = converter.PdfDocumentOptions.PdfHtmlHeader;
header.HtmlSourceUrl = headerHtmlUrl;
header.Height = 60;
header.ShowInFirstPage = false;
header.ReserveSpaceAlways = true;

// The alternative header: an HTML template of the same height, drawn at the top of the first page only
int pageWidth = converter.PdfDocumentOptions.PdfPageSize.Width;
PdfHtmlTemplate firstPageHeader = converter.PdfDocumentOptions.AddHtmlTemplate(0, 0, pageWidth, 60,
    firstPageHeaderHtml, baseUrl);
firstPageHeader.ShowInFirstPage = true;
firstPageHeader.ShowInOddPages = false;
firstPageHeader.ShowInEvenPages = false;

byte[] pdf = converter.ConvertUrl(url);
