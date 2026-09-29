// create the converter object where you want to perform the conversion
HtmlToPdfConverter converter = new HtmlToPdfConverter();

// convert an HTML string to a memory buffer
byte[] htmlToPdfBuffer = converter.ConvertHtml("<b>Hello World</b> from EVO PDF !", null);

FileResult fileResult = new FileContentResult(htmlToPdfBuffer, "application/pdf");
fileResult.FileDownloadName = "HtmlToPdf.pdf";
return fileResult;
