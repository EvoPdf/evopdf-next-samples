using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- ExcelToPdf [arguments]
    public static class ExcelToPdf
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string xlsx = args.Length > 0 ? args[0] : SampleFiles.Input("Excel_Document.xlsx");
            var converter = new ExcelToPdfConverter();
            converter.PdfDocumentOptions.UsePageSettingsFromExcel = true;   // ExcelToPdfDocumentOptions
            byte[] pdf = converter.ConvertToPdf(xlsx);
            File.WriteAllBytes(SampleFiles.Output("excel.pdf"), pdf);
            Console.WriteLine("output/excel.pdf written");
        }
    }
}
