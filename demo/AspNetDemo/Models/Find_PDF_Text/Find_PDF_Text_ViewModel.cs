using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_to_Text
{
    public class Find_PDF_Text_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }

        public string TextToFind { get; set; } = "PDF";

        public bool CaseSensitive { get; set; } = false;
        public bool WholeWord { get; set; } = false;

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public int StartPageNumber { get; set; } = 1;
        public int? EndPageNumber { get; set; } = null;        
    }
}