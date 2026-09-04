namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_Image
{
    public class Convert_HTML_to_Image_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Url";
        public string Url { get; set; } = "http://www.evopdf.com";
        public string HtmlString { get; set; } = "Enter the <b>HTML String to Convert</b> and optionally set a <b>Base URL</b> if the HTML string references external resources by relative URLs";
        public string BaseUrl { get; set; } = "";

        public int HtmlViewerWidth { get; set; } = 1024;
        public int? HtmlViewerHeight { get; set; } = 2048;
        public bool AutoResizeViewerHeight { get; set; } = false;

        public bool CaptureEntirePage { get; set; } = true;
        public string CaptureEntirePageMode { get; set; } = "Browser";

        public string ImageFormat { get; set; } = "Png";

        public int NavigationTimeout { get; set; } = 120;
        public int? ConversionDelay { get; set; } = 2;
    }
}