PdfFileAttachmentAnnotation annotation = PdfFileAttachmentAnnotation.FromBytes(data, "inventory.csv", 1, 30, 100);
annotation.IconColor = PdfColor.Blue;
annotation.AddToAttachmentsPanel = true;
pdfDocument.AddFileAttachmentAnnotation(annotation);
