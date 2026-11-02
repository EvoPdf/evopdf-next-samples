using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- PdfMerge.PageRanges [pdf file] [pdf file]
    public static class PdfMerge_PageRanges
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string first = args.Length > 0 ? args[0] : SampleFiles.Input("PDF_Document.pdf");
            string second = args.Length > 1 ? args[1] : SampleFiles.Input("PDF_Document.pdf");

            using var merge = new PdfMerge();
            // the add methods return the 1-based number of the first added page in the merged document
            int firstStart = merge.AddPdf(first);
            merge.AddBlankPage(PdfPageSize.A4);
            int secondStart = merge.AddPdfPages(second, "1");

            // a bookmark for each part, after the bookmarks of the merged documents
            merge.Bookmarks.Add("First document", firstStart, PdfLinkPageLocation.FitPage());
            merge.Bookmarks.Add("First page of the second document", secondStart, PdfLinkPageLocation.FitPage());
            merge.PdfDocumentInfo.Title = "Merged document";

            File.WriteAllBytes(SampleFiles.Output("merged.pdf"), merge.Save());
            Console.WriteLine($"output/merged.pdf written ({merge.PageCount} pages)");
        }
    }
}
