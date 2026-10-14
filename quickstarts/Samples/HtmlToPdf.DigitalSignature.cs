using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- HtmlToPdf.DigitalSignature [url] [timestamp server url]
    public static class HtmlToPdf_DigitalSignature
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string url = args.Length > 0 ? args[0] : "https://www.evopdf.com";
            string? timestampServerUrl = args.Length > 1 ? args[1] : null;

            // The sample certificate of the demo application; use your own PFX file or an X509Certificate2
            var signature = new PdfDigitalSignature(SampleFiles.Input("evopdf.pfx"), "evopdf");
            signature.Reason = "Approved";
            signature.Location = "Head office";

            // PAdES B-B without a timestamp server, PAdES B-T with one
            if (timestampServerUrl != null)
            {
                signature.TimestampServerUrl = timestampServerUrl;
                signature.Profile = PdfDigitalSignatureProfile.PadesBaselineT;
            }
            else
            {
                signature.Profile = PdfDigitalSignatureProfile.PadesBaselineB;
            }

            // A visible signature at the bottom of the first A4 page (595 x 842 points)
            signature.AppearanceEnabled = true;
            signature.Appearance.PageNumber = 1;
            signature.Appearance.BoundsRectangle = new PdfRectangle(20, 772, 220, 50);
            signature.Appearance.Text = "Signed by EVO PDF Software";

            var converter = new HtmlToPdfConverter();
            converter.DigitalSignature = signature;
            byte[] pdf = converter.ConvertUrl(url);

            File.WriteAllBytes(SampleFiles.Output("signed.pdf"), pdf);
            Console.WriteLine($"output/signed.pdf written ({signature.Profile})");
        }
    }
}
