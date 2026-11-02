using Microsoft.AspNetCore.Http;
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.Markdown_to_PDF
{
    public class Add_Markdown_to_PDF_Documents_ViewModel
    {
        public string MarkdownSource { get; set; } = "Url";

        public string MarkdownFileUrl { get; set; }
        public IFormFile MarkdownFile { get; set; }
        public string MarkdownString { get; set; }
        public string BaseUrl { get; set; } = string.Empty;

        public string StyleSheet { get; set; } = string.Empty;
        public bool DisableRawHtml { get; set; } = false;

        public float ElementX { get; set; } = 0;
        public float ElementWidth { get; set; } = 0;
        public double ElementZoom { get; set; } = 100;

        public bool FitWideContentToPage { get; set; } = false;
        public bool GenerateDocumentOutline { get; set; } = true;

        public PdfStandard PdfStandard { get; set; } = PdfStandard.None;
    }
}
