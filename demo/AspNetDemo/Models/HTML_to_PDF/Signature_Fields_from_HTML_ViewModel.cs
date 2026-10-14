namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Signature_Fields_from_HTML_ViewModel
    {
        public string HtmlString { get; set; }
        public string BaseUrl { get; set; }

        // The CSS selector of the HTML elements that mark the place of the signature fields
        public string SignatureFieldsSelector { get; set; } = ".signature-field";

        // Signs the first field with the sample certificate, so the result shows a signed field
        // next to the fields still waiting for their signers
        public bool SignFirstField { get; set; } = true;
    }
}
