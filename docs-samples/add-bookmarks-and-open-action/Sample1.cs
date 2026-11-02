PdfBookmarkCollection bookmarks = htmlToPdfConverter.PdfDocumentOptions.Bookmarks;

PdfBookmark contentBookmark = bookmarks.Add("HTML content", 1, PdfLinkPageLocation.FitPage());
contentBookmark.Open = true;
contentBookmark.Bold = true;
contentBookmark.Color = PdfColor.DarkBlue;
contentBookmark.Children.Add("Second page at 150%", 2, PdfLinkPageLocation.AtCoordinates(0, 0, 1.5f));

// A bookmark that opens a web address in the browser
bookmarks.AddUrl("Product web site", "https://www.evopdf.com");

// Display the bookmarks panel when the document is opened
htmlToPdfConverter.PdfViewerPreferences.PageMode = ViewerPageMode.UseOutlines;
