markdownElement.X = 0;
markdownElement.Y = crtYPos;
// 0 extends the content to the right margin of the page
markdownElement.Width = 0;
markdownElement.Zoom = 100;
// The CSS applied to the rendered Markdown content
markdownElement.StyleSheet = styleSheet;
// The headings of the Markdown content join the outline of the PDF document
markdownElement.GenerateDocumentOutline = true;

PdfContentRenderInfo markdownRenderInfo = pdfDocument.AddMarkdown(markdownElement);
