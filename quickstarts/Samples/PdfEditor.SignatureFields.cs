using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- PdfEditor.SignatureFields [page.html]
    public static class PdfEditor_SignatureFields
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string htmlFile = args.Length > 0 ? args[0] : SampleFiles.Input("Signature_Fields.html");

            // 1. Convert the HTML page and read where the elements marking the signatures were rendered
            var converter = new HtmlToPdfConverter();
            converter.HtmlElementsInfoSelector = ".signature-field";
            byte[] pdf = converter.ConvertHtml(File.ReadAllText(htmlFile), null);

            // 2. Add an empty signature field where each element was rendered, named by the element id
            var fieldNames = new List<string>();
            using (var editor = new PdfEditor(pdf))
            {
                foreach (HtmlElementInfo element in converter.HtmlElementsInfo.Elements)
                {
                    string name = string.IsNullOrEmpty(element.Id) ? "Signature" + (fieldNames.Count + 1) : element.Id;
                    PdfRenderedRectangle rectangle = element.RenderedRectangles[0];
                    editor.AddSignatureField(new PdfSignatureField(name, rectangle.PageNumber, rectangle.Bounds));
                    fieldNames.Add(name);
                }
                pdf = editor.Save();
            }
            File.WriteAllBytes(SampleFiles.Output("signature-fields.pdf"), pdf);

            // 3. The first signer fills in the first field; the other fields wait for their signers
            var signature = new PdfDigitalSignature(SampleFiles.Input("evopdf.pfx"), "evopdf");
            signature.FieldName = fieldNames[0];
            signature.Reason = "Approved";
            using (var editor = new PdfEditor(pdf, null, signature))
            {
                signature.AppearanceEnabled = true;
                signature.Appearance.Text = "Approved by EVO PDF Software";
                pdf = editor.Save();
            }
            File.WriteAllBytes(SampleFiles.Output("signature-fields-signed.pdf"), pdf);

            Console.WriteLine($"output/signature-fields.pdf and output/signature-fields-signed.pdf written (fields: {string.Join(", ", fieldNames)})");
        }
    }
}
