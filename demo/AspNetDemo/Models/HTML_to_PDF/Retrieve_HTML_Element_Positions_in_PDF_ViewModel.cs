namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Retrieve_HTML_Element_Positions_in_PDF_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Html";
        public string HtmlString { get; set; }
        public string BaseUrl { get; set; }
        public string Url { get; set; }

        public string RetrieveElementsInfoSelector { get; set; } = "h1, h2, h3, h4";
        public bool EnableElementsInfoSelector { get; set; } = true;
        public bool GenerateToc { get; set; } = false;
        public int? ConversionDelay { get; set; } = 0;
    }
}