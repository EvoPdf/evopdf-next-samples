PdfImageElement tallImage = new PdfImageElement(imagePath)
{
    X = 0,
    Y = crtYPos,
    Width = pdfDocument.ContentWidth,
    // The part of the image below the bottom margin continues on the next pages
    ContinueOnNextPage = true
};

PdfImageRenderInfo info = pdfDocument.AddImage(tallImage);

// One rectangle per page where a part of the image was drawn
foreach (PdfRenderedRectangle part in info.RenderedRectangles)
    Console.WriteLine("page {0}: {1}", part.PageNumber, part.Bounds);

// The content that follows goes below the last part, on its page
crtYPos = (int)info.LastPageRectangle.Bounds.Bottom + 10;
