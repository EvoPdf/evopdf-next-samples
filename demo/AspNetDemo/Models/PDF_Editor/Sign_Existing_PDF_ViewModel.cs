using EvoPdf.Next;
using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Editor
{
    public class Sign_Existing_PDF_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public string SignatureReason { get; set; } = "Approved";
        public string SignatureLocation { get; set; } = "My Signature Location";
        public string SignatureContact { get; set; } = "My Contact Information";

        // PAdES baseline profile; the long-term profiles need the timestamp server
        public PdfDigitalSignatureProfile Profile { get; set; } = PdfDigitalSignatureProfile.Auto;

        // Approval signature or certification signature with the changes allowed after signing
        public PdfCertificationLevel CertificationLevel { get; set; } = PdfCertificationLevel.NotCertified;

        public string TimestampServerUrl { get; set; } = string.Empty;

        // The name of an empty signature field of the document that receives the signature; empty for a new field
        public string SignatureFieldName { get; set; } = "ManagerSignature";

        public bool EnableAppearance { get; set; } = true;
        public string SignatureText { get; set; } = "Approved by EVO PDF Software";
    }
}
