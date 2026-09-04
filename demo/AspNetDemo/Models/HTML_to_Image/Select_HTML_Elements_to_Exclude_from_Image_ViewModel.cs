namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_Image
{
    public class Select_HTML_Elements_to_Exclude_from_Image_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Html";
        public string HtmlString { get; set; } = "";
        public string BaseUrl { get; set; } = "";
        public string Url { get; set; } = "";

        public string ExcludedElementsSelector { get; set; } = "#ExcludedHtmlElement";
        public bool EnableExcludedElementsSelector { get; set; } = true;
        public bool RemoveExcludedElements { get; set; } = true;
    }
}