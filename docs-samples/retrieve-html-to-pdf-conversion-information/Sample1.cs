byte[] outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);

HtmlToPdfConversionInfo conversionInfo = htmlToPdfConverter.ConversionInfo;
Console.WriteLine("Pages from HTML: {0}", conversionInfo.PagesFromHtml);
Console.WriteLine("Pages inserted before: {0}, after: {1}", conversionInfo.TotalPagesInsertedBefore, conversionInfo.ExternalPagesInsertedAfter);
Console.WriteLine("Total pages: {0}, zoom: {1}%", conversionInfo.TotalPages, conversionInfo.PrintZoom);
