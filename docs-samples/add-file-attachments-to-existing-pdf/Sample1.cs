// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-file-attachments-to-existing-pdf.htm
// Documentation page: Add File Attachments to Existing PDF

byte[] xmlBytes = Encoding.UTF8.GetBytes(BuildSampleXml());
var xmlAttachment = PdfFileAttachment.FromBytes(xmlBytes, "data.xml");
xmlAttachment.MimeType = "application/xml";
xmlAttachment.Description = "Source XML data";
xmlAttachment.Relationship = PdfAttachmentRelationship.Source;
pdfEditor.AddFileAttachment(xmlAttachment);
