// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/convert-rtf-to-pdf.htm
// Documentation page: Convert RTF to PDF

byte[] outPdfBuffer = await rtfToPdfConverter.ConvertStringToPdfAsync(rtfString);
