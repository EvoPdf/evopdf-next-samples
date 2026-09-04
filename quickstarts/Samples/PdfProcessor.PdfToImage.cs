using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- PdfProcessor.PdfToImage [arguments]
    public static class PdfProcessor_PdfToImage
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string pdfFile = args.Length > 0 ? args[0] : SampleFiles.Input("PDF_Document.pdf");
            var converter = new PdfToImageConverter();
            converter.Resolution = 150;                // DPI
            converter.TransparencyEnabled = false;

            PdfPageImage[] pages = converter.ConvertToImages(pdfFile);   // overloads: (file, startPage[, endPage])
            foreach (PdfPageImage page in pages)
                File.WriteAllBytes(SampleFiles.Output($"page-{page.PageNumber}.png"), page.ImageData);
            Console.WriteLine($"{pages.Length} page image(s) written to output/");

            // Or straight to files:
            // converter.ConvertToImageFiles(pdfFile, "out", "page");
        }
    }
}
