// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/getting-started-on-macos.htm
// Documentation page: Getting Started with EvoPdf Next for .NET on macOS

// create the converter object where you want to perform the conversion
HtmlToPdfConverter converter = new HtmlToPdfConverter();

// convert an HTML string to a memory buffer
byte[] htmlToPdfBuffer = converter.ConvertHtml("<b>Hello World</b> from EVO PDF !", null);

// write the memory buffer to a PDF file
System.IO.File.WriteAllBytes("HtmlToMemory.pdf", htmlToPdfBuffer);
