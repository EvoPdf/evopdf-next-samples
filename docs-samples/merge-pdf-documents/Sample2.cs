pdfMerge.KeepFieldsSeparate = true;
pdfMerge.KeepSourceBookmarks = true;
pdfMerge.Bookmarks.Add("First document", firstDocumentPage, PdfLinkPageLocation.FitPage());
pdfMerge.Bookmarks.Add("Second document", secondDocumentPage, PdfLinkPageLocation.FitPage());
