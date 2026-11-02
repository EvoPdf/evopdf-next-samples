using EvoPdf;

HtmlToPdfConverter converter = new HtmlToPdfConverter();
converter.LicenseKey = "...";
converter.NextConverterCreated += next =>
    next.PdfDocumentOptions.PdfStandard = EvoPdf.Next.PdfStandard.PdfUa1;

byte[] pdf = converter.ConvertUrl(url);
