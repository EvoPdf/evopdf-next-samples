namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Create_PDF_Forms_from_HTML_Forms_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Html";
        public string HtmlString { get; set; }
        public string BaseUrl { get; set; }
        public string Url { get; set; }

        public bool GeneratePdfFormFields { get; set; } = true;
    }
}
