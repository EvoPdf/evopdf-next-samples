namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_Image
{
    public class Select_HTML_Elements_to_Convert_to_Image_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Html";
        public string HtmlString { get; set; } = "";
        public string BaseUrl { get; set; } = "";
        public string Url { get; set; } = "";

        public string ConvertedElementsSelector { get; set; } = "#ConvertedHtmlElement";
        public bool EnableElementsSelector { get; set; } = true;
        public bool RemoveUnselectedElements { get; set; } = true;
    }
}