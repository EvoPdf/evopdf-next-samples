namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Merge_Multiple_HTML_to_PDF_ViewModel
    {
        public string FirstUrl { get; set; } = "http://www.evopdf.com";
        public string SecondUrl { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";

        // Continue: the second document starts where the first one ends; NewPage: it starts on a new page
        public string SecondDocumentStart { get; set; } = "NewPage";

        public int HtmlViewerWidth { get; set; } = 1024;
        public int? HtmlViewerHeight { get; set; } = 2048;

        public string PdfPageSize { get; set; } = "A4";
        public string PdfPageOrientation { get; set; } = "Portrait";
        public string PageLayout { get; set; } = "FitBrowserWindowToPage";

        public int LeftMargin { get; set; } = 0;
        public int RightMargin { get; set; } = 0;
        public int TopMargin { get; set; } = 0;
        public int BottomMargin { get; set; } = 0;

        // None, Document (one table of contents for the whole document) or EachDocument (one for each HTML document)
        public string TableOfContents { get; set; } = "None";
        // Inline (at the start of each document) or SeparatePages (on its own pages before each document, when the
        // document starts on a new page)
        public string TocPlacement { get; set; } = "Inline";
        public bool AutoBookmarks { get; set; } = false;

        public int NavigationTimeout { get; set; } = 120;
        public int? ConversionDelay { get; set; } = 0;

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
