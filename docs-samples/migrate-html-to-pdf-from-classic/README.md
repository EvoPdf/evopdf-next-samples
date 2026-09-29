# Migrate HTML to PDF Page Setup and Headers from EvoPdf Classic

EvoPdf Classic and EvoPdf Next share the `HtmlToPdfConverter` class name, the conversion methods and most option names, but the two rendering engines place the HTML on the PDF page in different ways and describe headers and footers differently. This topic covers the two areas where an integration moved from Classic changes its output or its code: the page setup with its scaling rules and the headers and footers. The namespace, package and license key changes are covered in the Classic to Next migration guide on the website.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/migrate-html-to-pdf-from-classic.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | The Classic Default: 1024 Pixel Layout Fitted to an A4 Page | 10 |
| `Sample2.cs` | The Classic Default: 1024 Pixel Layout Fitted to an A4 Page | 15 |
| `Sample3.cs` | A Page with a Fixed Width Larger Than the Viewer | 7 |
| `Sample4.cs` | AutoSizePdfPage and SinglePage | 7 |
| `Sample5.cs` | StretchToFit | 6 |
| `Sample6.cs` | A Header with HTML and Page Numbers | 23 |
| `Sample7.cs` | A Header with HTML and Page Numbers | 12 |
| `Sample8.html` | A Header with HTML and Page Numbers | 6 |
| `Sample9.cs` | Header Hidden on the First Page | 5 |
| `Sample10.cs` | Header Hidden on the First Page | 4 |
| `Sample11.cs` | Header Hidden on the First Page | 4 |
| `Sample12.cs` | A Different Header on the First Page | 22 |
| `Sample13.cs` | A Different Header and Height for Each Section of a Document | 22 |
