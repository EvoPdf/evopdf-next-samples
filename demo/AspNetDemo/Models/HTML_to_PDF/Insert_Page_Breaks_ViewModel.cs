namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Insert_Page_Breaks_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Html";
        public string HtmlString { get; set; }
        public string BaseUrl { get; set; }
        public string Url { get; set; }
    }
}