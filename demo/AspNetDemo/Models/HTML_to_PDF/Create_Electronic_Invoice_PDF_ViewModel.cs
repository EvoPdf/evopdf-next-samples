using Microsoft.AspNetCore.Http;
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Create_Electronic_Invoice_PDF_ViewModel
    {
        public string HtmlString { get; set; }
        public string BaseUrl { get; set; } = string.Empty;

        // Name of one of the sample invoice XML files, used when no file is uploaded
        public string SampleInvoice { get; set; } = "Invoice_EN_16931.xml";

        // Invoice XML uploaded by the user; takes priority over the sample
        public IFormFile InvoiceXmlFile { get; set; }

        public PdfAttachmentRelationship Relationship { get; set; } = PdfAttachmentRelationship.Alternative;

        // Only the PDF/A-3 standards can carry an electronic invoice
        public PdfStandard PdfStandard { get; set; } = PdfStandard.PdfUa1PdfA3a;
    }
}
