using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- HtmlToPdf.ElectronicInvoice [invoice.xml] [invoice.html]
    public static class HtmlToPdf_ElectronicInvoice
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string xmlFile = args.Length > 0 ? args[0] : SampleFiles.Input("Invoice_EN_16931.xml");
            string htmlFile = args.Length > 1 ? args[1] : SampleFiles.Input("Electronic_Invoice.html");

            // The profile, the document type and the invoice number are read from the Factur-X / ZUGFeRD XML
            ElectronicInvoice invoice = ElectronicInvoice.FromFile(xmlFile);

            var converter = new HtmlToPdfConverter();
            // An electronic invoice is a PDF/A-3 document; PdfUa1PdfA3a also makes the invoice page accessible
            converter.PdfDocumentOptions.PdfStandard = PdfStandard.PdfUa1PdfA3a;
            converter.PdfDocumentInfo.Title = "Invoice " + invoice.InvoiceNumber;
            converter.PdfDocumentInfo.Language = "en-US";
            converter.PdfDocumentOptions.AddElectronicInvoice(invoice);
            // Open the Attachments panel, where the invoice XML is listed
            converter.PdfViewerPreferences.PageMode = ViewerPageMode.UseAttachments;

            string baseUrl = new Uri(Path.GetDirectoryName(Path.GetFullPath(htmlFile))! + Path.DirectorySeparatorChar).AbsoluteUri;
            byte[] pdf = converter.ConvertHtml(File.ReadAllText(htmlFile), baseUrl);

            File.WriteAllBytes(SampleFiles.Output("electronic-invoice.pdf"), pdf);
            Console.WriteLine($"output/electronic-invoice.pdf written ({invoice.Profile} {invoice.DocumentType} {invoice.InvoiceNumber}, embedded as {invoice.FileName})");
        }
    }
}
