using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- RtfToPdf [arguments]
    public static class RtfToPdf
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string rtf = File.ReadAllText(args.Length > 0 ? args[0] : SampleFiles.Input("RTF_Document.rtf"));
            var converter = new RtfToPdfConverter();
            byte[] pdf = converter.ConvertStringToPdf(rtf);
            File.WriteAllBytes(SampleFiles.Output("rtf.pdf"), pdf);
            Console.WriteLine("output/rtf.pdf written");
        }
    }
}
