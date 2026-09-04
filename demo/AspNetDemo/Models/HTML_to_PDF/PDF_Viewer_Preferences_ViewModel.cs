namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class PDF_Viewer_Preferences_ViewModel
    {
        public string Url { get; set; } = "http://www.evopdf.com/DemoAppFiles/HTML_Files/Structured_HTML.html";

        public string PageMode { get; set; } = "Default";
        public string PageLayout { get; set; } = "One Column";

        public bool HideMenuBar { get; set; } = false;
        public bool HideToolbar { get; set; } = false;
        public bool HideWindowUI { get; set; } = false;

        public bool DisplayDocTitle { get; set; } = false;
    }
}