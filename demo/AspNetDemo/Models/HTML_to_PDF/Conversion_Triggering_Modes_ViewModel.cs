namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Conversion_Triggering_Modes_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Html";

        public string HtmlString { get; set; }
        public string BaseUrl { get; set; }
        public string Url { get; set; }

        public string TriggeringMode { get; set; } = "Auto";
        public int ConversionDelay { get; set; } = 3;
    }
}