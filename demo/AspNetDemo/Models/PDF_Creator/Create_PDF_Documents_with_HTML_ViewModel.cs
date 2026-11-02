using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Creator
{
    public class Create_PDF_Documents_with_HTML_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Url";
        public string Url { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";
        public string HtmlStringTextBox { get; set; }
        public string BaseUrlTextBox { get; set; } = "";

        public float ElementX { get; set; } = 0;
        public float ElementWidth { get; set; } = 0;
        public string MediaType { get; set; } = "screen";
        public bool GenerateDocumentOutline { get; set; } = true;
        public bool RetrieveElementPositions { get; set; } = true;
        public string HtmlElementsSelector { get; set; } = "h1, h2";
        public bool AddSecondElement { get; set; } = true;
        public bool AddPageNumbers { get; set; } = true;
        public bool FollowingElementsOnNewPage { get; set; } = false;

        public PdfStandard PdfStandard { get; set; } = PdfStandard.None;

        public float ElementY { get; set; } = 0;
    }
}
