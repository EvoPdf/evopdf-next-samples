using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- PdfEditor.Stamp [arguments]
    public static class PdfEditor_Stamp
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string pdfFile = args.Length > 0 ? args[0] : SampleFiles.Input("PDF_Document.pdf");
            using var editor = new PdfEditor(File.ReadAllBytes(pdfFile), null);
            int pageCount = editor.GetPageCount();
            // a template is repeated on the pages of the document; (x, y, width) in points, HTML plus an optional base URL
            PdfHtmlTemplate stamp = editor.AddHtmlTemplate(40, 40, 400, "<div style='font:48px Arial;color:rgba(200,0,0,.35);transform:rotate(-30deg)'>DRAFT</div>", null);
            File.WriteAllBytes(SampleFiles.Output("stamped.pdf"), editor.Save());
            Console.WriteLine($"output/stamped.pdf written ({pageCount} pages)");
        }
    }
}
