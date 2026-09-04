using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.Word_to_PDF
{
    public class Word_to_PDF_ViewModel
    {
        public string WordFileUrl { get; set; }
        public IFormFile WordFile { get; set; }

        public bool GenerateToc { get; set; } = false;

        public string PdfPageSettingsMode { get; set; } = "FromWordDocument";
        public bool ProcessWordPageBreakMarks { get; set; } = true;

        public string PdfPageSize { get; set; } = "A4";
        public string PdfPageOrientation { get; set; } = "Portrait";
        public int WordViewerZoom { get; set; } = 100;

        public int LeftMargin { get; set; } = 0;
        public int RightMargin { get; set; } = 0;
        public int TopMargin { get; set; } = 0;
        public int BottomMargin { get; set; } = 0;

        public bool HeaderEnabled { get; set; } = false;
        public string HeaderHtmlSource { get; set; } = "Url";
        public string HeaderHtmlTextBox { get; set; }
        public string HeaderHtmlBaseUrlTextBox { get; set; }
        public string HeaderUrlTextBox { get; set; }
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

        public bool FooterEnabled { get; set; } = false;
        public string FooterHtmlSource { get; set; } = "Url";
        public string FooterHtmlTextBox { get; set; }
        public string FooterHtmlBaseUrlTextBox { get; set; }
        public string FooterUrlTextBox { get; set; }
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