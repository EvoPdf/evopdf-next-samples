using System.Diagnostics;
using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- HtmlToPdf.Parallel [url1 url2 ...]
    public static class HtmlToPdf_Parallel
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string[] urls = args.Length > 0 ? args : new[]
            {
                "https://www.evopdf.com",
                "https://www.evopdf.com/evopdf-next-dotnet",
                "https://www.evopdf.com/support",
            };

            var sw = Stopwatch.StartNew();

            // Start one conversion per URL, each with its own converter, and wait for all of them
            Task<byte[]>[] conversions = urls.Select(url => new HtmlToPdfConverter().ConvertUrlAsync(url)).ToArray();
            byte[][] pdfs = Task.WhenAll(conversions).GetAwaiter().GetResult();

            // Merge the documents in the order of the URLs
            using var merge = new PdfMerge();
            foreach (byte[] pdf in pdfs)
                merge.AddPdf(pdf);
            byte[] merged = merge.Save();

            File.WriteAllBytes(SampleFiles.Output("parallel.pdf"), merged);
            Console.WriteLine($"output/parallel.pdf: {urls.Length} URLs converted in parallel and merged in {sw.ElapsedMilliseconds} ms");
        }
    }
}
