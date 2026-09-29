# Add Text Annotations to Existing PDF

The `PdfEditor` class lets you add new content to an existing PDF without rewriting the rest of the document. The editor is instantiated with the source PDF bytes or file path and an optional password. The standard claimed by the source document and its existing page geometry are inherited by the editor. Content is added at absolute page coordinates using the dedicated Add methods. Every Add method takes the target page number as the first argument. If the rendered content overflows the available space a new page is created automatically by the engine. The resulting modified PDF is produced with `PdfEditor.Save`.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-text-annotations-to-existing-pdf.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | Author Identification and Initial Popup State | 7 |
| `Sample2.cs` | Code Sample - Add Sticky Note Text Annotations to Existing PDF | 402 |
