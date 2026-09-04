// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/create-pdf-documents-with-standards.htm
// Documentation page: Create PDF/UA and PDF/A Documents

PdfDocumentCreateSettings settings = new PdfDocumentCreateSettings
{
    PageSize = PdfPageSize.A4,
    PageOrientation = PdfPageOrientation.Portrait,
    Margins = new PdfMargins(36, 36, 36, 36),
    PdfStandard = PdfStandard.PdfUa2PdfA4,
    Language = "en-US"
};
using PdfDocument pdfDocument = new PdfDocument(settings);
pdfDocument.PdfDocumentInfo.Title = "Accessibility and Archival Demo";
pdfDocument.PdfViewerPreferences.DisplayDocTitle = true;
