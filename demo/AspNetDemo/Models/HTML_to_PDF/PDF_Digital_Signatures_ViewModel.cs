using EvoPdf.Next;
namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class PDF_Digital_Signatures_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Url";
        public string Url { get; set; } = "http://www.evopdf.com";
        public string HtmlString { get; set; } = "Enter the <b>HTML String to Convert</b> and optionally set a <b>Base URL</b> if the HTML string references external resources by relative URLs";
        public string BaseUrl { get; set; } = string.Empty;

        public string SignatureReason { get; set; } = "My Signature Reason";
        public string SignatureLocation { get; set; } = "My Signature Location";
        public string SignatureContact { get; set; } = "My Contact Information";

        // PAdES baseline profile; the long-term profiles need the timestamp server
        public PdfDigitalSignatureProfile Profile { get; set; } = PdfDigitalSignatureProfile.Auto;

        // Approval signature or certification signature with the changes allowed after signing
        public PdfCertificationLevel CertificationLevel { get; set; } = PdfCertificationLevel.NotCertified;

        public string TimestampServerUrl { get; set; } = string.Empty;

        // Where the signing key comes from: the PFX file, an X509Certificate2 object or an
        // IExternalDigitalSignature implementation
        public string KeySource { get; set; } = "Pfx";

        public bool EnableAppearance { get; set; } = true;
        public bool DisplayOnLastPage { get; set; } = false;
        public bool AddSignatureText { get; set; } = true;
        public string SignatureText { get; set; } = "Signed by EVO PDF Software";
        public bool AddSignatureImage { get; set; } = true;
    }
}