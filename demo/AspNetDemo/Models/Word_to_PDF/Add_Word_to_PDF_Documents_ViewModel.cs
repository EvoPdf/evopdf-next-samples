using Microsoft.AspNetCore.Http;
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.Word_to_PDF
{
    public class Add_Word_to_PDF_Documents_ViewModel
    {
        public string WordFileUrl { get; set; }
        public IFormFile WordFile { get; set; }

        public float ElementX { get; set; } = 0;
        public float ElementWidth { get; set; } = 0;
        public double ElementZoom { get; set; } = 100;

        public bool FitWideContentToPage { get; set; } = false;
        public bool GenerateDocumentOutline { get; set; } = true;
        public bool UseEmbeddedFonts { get; set; } = true;

        public PdfStandard PdfStandard { get; set; } = PdfStandard.None;
    }
}
