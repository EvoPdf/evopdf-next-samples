# Add Signature Fields from HTML Elements

A document that several people sign needs an empty signature field for each signer, placed where the layout shows the signature. EVO HTML to PDF Converter and the PDF editor prepare such a document from a HTML page: the page marks the place of each signature with an element, the converter returns the page and the rectangle where the element was rendered and `PdfEditor.AddSignatureField(PdfSignatureField)` adds an empty signature field there. Each signer then fills in the field prepared for them with a digital signature that has the same field name, as described in the Digitally Sign Existing PDF topic, and the signatures already in the document stay valid.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-signature-fields-from-html-elements.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | Code Sample - Add Signature Fields from HTML Elements | 129 |
