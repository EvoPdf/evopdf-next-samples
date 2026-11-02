using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Creator
{
    public class Create_PDF_Documents_with_Shapes_ViewModel
    {
        public PdfStandard PdfStandard { get; set; } = PdfStandard.None;

        public float LineOpacity { get; set; } = 1;
        public float CornerRadius { get; set; } = 12;
        public float CircleRadius { get; set; } = 30;
        public float ArcStartAngle { get; set; } = 20;
        public float ArcSweepAngle { get; set; } = 200;
    }
}
