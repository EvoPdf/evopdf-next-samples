using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Editor
{
    public class Edit_PDF_Pages_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public bool InsertCoverPage { get; set; } = true;
        public string CoverPageSize { get; set; } = "A4";
        public string CoverPageOrientation { get; set; } = "Portrait";

        public bool AddLastPage { get; set; } = true;

        public bool MovePage { get; set; } = false;
        public int MovePageNumber { get; set; } = 2;
        public int MoveToPageNumber { get; set; } = 3;

        public bool RotatePage { get; set; } = true;
        public int RotatePageNumber { get; set; } = 2;
        public int RotateDegrees { get; set; } = 90;

        public string RemovePageRanges { get; set; } = string.Empty;
    }
}
