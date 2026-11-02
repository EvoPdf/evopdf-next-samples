// Use EVO PDF Namespace
using EvoPdf.Next;

public byte[] ConvertHtmlThroughProxy(string url, string proxyHostName, int proxyPortNumber,
    string proxyUsername, string proxyPassword, string[] bypassedHosts)
{
    // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
    // Licensing.LicenseKey = "your-license-key";

    // Create a HTML to PDF converter object with default settings
    HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

    // The type of the proxy used to load the HTML page and its resources. With the None type
    // the network settings of the system are used, as when no proxy is configured
    htmlToPdfConverter.ProxyOptions.Type = NetworkProxyType.Http;

    // The address of the proxy server
    htmlToPdfConverter.ProxyOptions.HostName = proxyHostName;
    htmlToPdfConverter.ProxyOptions.PortNumber = proxyPortNumber;

    // The credentials for a proxy which requires authentication
    if (!string.IsNullOrEmpty(proxyUsername))
    {
        htmlToPdfConverter.ProxyOptions.Username = proxyUsername;
        htmlToPdfConverter.ProxyOptions.Password = proxyPassword;
    }

    // The hosts accessed directly, without the proxy: host names, IP addresses, domains with
    // their subdomains (*.example.com) or CIDR ranges (10.0.0.0/8). The local machine is
    // always accessed directly
    if (bypassedHosts != null)
        htmlToPdfConverter.ProxyOptions.BypassedHosts = bypassedHosts;

    // The same proxy options are available in the HTML to image converter

    // Convert the HTML page to a PDF document in a memory buffer
    byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);

    return outPdfBuffer;
}
