using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Creator
{
    public class Create_PDF_Documents_with_Images_ViewModel
    {
        public PdfStandard PdfStandard { get; set; } = PdfStandard.None;

        public bool AddTallImage { get; set; } = true;
        public bool ContinueOnNextPage { get; set; } = true;
    }
}
