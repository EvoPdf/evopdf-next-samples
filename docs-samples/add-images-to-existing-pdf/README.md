# Add Images to Existing PDF

The T:EvoPdf.Next.PdfEditor class lets you add new content to an existing PDF without rewriting the rest of the document. The editor is instantiated with the source PDF bytes or file path and an optional password. The standard claimed by the source document and its existing page geometry are inherited by the editor. Content is added at absolute page coordinates using the dedicated Add methods. The methods that draw page content, such as AddText, AddImage or AddHtml, take the target page number as the first argument. Content that does not fit in the page continues on the following pages of the document when the element is set to continue, or on pages inserted after the current one when InsertContinuationPages is true. The resulting modified PDF is produced with M:EvoPdf.Next.PdfEditor.Save.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-images-to-existing-pdf.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | Code Sample - Add Images to Existing PDF | 402 |
