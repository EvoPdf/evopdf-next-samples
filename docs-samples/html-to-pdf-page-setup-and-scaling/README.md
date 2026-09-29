# HTML to PDF Page Setup and Scaling

The layout methods of the `HtmlToPdfConverter` class decide three things: the size of the PDF page, the width at which the HTML is laid out and the scale at which the layout is drawn on the page. Each method covers one kind of document; you call it, convert, and the converter computes the layout width and the scale from the page size, the orientation and the margins in use when the PDF is generated.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/html-to-pdf-page-setup-and-scaling.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | FitBrowserWindowToPage | 5 |
| `Sample2.cs` | LayoutAtPageWidth | 11 |
| `Sample3.cs` | PrintLikeChrome | 7 |
| `Sample4.cs` | SinglePageOfWidth | 2 |
| `Sample5.cs` | PageWidthFromBrowserWindow | 5 |
| `Sample6.cs` | One PDF Page for the Whole Content | 11 |
| `Sample7.cs` | A Web Page on A4 or Letter with Its Desktop Layout | 14 |
| `Sample8.cs` | An HTML Template Designed for the Paper Size | 22 |
| `Sample9.cs` | An HTML Page Converted at Its Exact Pixel Size | 13 |
| `Sample10.cs` | A Wide Table on a Landscape Page | 9 |
| `Sample11.cs` | The Same Output as Save as PDF in Chrome | 18 |
| `Sample12.cs` | A Page in Its Mobile Layout | 13 |
| `Sample13.cs` | A Receipt on One Page of Fixed Width | 18 |
| `Sample14.cs` | Options Set After a Layout Method | 9 |
