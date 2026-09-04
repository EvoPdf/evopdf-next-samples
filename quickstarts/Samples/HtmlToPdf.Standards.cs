using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- HtmlToPdf.Standards [arguments]
    public static class HtmlToPdf_Standards
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            var converter = new HtmlToPdfConverter();
            var o = converter.PdfDocumentOptions;
            o.PdfStandard = PdfStandard.PdfUa1PdfA2b;       // PdfUa1, PdfA2b, PdfUa1PdfA2b, ...
            o.AccessibilityOptions.AddMissingImageAlternateText = true;
            o.AccessibilityOptions.InsertMissingTableHeaders = true;
            converter.PdfDocumentInfo.Language = "en-US";
            converter.PdfDocumentInfo.Title = "Accessible, archivable sample";

            byte[] pdf = converter.ConvertUrl(args.Length > 0 ? args[0] : "https://www.evopdf.com");
            File.WriteAllBytes(SampleFiles.Output("pdfua-pdfa.pdf"), pdf);
            Console.WriteLine("output/pdfua-pdfa.pdf written");
        }
    }
}
