using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- PdfDocument.MultipleHtml [url] [url]
    public static class PdfDocument_MultipleHtml
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string firstUrl = args.Length > 0 ? args[0] : "https://www.evopdf.com";
            string secondUrl = args.Length > 1 ? args[1] : "https://www.evopdf.com/evopdf-next-dotnet";

            using var document = new PdfDocument();

            // the first HTML document flows over as many pages as it needs
            PdfHtmlRenderInfo first = document.AddHtml(new PdfHtmlElement(firstUrl));

            // the second one starts on a new page; it could also continue below the first one,
            // from first.LastPageRectangle.Bounds.Bottom on page first.LastPageNumber
            document.AddPage();
            document.AddHtml(new PdfHtmlElement(secondUrl));

            File.WriteAllBytes(SampleFiles.Output("multiple-html.pdf"), document.Save());
            Console.WriteLine($"output/multiple-html.pdf written (the first document ends on page {first.LastPageNumber})");
        }
    }
}
