using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.Word_to_PDF
{
    public class Word_to_PDF_with_Document_Headers_ViewModel
    {
        public string WordFileUrl { get; set; }
        public IFormFile WordFile { get; set; }

        public bool UseHeaderFooterFromWord { get; set; } = true;

        public bool AddHtmlHeader { get; set; } = false;
        public string HeaderHtmlTextBox { get; set; }
        public string HeaderHtmlBaseUrlTextBox { get; set; }

        public bool AddHtmlFooter { get; set; } = false;
        public string FooterHtmlTextBox { get; set; }
        public string FooterHtmlBaseUrlTextBox { get; set; }
    }
}
