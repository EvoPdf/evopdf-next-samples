# Migrate HTML to PDF Page Setup and Headers from EvoPdf Classic

EvoPdf Classic and EvoPdf Next share the HtmlToPdfConverter class name, the conversion methods and most option names, but the two rendering engines place the HTML on the PDF page in different ways and describe headers and footers differently. This topic covers the two areas where an integration moved from Classic changes its output or its code: the page setup with its scaling rules and the headers and footers. The namespace, package and license key changes are covered in the Classic to Next migration guide https://www.evopdf.com/evopdf-classic-to-next-migration on the website.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/migrate-html-to-pdf-from-classic.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.sh` | Keeping the Classic Code: the Platform Packages | 2 |
| `Sample2.cs` | Keeping the Classic Code: the Platform Packages | 8 |
| `Sample3.cs` | The Classic Default: 1024 Pixel Layout Fitted to an A4 Page | 10 |
| `Sample4.cs` | The Classic Default: 1024 Pixel Layout Fitted to an A4 Page | 15 |
| `Sample5.cs` | A Page with a Fixed Width Larger Than the Viewer | 7 |
| `Sample6.cs` | AutoSizePdfPage and SinglePage | 7 |
| `Sample7.cs` | StretchToFit | 6 |
| `Sample8.cs` | A Header with HTML and Page Numbers | 23 |
| `Sample9.cs` | A Header with HTML and Page Numbers | 12 |
| `Sample10.html` | A Header with HTML and Page Numbers | 6 |
| `Sample11.cs` | Header Hidden on the First Page | 5 |
| `Sample12.cs` | Header Hidden on the First Page | 4 |
| `Sample13.cs` | Header Hidden on the First Page | 4 |
| `Sample14.cs` | A Different Header on the First Page | 22 |
| `Sample15.cs` | A Different Header and Height for Each Section of a Document | 23 |
