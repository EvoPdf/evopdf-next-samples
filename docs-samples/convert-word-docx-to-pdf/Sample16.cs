byte[] outPdfBuffer = wordToPdfConverter.ConvertToPdf(inputBytes);

Console.WriteLine("Pages from the document: {0}", wordToPdfConverter.ConversionInfo.PagesFromDocument);
Console.WriteLine("Total pages: {0}", wordToPdfConverter.ConversionInfo.TotalPages);
