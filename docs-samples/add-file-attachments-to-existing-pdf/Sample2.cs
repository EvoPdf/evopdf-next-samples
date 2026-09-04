// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/add-file-attachments-to-existing-pdf.htm
// Documentation page: Add File Attachments to Existing PDF

byte[] csvBytes = Encoding.UTF8.GetBytes(BuildSampleCsv());
var ann = PdfFileAttachmentAnnotation.FromBytes(
    csvBytes, "inventory.csv", pageNumber: 1, x: 30, y: 200);
ann.MimeType = "text/csv";
ann.Description = "Inline CSV inventory data";
ann.TooltipText = "Click to open inventory.csv";
ann.Icon = PdfAttachmentIcon.Paperclip;
// AddToAttachmentsPanel stays at the default false. Adobe Reader picks
// up the annotation and lists the file in the panel anyway. Setting true
// would produce a duplicate entry.
pdfEditor.AddFileAttachmentAnnotation(ann);
