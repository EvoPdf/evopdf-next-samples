using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class HTML_to_PDF_Getting_Started_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Url";

        public string Url { get; set; } = "http://www.evopdf.com";
        public string HtmlString { get; set; } = "Enter the <b>HTML String to Convert</b> and optionally set a <b>Base URL</b> if the HTML string references external resources by relative URLs";
        public string BaseUrl { get; set; } = string.Empty;

        public int HtmlViewerWidth { get; set; } = 1024;
        public int? HtmlViewerHeight { get; set; } = 2048;
        public double HtmlViewerZoom { get; set; } = 100;

        public bool LoadLazyImages { get; set; } = true;
        public string LazyImagesLoadMode { get; set; } = "Browser";

        public string MediaType { get; set; } = "Screen";

        public bool PrintBackgrounds { get; set; } = true;

        public bool JavaScriptEnabled { get; set; } = true;

        public string PdfPageSize { get; set; } = "A4";
        public string PdfPageOrientation { get; set; } = "Portrait";
        public string PageLayout { get; set; } = "FitBrowserWindowToPage";
        public bool SinglePage { get; set; } = false;

        public int LeftMargin { get; set; } = 0;
        public int RightMargin { get; set; } = 0;
        public int TopMargin { get; set; } = 0;
        public int BottomMargin { get; set; } = 0;

        public PdfStandard PdfStandard { get; set; } = PdfStandard.None;

        public int NavigationTimeout { get; set; } = 120;
        public int? ConversionDelay { get; set; } = 2;
        
        public bool OpenInline { get; set; } = false;
    }
}