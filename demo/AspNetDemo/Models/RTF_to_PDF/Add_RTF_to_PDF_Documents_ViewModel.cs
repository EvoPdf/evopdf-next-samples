using Microsoft.AspNetCore.Http;
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.RTF_to_PDF
{
    public class Add_RTF_to_PDF_Documents_ViewModel
    {
        public string RtfSource { get; set; } = "Url";

        public string RtfFileUrl { get; set; }
        public IFormFile RtfFile { get; set; }
        public string RtfString { get; set; }

        public float ElementX { get; set; } = 0;
        public float ElementWidth { get; set; } = 0;
        public double ElementZoom { get; set; } = 100;

        public bool FitWideContentToPage { get; set; } = false;
        public bool GenerateDocumentOutline { get; set; } = true;

        public PdfStandard PdfStandard { get; set; } = PdfStandard.None;
    }
}
