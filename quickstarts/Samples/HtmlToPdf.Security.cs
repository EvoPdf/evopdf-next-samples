using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- HtmlToPdf.Security [arguments]
    public static class HtmlToPdf_Security
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            var converter = new HtmlToPdfConverter();
            var sec = converter.PdfSecurityOptions;
            sec.UserPassword = "open-me";
            sec.OwnerPassword = "owner-secret";
            sec.CanPrint = true;
            sec.CanCopyContent = false;
            sec.CanEditContent = false;

            var info = converter.PdfDocumentInfo;
            info.Title = "Protected sample";
            info.AuthorName = "EVO PDF Software";

            byte[] pdf = converter.ConvertHtml("<h1>Protected</h1><p>Open with the user password.</p>", "https://www.evopdf.com");
            File.WriteAllBytes(SampleFiles.Output("secured.pdf"), pdf);
            Console.WriteLine("output/secured.pdf written");
        }
    }
}
