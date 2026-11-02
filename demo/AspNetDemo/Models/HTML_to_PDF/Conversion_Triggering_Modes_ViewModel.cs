namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Conversion_Triggering_Modes_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Html";

        public string HtmlString { get; set; }
        public string BaseUrl { get; set; }
        public string Url { get; set; }

        public string TriggeringMode { get; set; } = "Auto";
        public int ConversionDelay { get; set; } = 3;

        public bool ExecuteScriptAfterLoad { get; set; } = false;
        public string AfterLoadScript { get; set; } = "await new Promise(resolve => setTimeout(resolve, 2000));\n" +
            "document.body.insertAdjacentHTML('beforeend', '<p style=\"color: navy; font-size: 20px\">This paragraph was added by the script executed after the page was loaded</p>');";
        public bool WaitForAfterLoadScript { get; set; } = true;
    }
}
