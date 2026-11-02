using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Editor
{
    public class Add_HTML_to_Existing_PDF_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public string HtmlPageSource { get; set; } = "Url";
        public string Url { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";
        public string HtmlStringTextBox { get; set; }
        public string BaseUrlTextBox { get; set; } = "";

        public float ElementX { get; set; } = 0;
        public float ElementY { get; set; } = 0;
        public float ElementWidth { get; set; } = 523;
        public string MediaType { get; set; } = "screen";
        public bool GenerateDocumentOutline { get; set; } = true;
        public bool RetrieveElementPositions { get; set; } = true;
        public string HtmlElementsSelector { get; set; } = "h1, h2";
        public bool InsertContinuationPages { get; set; } = true;
    }
}
