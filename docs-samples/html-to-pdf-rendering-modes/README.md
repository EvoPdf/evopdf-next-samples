# HTML to PDF Rendering Modes and the Persistent Rendering Engine

The HTML to PDF and HTML to Image converters render the HTML pages in a separate process, the HTML rendering engine, which is a headless Chromium browser shipped with the library. The library manages this process for the application and offers two ways of doing it, called rendering modes. The mode is a global setting of the library and applies to every HTML conversion of the application, including the conversions of the HTML templates used for headers and footers.

Source topic: https://www.evopdf.com/help/evopdf-next-dotnet/html/html-to-pdf-rendering-modes.htm

| File | Section | Lines |
| --- | --- | --- |
| `Sample1.cs` | The Two Rendering Modes | 5 |
| `Sample2.cs` | Monitoring the Persistent Process | 7 |
| `Sample3.cs` | Using the Persistent Process in ASP.NET Core | 4 |
