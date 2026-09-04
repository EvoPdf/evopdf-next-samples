using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- HtmlToPdf.HeadersFooters [arguments]
    public static class HtmlToPdf_HeadersFooters
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            var converter = new HtmlToPdfConverter();
            var o = converter.PdfDocumentOptions;

            var header = o.PdfHtmlHeader;
            header.Html = "<div style='font:12px Arial;color:#555;border-bottom:1px solid #ccc'>EvoPdf Next — sample header</div>";
            header.HtmlBaseUrl = "https://www.evopdf.com";
            header.Height = 40;

            var footer = o.PdfHtmlFooter;
            footer.Html = "<div style='font:10px Arial;text-align:right'>Page {page_number} of {total_pages}</div>";
            footer.HtmlBaseUrl = "https://www.evopdf.com";
            footer.Height = 30;
            footer.ShowInFirstPage = true;

            byte[] pdf = converter.ConvertUrl(args.Length > 0 ? args[0] : "https://www.evopdf.com");
            File.WriteAllBytes(SampleFiles.Output("headers-footers.pdf"), pdf);
            Console.WriteLine("output/headers-footers.pdf written");
        }
    }
}
