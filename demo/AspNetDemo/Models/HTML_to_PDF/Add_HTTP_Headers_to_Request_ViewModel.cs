namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Add_HTTP_Headers_to_Request_ViewModel
    {
        public string Url { get; set; } = "http://www.evopdf.com/HTTP_Headers/";

        public string Header1Name { get; set; } = "Header1";
        public string Header1Value { get; set; } = "Value 1";

        public string Header2Name { get; set; } = "Header2";
        public string Header2Value { get; set; } = "Value 2";

        public string Header3Name { get; set; } = "Header3";
        public string Header3Value { get; set; } = "Value 3";

        public string Header4Name { get; set; } = "Header4";
        public string Header4Value { get; set; } = "Value 4";

        public string Header5Name { get; set; } = "Header5";
        public string Header5Value { get; set; } = "Value 5";

        public bool PersistentHttpHeaders { get; set; } = false;
    }
}