using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- PdfEditor.EditPages [pdf file]
    public static class PdfEditor_EditPages
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string pdfFile = args.Length > 0 ? args[0] : SampleFiles.Input("PDF_Document.pdf");
            using var editor = new PdfEditor(File.ReadAllBytes(pdfFile), null);
            int loadedPages = editor.PageCount;

            // page operations come before the content added to the pages concerned
            int coverPage = editor.InsertPage(1, PdfPageSize.A4, PdfPageOrientation.Portrait);
            if (editor.PageCount > 2)
                editor.RotatePage(editor.PageCount, 90);

            // HTML content on the inserted cover page, as text and vectors, from (36, 72) in points
            var cover = new PdfHtmlElement(
                "<h1 style='font-family:Arial'>Cover page</h1><p style='font-family:Arial'>Inserted before the " +
                loadedPages + " pages of the loaded document.</p>", null)
            {
                X = 36,
                Y = 72,
                Width = 523
            };
            editor.AddHtml(coverPage, cover);

            // the document opens on the cover page and has a bookmark for it
            editor.Bookmarks.Insert(0, new PdfBookmark("Cover", coverPage, PdfLinkPageLocation.FitPage()));

            File.WriteAllBytes(SampleFiles.Output("edited-pages.pdf"), editor.Save());
            Console.WriteLine($"output/edited-pages.pdf written ({loadedPages} pages loaded)");
        }
    }
}
