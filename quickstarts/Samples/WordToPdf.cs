using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- WordToPdf [arguments]
    public static class WordToPdf
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string docx = args.Length > 0 ? args[0] : SampleFiles.Input("Word_Document.docx");
            var converter = new WordToPdfConverter();
            converter.PdfDocumentOptions.GenerateTableOfContents = false;
            byte[] pdf = converter.ConvertToPdf(docx);
            File.WriteAllBytes(SampleFiles.Output("word.pdf"), pdf);
            Console.WriteLine("output/word.pdf written");
        }
    }
}
