# Create PDF/UA and PDF/A Documents

A PDF created from scratch with `PdfDocument` can target an accessibility (PDF/UA) or archival (PDF/A) standard simply by setting the `PdfDocumentCreateSettings.PdfStandard` property on the `PdfDocumentCreateSettings` passed to the constructor. The library then handles the standard-mandated artefacts. These include embedded fonts, color profiles, the structure tree, namespaces, document metadata and viewer preferences.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/create-pdf-documents-with-standards.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | Configure Standard, Language and Page Setup | 11 |
| `Sample2.cs` | Code Sample - Create PDF/UA and PDF/A Compliant Documents from Scratch | 281 |
