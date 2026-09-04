using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- PdfProcessor.FindText [arguments]
    public static class PdfProcessor_FindText
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string pdfFile = args.Length > 0 ? args[0] : SampleFiles.Input("PDF_Document.pdf");
            string textToFind = args.Length > 1 ? args[1] : "PDF";

            var converter = new PdfToTextConverter();
            FindTextLocation[] hits = converter.FindText(pdfFile, textToFind, caseSensitive: false, wholeWord: false);
            Console.WriteLine($"{hits.Length} matches for '{textToFind}'");
            foreach (FindTextLocation hit in hits)
                Console.WriteLine($"page {hit.PageNumber}: x={hit.X:F1} y={hit.Y:F1} w={hit.Width:F1} h={hit.Height:F1}");
        }
    }
}
