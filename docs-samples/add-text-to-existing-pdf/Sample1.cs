string password = string.IsNullOrEmpty(ownerPassword) ? userPassword : ownerPassword;
using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, password);
