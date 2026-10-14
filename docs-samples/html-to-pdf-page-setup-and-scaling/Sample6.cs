// A 768 point wide page (1024 pixels), the content drawn 1:1, paginated at the A4 height
converter.PageWidthFromBrowserWindow();

// The whole page on one PDF page
converter.PageWidthFromBrowserWindow(1024, singlePage: true);
