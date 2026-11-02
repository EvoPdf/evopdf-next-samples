htmlElement.HtmlElementsInfoSelector = "h1, h2";
PdfHtmlRenderInfo htmlRenderInfo = pdfDocument.AddHtml(htmlElement);

foreach (HtmlElementInfo elementInfo in htmlRenderInfo.HtmlElementsInfo.Elements)
{
    foreach (PdfRenderedRectangle rectangle in elementInfo.RenderedRectangles)
        Console.WriteLine("{0} on page {1} at {2}", elementInfo.TagName, rectangle.PageNumber, rectangle.Bounds);
}
