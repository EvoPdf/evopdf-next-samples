using EvoPdf.Next;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;
using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Editor
{
    public class Stamp_with_HTML_Existing_PDF_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public string StampHtmlSource { get; set; } = "Html";
        public string StampHtml { get; set; }
        public string StampHtmlBaseUrl { get; set; }
        public string StampUrl { get; set; }

        public bool StampEnabled { get; set; } = true;
        public int StampOpacity { get; set; } = 100;
        public bool ShowStampInFirstPage { get; set; } = true;
        public bool ShowStampInOddPages { get; set; } = true;
        public bool ShowStampInEvenPages { get; set; } = true;
        public bool SkipVariablesParsing { get; set; } = false;
        public int? ConversionDelay { get; set; } = 0;

        public bool AutoSizeStampContentHeight { get; set; } = true;
        public int StampMinContentHeight { get; set; } = 0;
        public int StampMaxContentHeight { get; set; } = 500;

        public int StampWidth { get; set; } = 500;
        public int? StampHeight { get; set; } = 500;
        public bool FitStampHeight { get; set; } = true;

        public int StampXPosition { get; set; } = 0;
        public int StampYPosition { get; set; } = 0;

        public PdfTemplateHorizontalAlign HorizontalAlign { get; set; } = PdfTemplateHorizontalAlign.Center;
        public PdfTemplateVerticalAlign VerticalAlign { get; set; } = PdfTemplateVerticalAlign.Center;

        public RotationDirection RotationDirection { get; set; } = RotationDirection.Clockwise;
        public int RotationDegrees { get; set; } = 0;
        public PdfRotationPivot RotationPivot { get; set; } = PdfRotationPivot.Center;
    }
}