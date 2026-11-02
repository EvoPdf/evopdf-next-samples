using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- PdfRedactor.Redact [pdf file] [text to redact]
    public static class PdfRedactor_Redact
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            byte[] inputPdf = args.Length > 0 ? File.ReadAllBytes(args[0]) : CreateLetter();
            string text = args.Length > 1 ? args[1] : "Jane Smith";

            // a redactor removes the content for good, saves once and is then disposed
            using var redactor = new PdfRedactor(inputPdf, null);
            int texts = redactor.RedactText(text, false, true);
            int personalData = redactor.RedactPersonalData(PdfPersonalDataTypes.EmailAddress | PdfPersonalDataTypes.PhoneNumber | PdfPersonalDataTypes.Iban);
            redactor.OverlayText = "REDACTED";
            redactor.RemoveMetadata = true;

            File.WriteAllBytes(SampleFiles.Output("redacted.pdf"), redactor.Save());
            PdfRedactionResult result = redactor.Result!;
            Console.WriteLine($"output/redacted.pdf written: {texts} texts and {personalData} personal data items marked, {result.RemovedCharacters} characters removed");
        }

        // a short letter with personal data, created from code
        private static byte[] CreateLetter()
        {
            using var document = new PdfDocument();
            PdfFont font = PdfFontManager.CreateStandardFont(PdfStandardFont.Helvetica, 12f);
            string[] lines =
            {
                "Customer: Jane Smith",
                "Email: jane.smith@example.com",
                "Phone: +40 721 123 456",
                "IBAN: RO49 AAAA 1B31 0075 9384 0000",
                "Dear Jane Smith, thank you for your order."
            };
            for (int i = 0; i < lines.Length; i++)
                document.AddText(new PdfTextElement(lines[i], font) { X = 36, Y = 40 + i * 24 });
            return document.Save();
        }
    }
}
