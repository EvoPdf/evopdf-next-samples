using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- PdfProcessor.ExtractImages [arguments]
    public static class PdfProcessor_ExtractImages
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string pdfFile = args.Length > 0 ? args[0] : SampleFiles.Input("PDF_Document.pdf");
            var extractor = new PdfImagesExtractor();

            ExtractedImage[][] imagesPerPage = extractor.ExtractImages(pdfFile);   // one array per page
            int count = 0;
            foreach (ExtractedImage[] pageImages in imagesPerPage)
                foreach (ExtractedImage image in pageImages)
                    File.WriteAllBytes(SampleFiles.Output($"image-{image.PageNumber}-{++count}.png"), image.ImageData);
            Console.WriteLine($"{count} image(s) extracted to output/");

            // Or straight to files:
            // extractor.ExtractImagesToFile(pdfFile, "out", "img");
        }
    }
}
