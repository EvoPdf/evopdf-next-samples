namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Select_Screen_or_Print_Media_Type_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Html";
        public string HtmlString { get; set; }
        public string BaseUrl { get; set; }
        public string Url { get; set; }

        public string MediaType { get; set; } = "Screen";
    }
}