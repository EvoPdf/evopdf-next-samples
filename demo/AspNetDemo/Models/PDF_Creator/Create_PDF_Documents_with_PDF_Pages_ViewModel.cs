using EvoPdf.Next;
using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Creator
{
    public class Create_PDF_Documents_with_PDF_Pages_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }
        public string PdfPassword { get; set; } = string.Empty;

        public bool UseLetterhead { get; set; } = true;
        public bool RenderUnderContent { get; set; } = true;
        public float LetterheadOpacity { get; set; } = 1;

        public bool DrawPages { get; set; } = true;
        public string DrawnPages { get; set; } = "1,2,3,4";
        public float PageWidth { get; set; } = 150;
        public float PageHeight { get; set; } = 210;
        public float PageOpacity { get; set; } = 1;
        public float RotationDegrees { get; set; } = 0;
        public PdfRotationPivot RotationPivot { get; set; } = PdfRotationPivot.TopLeft;
        public bool IncludeAnnotations { get; set; } = true;
        public bool DrawFullSizePage { get; set; } = true;

        public PdfStandard PdfStandard { get; set; } = PdfStandard.None;
    }
}
