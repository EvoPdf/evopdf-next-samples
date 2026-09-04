// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/convert-markdown-to-pdf.htm
// Documentation page: Convert Markdown to PDF

byte[] inputMarkdownBytes = System.IO.File.ReadAllBytes(markdownFilePath);
string markdownString = Encoding.UTF8.GetString(inputMarkdownBytes);
string baseUrl = "file://" + markdownFilePath;
