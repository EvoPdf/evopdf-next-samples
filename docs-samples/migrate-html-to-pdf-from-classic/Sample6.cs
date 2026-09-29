using EvoPdf;
using System.Drawing;

converter.PdfDocumentOptions.ShowHeader = true;
converter.PdfHeaderOptions.HeaderHeight = 60;
converter.PdfHeaderOptions.HeaderBackColor = Color.White;

HtmlToPdfElement headerHtml = new HtmlToPdfElement(headerHtmlUrl);
headerHtml.FitHeight = true;
converter.PdfHeaderOptions.AddElement(headerHtml);

float headerWidth = converter.PdfDocumentOptions.PdfPageSize.Width -
    converter.PdfDocumentOptions.LeftMargin - converter.PdfDocumentOptions.RightMargin;
LineElement headerLine = new LineElement(0, 59, headerWidth, 59);
headerLine.ForeColor = Color.Gray;
converter.PdfHeaderOptions.AddElement(headerLine);

converter.PdfDocumentOptions.ShowFooter = true;
converter.PdfFooterOptions.FooterHeight = 40;
TextElement footerText = new TextElement(0, 15, "Page &p; of &P;",
    new Font(new FontFamily("Times New Roman"), 10, GraphicsUnit.Point));
footerText.TextAlign = HorizontalTextAlign.Right;
converter.PdfFooterOptions.AddElement(footerText);
