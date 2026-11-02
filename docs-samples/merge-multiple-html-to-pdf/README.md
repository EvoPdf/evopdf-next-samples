# Convert Multiple HTML to a Single PDF

EvoPdf Next renders several HTML documents one after the other in the same PDF document with the M:EvoPdf.Next.PdfDocument.AddHtml(EvoPdf.Next.PdfHtmlElement) method of the T:EvoPdf.Next.PdfDocument class. Each T:EvoPdf.Next.PdfHtmlElement starts at the position given by its P:EvoPdf.Next.PdfHtmlElement.Y property in the current page and continues on as many new pages as it needs, so the second document can continue on the page where the first one ends, at the bottom of the P:EvoPdf.Next.PdfContentRenderInfo.LastPageRectangle reported for the first one, or start on a new page added with M:EvoPdf.Next.PdfDocument.AddPage(EvoPdf.Next.PdfPageSize,EvoPdf.Next.PdfPageOrientation). A new page is the usual choice: a document whose background covers its pages to the bottom would otherwise share its last page with the next one. The page size, the orientation and the margins of the document apply to all the documents; the HTML to PDF converter of each element, exposed by P:EvoPdf.Next.PdfHtmlElement.Converter, keeps its other options.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/merge-multiple-html-to-pdf.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | Code Sample - Convert Multiple HTML to a Single PDF | 264 |
