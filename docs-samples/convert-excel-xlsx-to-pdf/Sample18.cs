byte[] outPdfBuffer = excelToPdfConverter.ConvertToPdf(inputBytes);

Console.WriteLine("Pages from the document: {0}", excelToPdfConverter.ConversionInfo.PagesFromDocument);
Console.WriteLine("Total pages: {0}", excelToPdfConverter.ConversionInfo.TotalPages);
