// Attach the source Word document to the generated PDF. The attachment appears in the
// Attachments panel of the PDF viewer
PdfFileAttachment attachment = PdfFileAttachment.FromBytes(inputWordBytes, "Source_Document.docx");
attachment.Description = "The Word document the PDF was generated from";
wordToPdfConverter.PdfDocumentOptions.AddFileAttachment(attachment);

// The panel shown by the viewer when the document is opened
wordToPdfConverter.PdfViewerPreferences.PageMode = ViewerPageMode.UseAttachments;
