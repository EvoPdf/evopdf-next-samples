namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class HTML_in_Header_Footer_ViewModel
    {
        public string Url { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";

        public string HeaderHtmlSource { get; set; } = "Html";
        public string HeaderHtmlTextBox { get; set; }
        public string HeaderHtmlBaseUrlTextBox { get; set; }
        public string HeaderUrlTextBox { get; set; }
        public bool HeaderEnabled { get; set; } = true;
        public bool AutoSizeHeaderContentHeight { get; set; } = true;
        public int HeaderMinContentHeight { get; set; } = 0;
        public int HeaderMaxContentHeight { get; set; } = 300;
        public int? HeaderHeight { get; set; }
        public bool FitHeaderHeight { get; set; } = true;
        public bool SkipHeaderVariablesParsing { get; set; } = false;
        public bool ShowHeaderInFirstPage { get; set; } = true;
        public bool ShowHeaderInOddPages { get; set; } = true;
        public bool ShowHeaderInEvenPages { get; set; } = true;
        public bool AutoResizeTopMargin { get; set; } = true;
        public bool ReserveHeaderSpace { get; set; } = true;
        public int? HeaderConversionDelay { get; set; } = 0;

        public string FooterHtmlSource { get; set; } = "Url";
        public string FooterHtmlTextBox { get; set; }
        public string FooterHtmlBaseUrlTextBox { get; set; }
        public string FooterUrlTextBox { get; set; }
        public bool FooterEnabled { get; set; } = true;
        public bool AutoSizeFooterContentHeight { get; set; } = true;
        public int FooterMinContentHeight { get; set; } = 0;
        public int FooterMaxContentHeight { get; set; } = 300;
        public int? FooterHeight { get; set; }
        public bool FitFooterHeight { get; set; } = true;
        public bool SkipFooterVariablesParsing { get; set; } = false;
        public bool ShowFooterInFirstPage { get; set; } = true;
        public bool ShowFooterInOddPages { get; set; } = true;
        public bool ShowFooterInEvenPages { get; set; } = true;
        public bool AutoResizeBottomMargin { get; set; } = true;
        public bool ReserveFooterSpace { get; set; } = true;
        public int? FooterConversionDelay { get; set; } = 0;
    }
}