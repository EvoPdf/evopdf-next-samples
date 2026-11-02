PdfLongTermValidationOptions ltvOptions = new PdfLongTermValidationOptions();
ltvOptions.TimestampServerUrl = "http://timestamp.example.com";
pdfEditor.AddLongTermValidation(ltvOptions);
byte[] outPdfBuffer = pdfEditor.Save();
