using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- HtmlToPdf.DynamicContent [arguments]
    public static class HtmlToPdf_DynamicContent
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            var converter = new HtmlToPdfConverter();
            converter.JavaScriptEnabled = true;
            converter.NavigationTimeout = 120;      // seconds
            converter.ConversionDelay = 2;          // wait 2 s after load for scripts to finish

            // Alternative: the page decides when it is ready and calls evoPdfConverter_startConversion()
            // converter.TriggeringMode = TriggeringMode.Manual;

            byte[] pdf = converter.ConvertUrl(args.Length > 0 ? args[0] : "https://www.evopdf.com");
            File.WriteAllBytes(SampleFiles.Output("dynamic.pdf"), pdf);
            Console.WriteLine("output/dynamic.pdf written");
        }
    }
}
