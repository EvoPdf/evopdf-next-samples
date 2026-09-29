string alphabetFilePath = Path.Combine(GetDemoTextsPath(), "Alphabet.txt");
var textAttachment = PdfFileAttachment.FromFile(alphabetFilePath);
textAttachment.MimeType = "text/plain";
textAttachment.Description = "Sample alphabet text";

htmlToPdfConverter.PdfDocumentOptions.AddFileAttachment(textAttachment);
