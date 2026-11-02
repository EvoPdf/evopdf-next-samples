namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Retrieve_Conversion_Info_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Url";
        public string Url { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";
        public string HtmlStringTextBox { get; set; }
        public string BaseUrlTextBox { get; set; } = "";

        public bool GenerateTableOfContents { get; set; } = true;
        public bool AddStartPdf { get; set; } = true;
        public bool AddEndPdf { get; set; } = true;
        public string InfoPagePosition { get; set; } = "End";
    }
}
