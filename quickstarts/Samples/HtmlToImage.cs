using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- HtmlToImage [arguments]
    public static class HtmlToImage
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            var converter = new HtmlToImageConverter();
            converter.HtmlViewerWidth = 1280;
            converter.CaptureEntirePage = true;

            byte[] png = converter.ConvertUrl(args.Length > 0 ? args[0] : "https://www.evopdf.com", ImageType.Png);
            File.WriteAllBytes(SampleFiles.Output("page.png"), png);
            Console.WriteLine($"output/page.png: {png.Length:N0} bytes");
        }
    }
}
