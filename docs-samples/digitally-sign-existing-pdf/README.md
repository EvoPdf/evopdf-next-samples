# Digitally Sign Existing PDF

The `PdfEditor` class adds a digital signature to an existing PDF document. The signature is given to the `PdfEditor.PdfEditor(Byte[], String, PdfDigitalSignature)` or `PdfEditor.PdfEditor(String, String, PdfDigitalSignature)` constructor as a `PdfDigitalSignature` object, the same object used by the HTML to PDF converter and the PDF creator, with the same signature levels, signing keys and appearance options described in the Digital Signatures topic. The signature is applied when the document is saved with `PdfEditor.Save()`. The signed document keeps the PDF/A and PDF/UA conformance of the source document.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/digitally-sign-existing-pdf.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | Code Sample - Digitally Sign Existing PDF | 163 |
