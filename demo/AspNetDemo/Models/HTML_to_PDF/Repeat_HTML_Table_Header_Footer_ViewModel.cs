namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Repeat_HTML_Table_Header_Footer_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Html";
        public string HtmlString { get; set; }
        public string BaseUrl { get; set; }
        public string Url { get; set; }

        public bool RepeatTableHeaderFooter { get; set; } = true;
    }
}