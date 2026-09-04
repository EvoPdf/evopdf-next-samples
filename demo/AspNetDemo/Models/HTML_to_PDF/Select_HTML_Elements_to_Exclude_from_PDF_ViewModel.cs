namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Select_HTML_Elements_to_Exclude_from_PDF_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Html";
        public string HtmlString { get; set; }
        public string BaseUrl { get; set; }
        public string Url { get; set; }

        public bool EnableExcludedElementsSelector { get; set; } = true;
        public string ExcludedElementsSelector { get; set; } = "#ExcludedHtmlElement";
        public bool RemoveExcludedElements { get; set; } = true;
    }
}