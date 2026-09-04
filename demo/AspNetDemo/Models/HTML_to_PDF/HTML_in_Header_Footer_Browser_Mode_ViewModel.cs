namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class HTML_in_Header_Footer_Browser_Mode_ViewModel
    {
        public string Url { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";
        public bool EnableHeaderFooter { get; set; } = true;

        public string HeaderTemplate { get; set; }
        public int? HeaderHeight { get; set; } = 50;
        public bool HeaderEnabled { get; set; } = true;

        public string FooterTemplate { get; set; }
        public int? FooterHeight { get; set; } = 50;
        public bool FooterEnabled { get; set; } = true;
    }
}