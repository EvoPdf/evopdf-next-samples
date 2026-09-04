// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-attachments-to-generated-pdf.htm
// Documentation page: Add Attachments to Generated PDF

string alphabetFilePath = Path.Combine(GetDemoTextsPath(), "Alphabet.txt");
var textAttachment = PdfFileAttachment.FromFile(alphabetFilePath);
textAttachment.MimeType = "text/plain";
textAttachment.Description = "Sample alphabet text";

htmlToPdfConverter.PdfDocumentOptions.AddFileAttachment(textAttachment);
