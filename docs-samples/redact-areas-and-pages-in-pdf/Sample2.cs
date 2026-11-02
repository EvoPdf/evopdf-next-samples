// The second page loses all its content and receives its own overlay text
PdfRedactionArea pageArea = pdfRedactor.RedactPage(2);
pageArea.OverlayText = "PAGE REMOVED";
pageArea.OverlayFontSize = 24;
