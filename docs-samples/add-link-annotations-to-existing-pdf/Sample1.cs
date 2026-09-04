// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-link-annotations-to-existing-pdf.htm
// Documentation page: Add Link Annotations to Existing PDF

PdfTextElement t = new PdfTextElement("Visit evopdf.com", linkFont) { X = 0, Y = crtYPos };
var info = pdfEditor.AddText(currentPage, t);
var b = info.LastPageRectangle.Bounds;

PdfLinkAnnotation link = PdfLinkAnnotation.FromUrl(
    url: "https://www.evopdf.com",
    pageNumber: currentPage,
    x: b.X, y: b.Y, width: b.Width, height: b.Height);
link.Description = "EvoPdf homepage";
link.BorderStyle = PdfLinkBorderStyle.None;
pdfEditor.AddLinkAnnotation(link);
