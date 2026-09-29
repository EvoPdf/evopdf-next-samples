using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- HtmlToPdf.PageSetup [url] [layout]
    // layout: fit (default), page, chrome, receipt or window; receipt converts the receipt HTML below
    public static class HtmlToPdf_PageSetup
    {
        private const string ReceiptHtml =
            "<html><body style='font:12px monospace;margin:0'>" +
            "<div style='text-align:center;font-weight:bold'>CORNER CAFE</div>" +
            "<div style='text-align:center'>Receipt 1042</div><hr>" +
            "<table style='width:100%;font:inherit'>" +
            "<tr><td>Espresso</td><td style='text-align:right'>2.50</td></tr>" +
            "<tr><td>Croissant</td><td style='text-align:right'>3.20</td></tr>" +
            "<tr><td>Orange juice</td><td style='text-align:right'>4.10</td></tr>" +
            "</table><hr>" +
            "<table style='width:100%;font:inherit;font-weight:bold'>" +
            "<tr><td>Total</td><td style='text-align:right'>9.80</td></tr></table>" +
            "<div style='text-align:center;margin-top:12px'>Thank you</div>" +
            "</body></html>";

        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string url = args.Length > 0 ? args[0] : "https://www.evopdf.com";
            string layout = args.Length > 1 ? args[1].ToLowerInvariant() : "fit";

            var converter = new HtmlToPdfConverter();
            var o = converter.PdfDocumentOptions;

            switch (layout)
            {
                case "page":
                    // An HTML template designed for the paper size: laid out at the width of the page, not scaled
                    converter.LayoutAtPageWidth(PdfPageSize.A4, PdfPageOrientation.Portrait, mediaType: "print");
                    o.LeftMargin = o.RightMargin = o.TopMargin = o.BottomMargin = 36;
                    break;

                case "chrome":
                    // The same output as Save as PDF in Chrome
                    converter.PrintLikeChrome(PdfPageSize.Letter);
                    break;

                case "receipt":
                    // One page 227 points wide (80 mm paper) with 14 point margins, as tall as the content.
                    // The HTML is designed for the receipt width, so it fits the page without scaling
                    converter.SinglePageOfWidth(227, 14);
                    break;

                case "window":
                    // The page is as wide as a 1024 pixel browser window and the HTML is drawn 1:1
                    converter.PageWidthFromBrowserWindow();
                    break;

                default:
                    // A4 portrait with the page laid out as in a 1024 pixel browser window, scaled to the page width.
                    // The margins set after the call are taken into account when the layout is computed
                    converter.FitBrowserWindowToPage(PdfPageSize.A4);
                    o.LeftMargin = o.RightMargin = o.TopMargin = o.BottomMargin = 36;
                    break;
            }

            PageLayoutMethod method = converter.LayoutMethod;
            byte[] pdf = layout == "receipt"
                ? converter.ConvertHtml(ReceiptHtml, null)
                : converter.ConvertUrl(url);
            string file = $"page-setup-{layout}.pdf";
            File.WriteAllBytes(SampleFiles.Output(file), pdf);
            Console.WriteLine($"output/{file} written, layout {method}, drawn at {converter.ConversionInfo.PrintZoom:0.#} percent");
        }
    }
}
