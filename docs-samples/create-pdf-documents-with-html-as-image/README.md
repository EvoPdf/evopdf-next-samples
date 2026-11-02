# Create PDF Documents with HTML as Image

The T:EvoPdf.Next.PdfHtmlImageElement class renders an HTML document as an image in the pages of a document being created with T:EvoPdf.Next.PdfDocument. The HTML is given as the URL of a page or as a string with a base URL and it is converted by the HTML to Image converter of the element, which captures the whole page at the height of its content. The image starts at a position in the current page, after the elements added before it, and continues on the next pages when it is taller than the space left on the page. The elements added after it go below the image on its last page.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/create-pdf-documents-with-html-as-image.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | Render HTML as an Image in the Document | 14 |
| `Sample2.cs` | Set the Converter of the Element | 2 |
| `Sample3.cs` | Continue the Image on the Next Pages | 9 |
| `Sample4.cs` | Code Sample - Create PDF Documents with HTML as Image | 169 |
