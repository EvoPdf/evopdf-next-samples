// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-attachments-to-generated-pdf.htm
// Documentation page: Add Attachments to Generated PDF

byte[] xmlBytes = Encoding.UTF8.GetBytes(BuildSampleXml());
var xmlAttachment = PdfFileAttachment.FromBytes(xmlBytes, "data.xml");
xmlAttachment.MimeType = "application/xml";
xmlAttachment.Description = "Source XML data";
xmlAttachment.Relationship = PdfAttachmentRelationship.Source;

htmlToPdfConverter.PdfDocumentOptions.AddFileAttachment(xmlAttachment);
