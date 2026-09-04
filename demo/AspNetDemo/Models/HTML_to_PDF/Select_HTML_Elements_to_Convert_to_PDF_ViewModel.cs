namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Select_HTML_Elements_to_Convert_to_PDF_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Html";
        public string HtmlString { get; set; }
        public string BaseUrl { get; set; }
        public string Url { get; set; }

        public bool EnableElementsSelector { get; set; } = true;
        public string ConvertedElementsSelector { get; set; } = "#ConvertedHtmlElement";
        public bool RemoveUnselectedElements { get; set; } = true;
        public bool AutoResizePdfPageHeight { get; set; } = false;
    }
}