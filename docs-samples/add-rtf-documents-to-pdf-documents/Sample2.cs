rtfElement.X = 0;
rtfElement.Y = crtYPos;
// 0 extends the content to the right margin of the page
rtfElement.Width = 0;
rtfElement.Zoom = 100;
// The headings of the RTF document join the outline of the PDF document
rtfElement.GenerateDocumentOutline = true;

PdfContentRenderInfo rtfRenderInfo = pdfDocument.AddRtf(rtfElement);
