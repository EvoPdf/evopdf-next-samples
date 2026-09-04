namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Convert_Multiple_HTML_to_PDF_in_Parallel_ViewModel
    {
        public string FirstUrl { get; set; } = "http://www.evopdf.com";
        public string SecondUrl { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";

        public bool AsyncParallelConversionEnabled { get; set; } = true;

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
        
        public bool AutoBookmarks { get; set; } = false;

        public int NavigationTimeout { get; set; } = 120;
        public int? ConversionDelay { get; set; } = 2;

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public bool PrintEnabled { get; set; } = true;
        public bool FillFormFieldsEnabled { get; set; } = true;
        public bool EditContentEnabled { get; set; } = true;
        public bool EditAnnotationsEnabled { get; set; } = true;
        public bool CopyContentEnabled { get; set; } = true;
        public bool CopyAccessibilityContentEnabled { get; set; } = true;

        public string EncryptionType { get; set; } = "RC4";
        public string EncryptionKey { get; set; } = "Bit128";
    }
}