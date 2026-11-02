using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.Redact_PDF
{
    public class Apply_Redaction_Annotations_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public bool RedactPersonalData { get; set; } = false;

        public bool RemoveMetadata { get; set; } = true;
        public bool RemoveJavaScript { get; set; } = true;
    }
}
