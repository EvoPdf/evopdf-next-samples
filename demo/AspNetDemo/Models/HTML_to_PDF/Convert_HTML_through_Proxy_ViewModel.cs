namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Convert_HTML_through_Proxy_ViewModel
    {
        public string Url { get; set; } = "http://www.evopdf.com";

        public string ProxyType { get; set; } = "None";
        public string ProxyHostName { get; set; } = "proxy.example.com";
        public int ProxyPortNumber { get; set; } = 8080;
        public string ProxyUsername { get; set; } = string.Empty;
        public string ProxyPassword { get; set; } = string.Empty;
        public string BypassedHosts { get; set; } = "localhost, *.intranet.example.com, 10.0.0.0/8";
    }
}
