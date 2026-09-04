using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- HtmlToPdf.Basic [arguments]
    public static class HtmlToPdf_Basic
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string url = args.Length > 0 ? args[0] : "https://www.evopdf.com";

            var converter = new HtmlToPdfConverter();
            byte[] pdf = converter.ConvertUrl(url);
            File.WriteAllBytes(SampleFiles.Output("url.pdf"), pdf);
            Console.WriteLine($"output/url.pdf: {pdf.Length:N0} bytes");

            // Converter instances are not reusable: create a new one for every conversion.
            // HTML string; the base URL resolves relative images, CSS and links
            converter = new HtmlToPdfConverter();
            string html = "<html><body><h1>Hello from EvoPdf Next</h1><p>Converted on " + DateTime.Now + "</p></body></html>";
            pdf = converter.ConvertHtml(html, "https://www.evopdf.com");
            File.WriteAllBytes(SampleFiles.Output("string.pdf"), pdf);
            Console.WriteLine($"output/string.pdf: {pdf.Length:N0} bytes");
        }
    }
}
