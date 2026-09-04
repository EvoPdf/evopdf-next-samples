using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_to_Text
{
    public class PDF_to_Text_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public int StartPageNumber { get; set; } = 1;
        public int? EndPageNumber { get; set; } = null;

        public string TextLayout { get; set; } = "Original";
        public bool MarkPageBreaks { get; set; } = false;        
    }
}