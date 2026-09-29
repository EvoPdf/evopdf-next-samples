# Select Conversion Triggering Mode

The HTML conversion to PDF happens in two main stages. In the first stage the HTML document is loaded in the internal HTML viewer. When the load is complete the second stage starts and the HTML content is rendered to PDF document or into an image. The moment when the HTML load is complete is not well defined for all HTML documents. There might be for example scripts running in page which continuously update the HTML document. The triggering modes help the converter to decide when the HTML page load should be considered completed and when the actual rendering to PDF can start. The triggering mode is given by the `HtmlToPdfConverter.TriggeringMode` property. There are two possible triggering modes:

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/select-conversion-triggering-mode.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | Select Conversion Triggering Mode | 4 |
| `Sample2.cs` | Select Conversion Triggering Mode | 4 |
| `Sample3.cs` | Select Conversion Triggering Mode | 4 |
| `Sample4.cs` | Code Sample - Select Conversion Triggering Mode | 113 |
| `Sample5.html` | HTML Code with Manual Triggering | 53 |
