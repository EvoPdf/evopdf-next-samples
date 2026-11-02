// Use EVO PDF Namespace
using EvoPdf.Next;

public byte[] CreateElectronicInvoiceFromWord(byte[] inputWordBytes, byte[] invoiceXml)
{
    // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
    // Licensing.LicenseKey = "your-license-key";

    // Read the invoice. The profile and the document type (invoice, credit note, order...) are
    // taken from the XML itself
    ElectronicInvoice invoice = ElectronicInvoice.FromXml(invoiceXml);

    // How the embedded XML relates to the visible page. MINIMUM and BASIC WL do not carry
    // the whole invoice, so for them the relationship is always Data
    invoice.Relationship = PdfAttachmentRelationship.Alternative;

    // Create a Word to PDF converter object with default settings
    WordToPdfConverter wordToPdfConverter = new WordToPdfConverter();

    // An electronic invoice needs a PDF/A-3 carrier. PDF/UA-1 + PDF/A-3a produces an
    // invoice that is also accessible
    wordToPdfConverter.PdfDocumentOptions.PdfStandard = PdfStandard.PdfUa1PdfA3a;

    // Embed the invoice XML and write the metadata invoice software looks for. The Excel,
    // RTF and Markdown to PDF converters embed an electronic invoice the same way
    wordToPdfConverter.PdfDocumentOptions.AddElectronicInvoice(invoice);

    // Open the Attachments panel on document load, so the embedded XML is visible
    wordToPdfConverter.PdfViewerPreferences.PageMode = ViewerPageMode.UseAttachments;

    // The buffer to receive the generated PDF document
    byte[] outPdfBuffer = wordToPdfConverter.ConvertToPdf(inputWordBytes);

    return outPdfBuffer;
}
