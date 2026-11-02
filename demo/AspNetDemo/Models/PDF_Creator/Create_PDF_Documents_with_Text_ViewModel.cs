using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Creator
{
    public class Create_PDF_Documents_with_Text_ViewModel
    {
        public PdfStandard PdfStandard { get; set; } = PdfStandard.None;

        public float Leading { get; set; } = 0;
        public bool AddPageNumbers { get; set; } = true;
        public int PageNumberOffset { get; set; } = 0;
        public int TotalPagesOffset { get; set; } = 0;
        public bool ReportRenderedPages { get; set; } = true;
    }
}
