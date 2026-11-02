using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.Redact_PDF
{
    public class Redact_Areas_and_Pages_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public string Areas { get; set; } = "1, 36, 288, 525, 62\r\n1, 50, 505, 200, 60";
        public bool DrawOverlayImage { get; set; } = true;
        public bool RemoveIntersectingAnnotations { get; set; } = true;

        public string PagesToRedact { get; set; } = "2";
        public string PageOverlayText { get; set; } = "PAGE REMOVED";

        public string FillColor { get; set; } = "Black";
        public bool RemoveMetadata { get; set; } = false;
    }
}
