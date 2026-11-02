using EvoPdf.Next;
using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Editor
{
    public class Add_PDF_Pages_to_Existing_PDF_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }
        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public string SourcePdfFileUrl { get; set; }
        public IFormFile SourcePdfFile { get; set; }
        public string SourcePassword { get; set; } = string.Empty;
        public int SourcePageNumber { get; set; } = 1;

        public string TargetPages { get; set; } = "All";
        public float ElementX { get; set; } = 0;
        public float ElementY { get; set; } = 0;
        public float ElementWidth { get; set; } = 0;
        public float ElementHeight { get; set; } = 0;
        public float Opacity { get; set; } = 1;
        public float RotationDegrees { get; set; } = 0;
        public PdfRotationPivot RotationPivot { get; set; } = PdfRotationPivot.TopLeft;
        public bool RenderUnderContent { get; set; } = true;
        public bool IncludeAnnotations { get; set; } = true;
        public bool AddCaption { get; set; } = true;
    }
}
