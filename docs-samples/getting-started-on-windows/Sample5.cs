// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/getting-started-on-windows.htm
// Documentation page: Getting Started with EvoPdf Next for .NET on Windows

// create the converter object where you want to perform the conversion
HtmlToPdfConverter converter = new HtmlToPdfConverter();

// convert a URL to a memory buffer
string htmlPageURL = "http://www.evopdf.com";
byte[] urlToPdfBuffer = converter.ConvertUrl(htmlPageURL);

FileResult fileResult = new FileContentResult(urlToPdfBuffer, "application/pdf");
fileResult.FileDownloadName = "UrlToPdf.pdf";
return fileResult;
