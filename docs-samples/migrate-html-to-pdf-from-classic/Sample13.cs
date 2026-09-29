using PdfMerge pdfMerge = new PdfMerge();

// Section 1: a tall header with the report title
HtmlToPdfConverter section1 = new HtmlToPdfConverter();
section1.PdfDocumentOptions.PdfHtmlHeader.Html = titleHeaderHtml;
section1.PdfDocumentOptions.PdfHtmlHeader.HtmlBaseUrl = baseUrl;
section1.PdfDocumentOptions.PdfHtmlHeader.Height = 120;
section1.PdfDocumentOptions.PdfHtmlFooter.Html = "<div>Page {page_number}</div>";
section1.PdfDocumentOptions.PdfHtmlFooter.HtmlBaseUrl = baseUrl;
int section1Pages = pdfMerge.AddPdf(section1.ConvertHtml(section1Html, baseUrl));

// Section 2: a one line header, page numbers continue after section 1
HtmlToPdfConverter section2 = new HtmlToPdfConverter();
section2.PdfDocumentOptions.PdfHtmlHeader.Html = lineHeaderHtml;
section2.PdfDocumentOptions.PdfHtmlHeader.HtmlBaseUrl = baseUrl;
section2.PdfDocumentOptions.PdfHtmlHeader.Height = 30;
section2.PdfDocumentOptions.PdfHtmlFooter.Html = "<div>Page {page_number}</div>";
section2.PdfDocumentOptions.PdfHtmlFooter.HtmlBaseUrl = baseUrl;
section2.PdfDocumentOptions.PdfHtmlFooter.PageNumberOffset = section1Pages;
pdfMerge.AddPdf(section2.ConvertHtml(section2Html, baseUrl));

byte[] pdf = pdfMerge.Save();
