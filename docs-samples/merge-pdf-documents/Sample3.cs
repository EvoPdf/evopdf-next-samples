pdfMerge.PdfDocumentInfo.Title = "Merged Document";
pdfMerge.OpenAction = new PdfGoToPageAction(secondDocumentPage, PdfLinkPageLocation.FitPage());

PdfHtmlTemplate footer = pdfMerge.AddHtmlTemplate(0, 0, 300, 30,
    "<div style=\"font-family: Arial; font-size: 10px;\">Page {page_number} of {total_pages}</div>", null);
footer.HorizontalAlign = PdfTemplateHorizontalAlign.Center;
footer.VerticalAlign = PdfTemplateVerticalAlign.Bottom;
