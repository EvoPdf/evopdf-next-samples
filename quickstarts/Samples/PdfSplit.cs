using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- PdfSplit [pdf file] [page ranges]
    public static class PdfSplit_Pages
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string pdfFile = args.Length > 0 ? args[0] : SampleFiles.Input("PDF_Document.pdf");
            string ranges = args.Length > 1 ? args[1] : "2,1";

            using var split = new PdfSplit(File.ReadAllBytes(pdfFile), null);
            Console.WriteLine($"{split.PageCount} pages loaded");

            // the pages of the ranges, in the order of the ranges; page numbers are 1-based
            File.WriteAllBytes(SampleFiles.Output("split-ranges.pdf"), split.ExtractPages(ranges));

            // one document per page
            List<byte[]> parts = split.SplitByPageCount(1);
            for (int i = 0; i < parts.Count; i++)
                File.WriteAllBytes(SampleFiles.Output($"split-page-{i + 1}.pdf"), parts[i]);

            Console.WriteLine($"output/split-ranges.pdf and {parts.Count} single page documents written");
        }
    }
}
