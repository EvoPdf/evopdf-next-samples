using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Editor
{
    public class Split_PDF_Documents_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public string SplitOperation { get; set; } = "ExtractPage";
        public int PageNumber { get; set; } = 2;
        public int FirstPage { get; set; } = 2;
        public int LastPage { get; set; } = 4;
        public string PageRanges { get; set; } = "1,3-4,6-";
        public string RemovePageRanges { get; set; } = "2,5";
        public int PagesPerDocument { get; set; } = 2;
        public string SplitPageRanges { get; set; } = "1-2;3-5;6-";

        public bool SetDocumentTitle { get; set; } = false;
        public string DocumentTitle { get; set; } = "Split Document";
    }
}
