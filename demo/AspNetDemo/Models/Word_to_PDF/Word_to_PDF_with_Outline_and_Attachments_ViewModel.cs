using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.Word_to_PDF
{
    public class Word_to_PDF_with_Outline_and_Attachments_ViewModel
    {
        public string WordFileUrl { get; set; }
        public IFormFile WordFile { get; set; }

        public bool GenerateDocumentOutline { get; set; } = true;
        public bool AddCustomBookmarks { get; set; } = true;
        public bool SetOpenAction { get; set; } = true;
        public bool AttachSourceDocument { get; set; } = true;
        public bool AddStartPdf { get; set; } = false;

        public string PageMode { get; set; } = "UseOutlines";
    }
}
