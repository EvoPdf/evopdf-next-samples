# Add Header and Footer to PDF from Multiple HTML

A PDF document that renders several HTML documents one after the other, as described in the Convert Multiple HTML to a Single PDF topic, gets its header and footer from the HTML templates of the document, added with the M:EvoPdf.Next.PdfDocument.AddHtmlTemplate(System.Int32,System.Int32,System.Int32,System.Int32,System.String,System.String) method of the T:EvoPdf.Next.PdfDocument class, from an HTML string or from a URL. The templates are drawn on every page of the document when it is saved, so the {page_number} and {total_pages} variables give the page numbers of the whole document.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/header-and-footer-on-pdf-from-multiple-html.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | Code Sample - Add Header and Footer to PDF from Multiple HTML | 189 |
| `Sample2.html` | Header HTML Template | 25 |
| `Sample3.html` | Footer HTML Template | 25 |
