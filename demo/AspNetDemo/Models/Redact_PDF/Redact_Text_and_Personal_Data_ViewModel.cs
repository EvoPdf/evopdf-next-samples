using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.Redact_PDF
{
    public class Redact_Text_and_Personal_Data_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public string TextsToRedact { get; set; } = "Jane Smith";
        public bool MatchCase { get; set; } = false;
        public bool WholeWord { get; set; } = true;

        public bool UseRegularExpression { get; set; } = true;
        public string RegularExpression { get; set; } = @"C-\d{4}-\d{5}";

        public bool RedactEmailAddresses { get; set; } = true;
        public bool RedactPhoneNumbers { get; set; } = true;
        public bool RedactIbans { get; set; } = true;
        public bool RedactPaymentCardNumbers { get; set; } = true;
        public bool RedactIpAddresses { get; set; } = true;
        public bool RedactSocialSecurityNumbers { get; set; } = true;
        public bool RedactPersonalNumericCodes { get; set; } = true;

        public string FillColor { get; set; } = "Black";
        public string OverlayText { get; set; } = "REDACTED";
        public bool RepeatOverlayText { get; set; } = false;

        public bool RemoveMetadata { get; set; } = true;
        public bool RemoveJavaScript { get; set; } = true;
        public bool RemoveEmbeddedFiles { get; set; } = true;
        public bool RemoveHiddenContent { get; set; } = true;

        public bool SaveRedactionMarks { get; set; } = false;
    }
}
