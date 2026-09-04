using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Images_Extractor
{
    public class Extract_PDF_Images_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public int StartPageNumber { get; set; } = 1;
        public int? EndPageNumber { get; set; } = null;

        public bool ExtractLargest { get; set; } = true;
    }
}