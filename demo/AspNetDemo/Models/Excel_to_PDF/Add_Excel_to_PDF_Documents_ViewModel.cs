using Microsoft.AspNetCore.Http;
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models.Excel_to_PDF
{
    public class Add_Excel_to_PDF_Documents_ViewModel
    {
        public string ExcelFileUrl { get; set; }
        public IFormFile ExcelFile { get; set; }

        public float ElementX { get; set; } = 0;
        public float ElementWidth { get; set; } = 0;
        public double ElementZoom { get; set; } = 100;

        public bool KeepColumnWidths { get; set; } = true;
        public bool KeepRowHeights { get; set; } = true;
        public bool ConvertOnlyFirstWorksheet { get; set; } = false;
        public bool PageBreakBetweenWorksheets { get; set; } = true;
        public bool DisplayWorksheetTitle { get; set; } = true;
        public bool FitWideContentToPage { get; set; } = true;

        public PdfStandard PdfStandard { get; set; } = PdfStandard.None;
    }
}
