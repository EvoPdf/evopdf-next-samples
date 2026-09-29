using EvoPdf;

HtmlToPdfConverter converter = new HtmlToPdfConverter();
converter.LicenseKey = "...";

converter.HtmlViewerWidth = 1024;
converter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
converter.PdfDocumentOptions.FitWidth = true;

byte[] pdf = converter.ConvertUrl(url);
