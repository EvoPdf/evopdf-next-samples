PdfLinkAnnotation link = PdfLinkAnnotation.FromUrl("https://www.example.com", 1, 30, 100, 200, 20);
link.BorderStyle = PdfLinkBorderStyle.Solid;
link.HighlightMode = PdfLinkHighlightMode.Outline;
pdfDocument.AddLinkAnnotation(link);
