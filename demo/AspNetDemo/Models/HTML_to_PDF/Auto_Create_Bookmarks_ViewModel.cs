namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Auto_Create_Bookmarks_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Url";
        public string Url { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";
        public string HtmlStringTextBox { get; set; }
        public string BaseUrlTextBox { get; set; } = "";

        public bool GenerateDocumentOutline { get; set; } = true;
        public bool UseBrowserOutlineMode { get; set; } = false;
    }
}