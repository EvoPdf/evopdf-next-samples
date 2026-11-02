byte[] outPdfBuffer = rtfToPdfConverter.ConvertToPdf(inputBytes);

Console.WriteLine("Pages from the document: {0}", rtfToPdfConverter.ConversionInfo.PagesFromDocument);
Console.WriteLine("Total pages: {0}", rtfToPdfConverter.ConversionInfo.TotalPages);
