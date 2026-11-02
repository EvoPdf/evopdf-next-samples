using Microsoft.AspNetCore.Http;
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.Word_to_PDF
{
    public class Create_Electronic_Invoice_from_Word_ViewModel
    {
        public string WordFileUrl { get; set; }
        public IFormFile WordFile { get; set; }

        // Name of one of the sample invoice XML files, used when no file is uploaded
        public string SampleInvoice { get; set; } = "Invoice_EN_16931.xml";

        // Invoice XML uploaded by the user; takes priority over the sample
        public IFormFile InvoiceXmlFile { get; set; }

        public PdfAttachmentRelationship Relationship { get; set; } = PdfAttachmentRelationship.Alternative;

        // Only the PDF/A-3 standards can carry an electronic invoice
        public PdfStandard PdfStandard { get; set; } = PdfStandard.PdfUa1PdfA3a;
    }
}
