byte[] outPdfBuffer = markdownToPdfConverter.ConvertStringToPdf(markdownString, baseUrl);

Console.WriteLine("Pages from the document: {0}", markdownToPdfConverter.ConversionInfo.PagesFromDocument);
Console.WriteLine("Total pages: {0}", markdownToPdfConverter.ConversionInfo.TotalPages);
