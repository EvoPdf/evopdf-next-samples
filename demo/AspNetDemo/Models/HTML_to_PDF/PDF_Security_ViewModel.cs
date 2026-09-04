namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class PDF_Security_ViewModel
    {
        public string Url { get; set; } = "http://www.evopdf.com";

        public string EncryptionType { get; set; } = "RC4";
        public string EncryptionKey { get; set; } = "Bit128";

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public bool PrintEnabled { get; set; } = true;
        public bool FillFormFieldsEnabled { get; set; } = true;
        public bool EditContentEnabled { get; set; } = true;
        public bool EditAnnotationsEnabled { get; set; } = true;
        public bool CopyContentEnabled { get; set; } = true;
        public bool CopyAccessibilityContentEnabled { get; set; } = true;
    }
}