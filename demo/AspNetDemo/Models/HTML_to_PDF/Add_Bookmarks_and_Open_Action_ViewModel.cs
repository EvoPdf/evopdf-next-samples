namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Add_Bookmarks_and_Open_Action_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Url";
        public string Url { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";
        public string HtmlStringTextBox { get; set; }
        public string BaseUrlTextBox { get; set; } = "";

        public bool GenerateDocumentOutline { get; set; } = true;
        public bool AddBookmarks { get; set; } = true;
        public bool StyleBookmarks { get; set; } = true;
        public bool AddStartPdf { get; set; } = false;
        public bool ShowBookmarksPanel { get; set; } = true;
        public string OpenAction { get; set; } = "None";
    }
}
