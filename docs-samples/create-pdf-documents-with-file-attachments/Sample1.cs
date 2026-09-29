byte[] xmlBytes = Encoding.UTF8.GetBytes(BuildSampleXml());
var xmlAttachment = PdfFileAttachment.FromBytes(xmlBytes, "data.xml");
xmlAttachment.MimeType = "application/xml";
xmlAttachment.Description = "Source XML data";
xmlAttachment.Relationship = PdfAttachmentRelationship.Source;
pdfDocument.AddFileAttachment(xmlAttachment);
