using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Creator
{
    public class Create_PDF_Forms_from_Code_ViewModel
    {
        public bool AddFieldActions { get; set; } = true;
        public bool AddDocumentScript { get; set; } = true;
        public PdfCheckBoxStyle CheckBoxStyle { get; set; } = PdfCheckBoxStyle.Check;
        public PdfFormFieldBorderStyle BorderStyle { get; set; } = PdfFormFieldBorderStyle.Solid;
        public bool RequiredFields { get; set; } = true;
        public bool FlattenFields { get; set; } = false;
        public PdfStandard PdfStandard { get; set; } = PdfStandard.None;
    }
}
