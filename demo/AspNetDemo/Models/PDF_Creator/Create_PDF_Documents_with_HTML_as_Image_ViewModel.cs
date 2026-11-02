using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Creator
{
    public class Create_PDF_Documents_with_HTML_as_Image_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Html";
        public string Url { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";
        public string HtmlStringTextBox { get; set; }
        public string BaseUrlTextBox { get; set; } = "";

        public float ElementX { get; set; } = 0;
        public float ElementY { get; set; } = 0;
        public float ElementWidth { get; set; } = 0;
        public float ElementHeight { get; set; } = 0;
        public int HtmlViewerWidth { get; set; } = 1024;
        public int Resolution { get; set; } = 150;
        public bool ContinueOnNextPage { get; set; } = true;

        public PdfStandard PdfStandard { get; set; } = PdfStandard.None;
    }
}
