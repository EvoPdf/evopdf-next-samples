using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Editor
{
    public class Merge_PDF_Documents_ViewModel
    {
        public string FirstPdfFileUrl { get; set; }
        public IFormFile FirstPdfFile { get; set; }
        public string FirstPageRanges { get; set; } = string.Empty;
        public string FirstPassword { get; set; } = string.Empty;

        public string SecondPdfFileUrl { get; set; }
        public IFormFile SecondPdfFile { get; set; }
        public string SecondPageRanges { get; set; } = string.Empty;
        public string SecondPassword { get; set; } = string.Empty;

        public bool AddSecondPdfTwice { get; set; } = false;
        public bool KeepFieldsSeparate { get; set; } = false;
        public bool AddBlankPage { get; set; } = false;
        public string BlankPageSize { get; set; } = "A4";
        public string BlankPageOrientation { get; set; } = "Portrait";
        public bool AddImagePage { get; set; } = false;
        public bool FitImageInPage { get; set; } = false;

        public bool KeepSourceBookmarks { get; set; } = true;
        public bool AddDocumentBookmarks { get; set; } = true;
        public bool AddHeaderFooter { get; set; } = false;
        public string OpenAction { get; set; } = "None";
        public bool FlattenMergedPdf { get; set; } = false;
        public string DocumentTitle { get; set; } = "Merged Document";
    }
}
