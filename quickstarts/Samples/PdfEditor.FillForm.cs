using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- PdfEditor.FillForm [flatten]
    public static class PdfEditor_FillForm
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            // 1. A PDF form created from code: a text field and a check box on the first page (positions in points)
            byte[] formPdf;
            using (var document = new PdfDocument())
            {
                PdfFont font = PdfFontManager.CreateStandardFont(PdfStandardFont.Helvetica, 12f);
                document.AddText(new PdfTextElement("Name:", font) { X = 36, Y = 40 });
                document.AddText(new PdfTextElement("Subscribe to the newsletter", font) { X = 60, Y = 80 });
                document.AddFormField(new PdfTextField("fullName", 1, 100, 36, 200, 20) { BorderColor = PdfColor.Gray });
                document.AddFormField(new PdfCheckBoxField("newsletter", 1, 36, 78, 14, 14));
                formPdf = document.Save();
            }
            File.WriteAllBytes(SampleFiles.Output("form.pdf"), formPdf);

            // 2. The form filled in an existing document; with "flatten" the fields become static content
            using var editor = new PdfEditor(formPdf, null);
            foreach (PdfFormFieldInfo field in editor.Form.Fields)
                Console.WriteLine($"field {field.Name} ({field.Type})");
            editor.Form.SetValue("fullName", "John Smith");
            editor.Form.SetChecked("newsletter", true);
            editor.Flatten = args.Length > 0 && args[0] == "flatten";

            File.WriteAllBytes(SampleFiles.Output("form-filled.pdf"), editor.Save());
            Console.WriteLine("output/form.pdf and output/form-filled.pdf written");
        }
    }
}
