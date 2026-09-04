using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- HtmlToPdf.PageSetup [arguments]
    public static class HtmlToPdf_PageSetup
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            var converter = new HtmlToPdfConverter();
            converter.HtmlViewerWidth = 1024;                 // layout width in pixels (96 DPI)

            var o = converter.PdfDocumentOptions;
            o.PdfPageSize = PdfPageSize.A4;
            o.PdfPageOrientation = PdfPageOrientation.Portrait;
            o.AutoResizePdfPageWidth = false;                 // keep the exact page size instead of following HtmlViewerWidth
            o.AutoResizePdfPageHeight = false;
            o.LeftMargin = o.RightMargin = o.TopMargin = o.BottomMargin = 36; // points

            byte[] pdf = converter.ConvertUrl(args.Length > 0 ? args[0] : "https://www.evopdf.com");
            File.WriteAllBytes(SampleFiles.Output("a4.pdf"), pdf);
            Console.WriteLine("output/a4.pdf written");
        }
    }
}
