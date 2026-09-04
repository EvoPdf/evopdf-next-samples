using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- PdfProcessor.PdfToText [arguments]
    public static class PdfProcessor_PdfToText
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string pdfFile = args.Length > 0 ? args[0] : SampleFiles.Input("PDF_Document.pdf");
            var converter = new PdfToTextConverter();
            converter.TextLayout = PdfToTextLayout.Original;   // or PdfToTextLayout.Reading
            converter.MarkPageBreaks = true;

            string text = converter.ConvertToText(pdfFile);      // all pages; overloads take (startPage[, endPage])
            File.WriteAllText(SampleFiles.Output("output.txt"), text);
            Console.WriteLine($"output/output.txt: {text.Length:N0} characters");
        }
    }
}
