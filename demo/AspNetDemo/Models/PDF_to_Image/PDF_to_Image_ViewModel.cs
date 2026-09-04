using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_to_Image
{
    public class PDF_to_Image_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public int StartPageNumber { get; set; } = 1;
        public int? EndPageNumber { get; set; } = null;

        public string ColorSpace { get; set; } = "RGB";
        public int Resolution { get; set; } = 150;
        public bool TransparencyEnabled { get; set; } = false;
    }
}