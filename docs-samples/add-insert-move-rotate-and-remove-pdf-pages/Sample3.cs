// Rotate the page clockwise by a multiple of 90 degrees. The rotation is added to the
// current rotation of the page; SetPageRotation would set an absolute rotation instead.
// A page is rotated before content is added to it
pdfEditor.RotatePage(rotatePageNumber, 90);

// A note at the bottom of the rotated page, placed in the page as it is displayed:
// a rotation of 90 or 270 degrees swaps its width and height
PdfPageInfo rotatedPageInfo = pdfEditor.GetPdfPageInfo(rotatePageNumber);
bool turned = rotatedPageInfo.Rotation % 180 != 0;
float displayedWidth = turned ? rotatedPageInfo.PageSize.Height : rotatedPageInfo.PageSize.Width;
float displayedHeight = turned ? rotatedPageInfo.PageSize.Width : rotatedPageInfo.PageSize.Height;
PdfTextElement rotatedPageText = new PdfTextElement("This page was rotated by 90 degrees", noteFont)
{
    X = 36,
    Y = displayedHeight - 60,
    Width = displayedWidth - 72
};
pdfEditor.AddText(rotatePageNumber, rotatedPageText);
