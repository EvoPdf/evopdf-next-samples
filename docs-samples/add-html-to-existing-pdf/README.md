# Add HTML to Existing PDF

The T:EvoPdf.Next.PdfEditor class renders HTML content in the pages of a loaded document with M:EvoPdf.Next.PdfEditor.AddHtml(System.Int32,EvoPdf.Next.PdfHtmlElement), or asynchronously with M:EvoPdf.Next.PdfEditor.AddHtmlAsync(System.Int32,EvoPdf.Next.PdfHtmlElement,System.Threading.CancellationToken), which take the number of the page where the content starts and a T:EvoPdf.Next.PdfHtmlElement, the element described in the topic. The content starts at the position given by the element, relative to the top left corner of the page, and continues on the next pages when it does not fit, between the continuation margins of the element. The links and the form fields of the content are kept and the outline generated from the HTML headings joins the bookmarks of the document. In a tagged document, like a PDF/UA document, the content is rendered with the standard of the document and its structure joins the structure tree of the document.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-html-to-existing-pdf.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | Render HTML Content in an Edited Document | 17 |
| `Sample2.cs` | Set the Converter and Retrieve the Positions of the HTML Elements | 10 |
| `Sample3.cs` | Code Sample - Add HTML to Existing PDF | 259 |
