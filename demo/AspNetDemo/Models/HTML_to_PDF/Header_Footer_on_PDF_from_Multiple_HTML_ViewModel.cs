namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Header_Footer_on_PDF_from_Multiple_HTML_ViewModel
    {
        public string FirstUrl { get; set; } = "http://www.evopdf.com";
        public string SecondUrl { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";

        // Continue: the second document starts where the first one ends; NewPage: it starts on a new page
        public string SecondDocumentStart { get; set; } = "NewPage";

        // The header and the footer are drawn at the zoom of the HTML documents, so the fonts have the same sizes
        public bool TemplateZoomFromDocuments { get; set; } = true;

        public string HeaderHtmlSource { get; set; } = "Url";
        public string HeaderHtmlTextBox { get; set; }
        public string HeaderHtmlBaseUrlTextBox { get; set; }
        public string HeaderUrlTextBox { get; set; }
        public bool HeaderEnabled { get; set; } = true;
        // The height reserved for the header at the top of the pages, in points
        public int HeaderHeight { get; set; } = 60;
        public bool FitHeaderHeight { get; set; } = true;
        // The zoom at which the header HTML is laid out and drawn, in percent
        public double HeaderZoom { get; set; } = 100;
        public bool SkipHeaderVariablesParsing { get; set; } = false;
        public bool ShowHeaderInFirstPage { get; set; } = true;
        public bool ShowHeaderInOddPages { get; set; } = true;
        public bool ShowHeaderInEvenPages { get; set; } = true;
        public int? HeaderConversionDelay { get; set; } = 0;

        public string FooterHtmlSource { get; set; } = "Url";
        public string FooterHtmlTextBox { get; set; }
        public string FooterHtmlBaseUrlTextBox { get; set; }
        public string FooterUrlTextBox { get; set; }
        public bool FooterEnabled { get; set; } = true;
        // The height reserved for the footer at the bottom of the pages, in points
        public int FooterHeight { get; set; } = 60;
        public bool FitFooterHeight { get; set; } = true;
        // The zoom at which the footer HTML is laid out and drawn, in percent
        public double FooterZoom { get; set; } = 100;
        public bool SkipFooterVariablesParsing { get; set; } = false;
        public bool ShowFooterInFirstPage { get; set; } = true;
        public bool ShowFooterInOddPages { get; set; } = true;
        public bool ShowFooterInEvenPages { get; set; } = true;
        public int? FooterConversionDelay { get; set; } = 0;
    }
}
