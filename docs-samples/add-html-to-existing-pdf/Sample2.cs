htmlElement.Converter.MediaType = "print";
htmlElement.Converter.PdfDocumentOptions.GenerateDocumentOutline = true;
htmlElement.HtmlElementsInfoSelector = "h1, h2";

PdfHtmlRenderInfo htmlRenderInfo = pdfEditor.AddHtml(1, htmlElement);
foreach (HtmlElementInfo elementInfo in htmlRenderInfo.HtmlElementsInfo.Elements)
{
    foreach (PdfRenderedRectangle rectangle in elementInfo.RenderedRectangles)
        Console.WriteLine("{0} on page {1} at {2}", elementInfo.TagName, rectangle.PageNumber, rectangle.Bounds);
}
