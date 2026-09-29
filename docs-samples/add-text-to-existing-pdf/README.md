# Add Text to Existing PDF

The `PdfEditor` class lets you add new content to an existing PDF without rewriting the rest of the document. The editor is instantiated with the source PDF bytes or file path and an optional password. The standard claimed by the source document and its existing page geometry are inherited by the editor. Content is added at absolute page coordinates using the dedicated Add methods. Every Add method takes the target page number as the first argument. If the rendered content overflows the available space a new page is created automatically by the engine. The resulting modified PDF is produced with `PdfEditor.Save`.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-text-to-existing-pdf.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | Open the Source PDF | 2 |
| `Sample2.cs` | AddText, Page Cursor and Multi-Page Flow | 4 |
| `Sample3.cs` | Code Sample - Add Text to Existing PDF | 409 |
