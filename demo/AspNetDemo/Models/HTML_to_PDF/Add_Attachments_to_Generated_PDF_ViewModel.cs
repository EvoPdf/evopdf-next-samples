using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Add_Attachments_to_Generated_PDF_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Url";
        public string Url { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";
        public string HtmlString { get; set; }
        public string BaseUrl { get; set; } = string.Empty;
        public PdfStandard PdfStandard { get; set; } = PdfStandard.None;
    }
}
