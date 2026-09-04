// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/auto-create-table-of-contents.htm
// Documentation page: Auto Create Table of Contents

// Create a HTML to PDF converter object with default settings
HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

// Enable or disable the automatic creation of a table of contents in the PDF document based on H1 to H6 HTML tags
htmlToPdfConverter.PdfDocumentOptions.GenerateTableOfContents = true;
