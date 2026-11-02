// Bookmarks added to the outline after the ones created from the headings: a bookmark to
// a page location with a child bookmark and a bookmark opening a web address
PdfBookmark contentBookmark = wordToPdfConverter.PdfDocumentOptions.Bookmarks.Add(
    "Converted Word document", firstContentPage, PdfLinkPageLocation.FitWidth(0));
contentBookmark.Bold = true;
contentBookmark.Color = PdfColor.DarkBlue;
contentBookmark.Children.Add("Top of the first page, zoomed", firstContentPage, PdfLinkPageLocation.AtCoordinates(0, 0, 1.5f));

wordToPdfConverter.PdfDocumentOptions.Bookmarks.AddUrl("Library web site", "https://www.evopdf.com");
