using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.Word_to_PDF
{
    public class Word_to_PDF_with_Fonts_ViewModel
    {
        public string WordFileUrl { get; set; }
        public IFormFile WordFile { get; set; }

        public bool UseEmbeddedFonts { get; set; } = true;
        public bool UseFontFile { get; set; } = true;
        public bool UseFallbackFontFile { get; set; } = true;
    }
}
