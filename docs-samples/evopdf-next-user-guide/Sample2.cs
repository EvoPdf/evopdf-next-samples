// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/evopdf-next-user-guide.htm
// Documentation page: EvoPdf Next for .NET Overview

// create the converter object in your code where you want to run conversion
HtmlToPdfConverter converter = new HtmlToPdfConverter();

// convert an HTML string to a memory buffer
byte[] htmlToPdfBuffer = converter.ConvertHtml("<b>Hello World</b> from EVO PDF !", null);

// write the data from the memory buffer to a PDF file
System.IO.File.WriteAllBytes("HtmlToMemory.pdf", htmlToPdfBuffer);
