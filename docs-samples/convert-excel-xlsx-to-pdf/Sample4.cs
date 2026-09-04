// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/convert-excel-xlsx-to-pdf.htm
// Documentation page: Convert Excel XLSX to PDF

byte[] outPdfBuffer = await excelToPdfConverter.ConvertToPdfAsync(excelBytes);
