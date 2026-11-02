htmlImage.ContinueOnNextPage = true;
PdfImageRenderInfo htmlImageInfo = pdfDocument.AddHtmlImage(htmlImage);

foreach (PdfRenderedRectangle part in htmlImageInfo.RenderedRectangles)
    Console.WriteLine("Page {0}: {1}", part.PageNumber, part.Bounds);

// The elements added after the image go below the part drawn on its last page
PdfRectangleF lastPart = htmlImageInfo.LastPageRectangle.VisibleBounds ?? htmlImageInfo.LastPageRectangle.Bounds;
crtYPos = lastPart.Bottom + 10;
