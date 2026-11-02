# Digitally Sign Existing PDF

The T:EvoPdf.Next.PdfEditor class adds a digital signature to an existing PDF document. The signature is given to the M:EvoPdf.Next.PdfEditor.#ctor(System.Byte[],System.String,EvoPdf.Next.PdfDigitalSignature) or M:EvoPdf.Next.PdfEditor.#ctor(System.String,System.String,EvoPdf.Next.PdfDigitalSignature) constructor as a T:EvoPdf.Next.PdfDigitalSignature object, the same object used by the HTML to PDF converter and the PDF creator, with the same signature levels, signing keys and appearance options described in the Digital Signatures topic. The signature is applied when the document is saved with M:EvoPdf.Next.PdfEditor.Save. The signed document keeps the PDF/A and PDF/UA conformance of the source document.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/digitally-sign-existing-pdf.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | Set the Signature Options | 5 |
| `Sample2.cs` | Add the Long Term Validation Data | 4 |
| `Sample3.cs` | Code Sample - Digitally Sign Existing PDF | 203 |
