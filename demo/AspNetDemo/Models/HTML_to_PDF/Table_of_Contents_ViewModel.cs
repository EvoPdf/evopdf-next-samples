namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Table_of_Contents_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Url";
        public string Url { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";
        public string HtmlStringTextBox { get; set; }
        public string BaseUrlTextBox { get; set; } = "";

        public bool GenerateToc { get; set; } = true;
        public bool InlineToc { get; set; } = false;
        public bool UseBrowserMode { get; set; } = false;

        public bool ShowPageNumbers { get; set; } = true;
        public bool CountTocPages { get; set; } = true;
        public int PageNumbersOffset { get; set; } = 0;

        public string TocTitle { get; set; } = "Table of Contents";
        public string TocStyleTextBox { get; set; }

        public int HtmlViewerWidth { get; set; } = 1024;
        public int? HtmlViewerHeight { get; set; } = 2048;
        public int HtmlViewerZoom { get; set; } = 100;

        public string PdfPageSize { get; set; } = "A4";
        public string PdfPageOrientation { get; set; } = "Portrait";
        public bool AutoResizePdfPageWidth { get; set; } = true;

        public int LeftMargin { get; set; } = 0;
        public int RightMargin { get; set; } = 0;
        public int TopMargin { get; set; } = 0;
        public int BottomMargin { get; set; } = 0;

        public int NavigationTimeout { get; set; } = 120;
        public int? ConversionDelay { get; set; } = 2;
    }
}