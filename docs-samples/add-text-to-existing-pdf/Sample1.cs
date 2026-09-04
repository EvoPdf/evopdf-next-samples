// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-text-to-existing-pdf.htm
// Documentation page: Add Text to Existing PDF

string password = string.IsNullOrEmpty(ownerPassword) ? userPassword : ownerPassword;
using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, password);
