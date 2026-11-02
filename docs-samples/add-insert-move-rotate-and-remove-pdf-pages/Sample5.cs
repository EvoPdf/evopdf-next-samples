using System;

// Use EVO PDF Namespace
using EvoPdf.Next;

public byte[] EditPdfPages(byte[] inputPdfBytes, string password, string fontFilePath,
    int movePageNumber, int moveToPageNumber, int rotatePageNumber, int rotateDegrees, string removePageRanges)
{
    // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
    // Licensing.LicenseKey = "your-license-key";

    byte[] outPdfBuffer;

    using (PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, password))
    {
        PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
        PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 16f, PdfFontStyle.Bold, PdfColor.DarkBlue);
        PdfFont textFont = PdfFontManager.CreateFont(baseFont, 10f, PdfFontStyle.Normal, PdfColor.Black);
        PdfFont noteFont = PdfFontManager.CreateFont(baseFont, 12f, PdfFontStyle.Bold, PdfColor.DarkRed);

        // The page operations come first; the text drawn afterwards goes to the pages at their
        // final position and rotation
        if (!string.IsNullOrWhiteSpace(removePageRanges))
        {
            // The pages are removed first, so the ranges refer to the pages of the loaded document. The
            // ranges are 1-based, for example "2,4-5" or "3-" for the pages from the third one to the end
            pdfEditor.RemovePages(removePageRanges.Trim());
        }

        // Insert a blank page of the chosen size before the first page. The existing pages
        // are shifted and keep their content, links and bookmarks
        int coverPageNumber = pdfEditor.InsertPage(1, PdfPageSize.A4, PdfPageOrientation.Portrait);

        // Add a blank page at the end of the document, with the size of the last page
        int lastPageNumber = pdfEditor.AddPage();

        // Move a page to another position; the pages in between are shifted
        pdfEditor.MovePage(movePageNumber, moveToPageNumber);

        // The pages added above follow the move
        coverPageNumber = PageNumberAfterMove(coverPageNumber, movePageNumber, moveToPageNumber);
        lastPageNumber = PageNumberAfterMove(lastPageNumber, movePageNumber, moveToPageNumber);

        // Rotate the page clockwise by a multiple of 90 degrees. The rotation is added to the
        // current rotation of the page; SetPageRotation would set an absolute rotation instead.
        // A page is rotated before content is added to it
        pdfEditor.RotatePage(rotatePageNumber, rotateDegrees);

        PdfTextElement lastPageText = new PdfTextElement("This page was added at the end of the document with PdfEditor.AddPage()", textFont)
        {
            X = 36,
            Y = 36,
            Width = pdfEditor.GetPdfPageInfo(lastPageNumber).PageSize.Width - 72
        };
        pdfEditor.AddText(lastPageNumber, lastPageText);

        // A note at the bottom of the rotated page, placed in the page as it is displayed:
        // a rotation of 90 or 270 degrees swaps its width and height
        PdfPageInfo rotatedPageInfo = pdfEditor.GetPdfPageInfo(rotatePageNumber);
        bool turned = rotatedPageInfo.Rotation % 180 != 0;
        float displayedWidth = turned ? rotatedPageInfo.PageSize.Height : rotatedPageInfo.PageSize.Width;
        float displayedHeight = turned ? rotatedPageInfo.PageSize.Width : rotatedPageInfo.PageSize.Height;
        PdfTextElement rotatedPageText = new PdfTextElement(
            "This page was rotated by " + rotateDegrees + " degrees with PdfEditor.RotatePage()", noteFont)
        {
            X = 36,
            Y = displayedHeight - 60,
            Width = displayedWidth - 72
        };
        pdfEditor.AddText(rotatePageNumber, rotatedPageText);

        // The cover page lists the pages of the edited document with the size and
        // rotation reported by GetPdfPageInfo
        PdfPageInfo coverPageInfo = pdfEditor.GetPdfPageInfo(coverPageNumber);
        float textWidth = coverPageInfo.PageSize.Width - 72;

        PdfTextElement coverTitle = new PdfTextElement("Pages of the edited document", titleFont)
        {
            X = 36,
            Y = 36,
            Width = textWidth
        };
        float crtYPos = pdfEditor.AddText(coverPageNumber, coverTitle).LastPageRectangle.Bounds.Bottom + 12;

        int pageCount = pdfEditor.PageCount;
        for (int pageNumber = 1; pageNumber <= pageCount; pageNumber++)
        {
            PdfPageInfo pageInfo = pdfEditor.GetPdfPageInfo(pageNumber);
            string pageLine = string.Format("Page {0}: {1} x {2} pt, rotation {3} degrees",
                pageInfo.PageNumber, pageInfo.PageSize.Width, pageInfo.PageSize.Height, pageInfo.Rotation);

            PdfTextElement pageText = new PdfTextElement(pageLine, textFont)
            {
                X = 54,
                Y = crtYPos,
                Width = textWidth - 18
            };
            crtYPos = pdfEditor.AddText(coverPageNumber, pageText).LastPageRectangle.Bounds.Bottom + 4;
        }

        outPdfBuffer = pdfEditor.Save();
    }

    return outPdfBuffer;
}

// The number of a page after another page was moved
private static int PageNumberAfterMove(int pageNumber, int movedPage, int newPage)
{
    if (pageNumber == movedPage)
        return newPage;
    if (movedPage < pageNumber && pageNumber <= newPage)
        return pageNumber - 1;
    if (newPage <= pageNumber && pageNumber < movedPage)
        return pageNumber + 1;
    return pageNumber;
}
