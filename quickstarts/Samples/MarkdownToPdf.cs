using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- MarkdownToPdf [arguments]
    public static class MarkdownToPdf
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string markdown = File.ReadAllText(args.Length > 0 ? args[0] : SampleFiles.Input("Markdown_Document.md"));
            var converter = new MarkdownToPdfConverter();
            byte[] pdf = converter.ConvertStringToPdf(markdown, "https://www.evopdf.com");
            File.WriteAllBytes(SampleFiles.Output("markdown.pdf"), pdf);
            Console.WriteLine("output/markdown.pdf written");
        }
    }
}
