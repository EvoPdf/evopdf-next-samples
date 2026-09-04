// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/create-pdf-documents-with-file-attachments.htm
// Documentation page: Create PDF Documents with File Attachments

byte[] xmlBytes = Encoding.UTF8.GetBytes(BuildSampleXml());
var xmlAttachment = PdfFileAttachment.FromBytes(xmlBytes, "data.xml");
xmlAttachment.MimeType = "application/xml";
xmlAttachment.Description = "Source XML data";
xmlAttachment.Relationship = PdfAttachmentRelationship.Source;
pdfDocument.AddFileAttachment(xmlAttachment);
