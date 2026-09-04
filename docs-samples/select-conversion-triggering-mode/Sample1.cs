// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/select-conversion-triggering-mode.htm
// Documentation page: Select Conversion Triggering Mode

// Create the PDF converter
HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();
// Set the triggering mode
htmlToPdfConverter.TriggeringMode = TriggeringMode.Auto;
