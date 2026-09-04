// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/create-pdf-documents-with-link-annotations.htm
// Documentation page: Create PDF Documents with Link Annotations

PdfTextElement t = new PdfTextElement("Visit evopdf.com", linkFont) { X = 0, Y = crtYPos };
var info = pdfDocument.AddText(t);
var b = info.LastPageRectangle.Bounds;

PdfLinkAnnotation link = PdfLinkAnnotation.FromUrl(
    url: "https://www.evopdf.com",
    pageNumber: 1,
    x: b.X, y: b.Y, width: b.Width, height: b.Height);
link.Description = "EvoPdf homepage";
link.BorderStyle = PdfLinkBorderStyle.None;

pdfDocument.AddLinkAnnotation(link);
