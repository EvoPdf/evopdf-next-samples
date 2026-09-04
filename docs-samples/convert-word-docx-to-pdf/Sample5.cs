// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/convert-word-docx-to-pdf.htm
// Documentation page: Convert Word DOCX to PDF

byte[] outPdfBuffer = await wordToPdfConverter.ConvertToPdfAsync(wordFilePath);
