// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/create-pdf-documents-with-file-attachments.htm
// Documentation page: Create PDF Documents with File Attachments

string alphabetFilePath = Path.Combine(GetDemoTextsPath(), "Alphabet.txt");
var textAttachment = PdfFileAttachment.FromFile(alphabetFilePath);
textAttachment.MimeType = "text/plain";
textAttachment.Description = "Sample alphabet text";
pdfDocument.AddFileAttachment(textAttachment);
