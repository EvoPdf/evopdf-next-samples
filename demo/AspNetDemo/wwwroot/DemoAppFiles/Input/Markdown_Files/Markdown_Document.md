# EvoPdf Next PDF Library for .NET (Multi-platform)

[![EvoPdf Next PDF Library Logo](https://raw.githubusercontent.com/EvoPdf/evopdf-files/main/next/evopdf-next-pdf-library-logo.png)](https://www.evopdf.com/evopdf-next-dotnet)

[EvoPdf Next for .NET](https://www.evopdf.com/evopdf-next-dotnet) |
[EvoPdf Next HTML to PDF for .NET](https://www.evopdf.com/evopdf-next-html-to-pdf-dotnet) |
[EVO PDF Software](https://www.evopdf.com) |
[Free Trial](https://www.evopdf.com/download) |
[Licensing](https://www.evopdf.com/buy) |
[Support](https://www.evopdf.com/contact)

## Overview

EvoPdf Next for .NET can be integrated into your applications to create, edit and merge PDF documents, convert HTML to PDF or images, convert Word, Excel, RTF and Markdown to PDF, extract text and images from PDFs, search text in PDFs and convert PDF pages to images.

The library targets .NET Standard 2.0 and can be used in .NET Core and .NET Framework applications deployed on Windows and Linux platforms, including Azure App Service, Azure Functions and Docker.

## Package structure

EvoPdf Next for .NET has a modular architecture, with separate NuGet packages for each major component to prevent unnecessary files from being included in your applications.

This package is a multi-platform metapackage that references both the Windows x64 and Linux x64 EvoPdf Next runtimes. It is ideal when developing on one operating system and deploying to multiple runtime environments.

If you need a single platform package, use 
[EvoPdf.Next.Windows](https://www.nuget.org/packages/EvoPdf.Next.Windows)
or
[EvoPdf.Next.Linux](https://www.nuget.org/packages/EvoPdf.Next.Linux)
.

## Compatibility

The compatibility list of this multi-platform metapackage package includes the following platforms:

* Windows 10, 11 and Windows Server 2016 to 2025
* Linux x64 distributions
* .NET 10.0, 9.0, 8.0, 7.0, 6.0 and .NET Standard 2.0
* .NET Framework 4.6.2 to 4.8.1
* Azure App Service and Azure Functions
* Virtual Machines
* Docker containers
* Web, Console and Desktop applications

## Main Features

* Create, edit and merge PDF documents
* Apply HTML stamps, headers and footers to PDFs
* Generate password-protected and digitally signed PDFs
* Convert HTML with CSS, web fonts and JavaScript to PDF
* Convert HTML to JPEG, PNG and WebP images
* Convert SVG to PDF
* Convert Word DOCX to PDF
* Convert Excel XLSX to PDF
* Convert RTF to PDF
* Convert Markdown to PDF
* Convert PDF to text
* Search text in PDF documents
* Convert PDF pages to images
* Extract images from PDF pages

## Installation

On Windows platforms, the runtime generally does not require the installation of additional dependencies.

On Linux platforms, the HTML to PDF, Word to PDF, Excel to PDF, RTF to PDF and Markdown to PDF converter components may require the installation of additional system dependencies, depending on the Linux distribution and version used.

Detailed instructions for installing Linux dependencies are available in the online documentation, in the Getting Started and Publish guides.

The other components of the EvoPdf Next library generally do not require the installation of additional dependencies.

Install the package using your preferred NuGet package manager.

## Related packages

### Full library metapackages

* **EvoPdf.Next** (Multi-platform)  
  [https://www.nuget.org/packages/EvoPdf.Next](https://www.nuget.org/packages/EvoPdf.Next)

* **EvoPdf.Next.Windows** (Windows x64)  
  [https://www.nuget.org/packages/EvoPdf.Next.Windows](https://www.nuget.org/packages/EvoPdf.Next.Windows)

* **EvoPdf.Next.Linux** (Linux x64)  
  [https://www.nuget.org/packages/EvoPdf.Next.Linux](https://www.nuget.org/packages/EvoPdf.Next.Linux)

### Other EvoPdf.Next component packages

* **Core PDF API** (create, edit, merge and secure PDF documents)  
  [https://www.nuget.org/packages/EvoPdf.Next.Core](https://www.nuget.org/packages/EvoPdf.Next.Core)

* **HTML to PDF**  
  [https://www.nuget.org/packages/EvoPdf.Next.HtmlToPdf](https://www.nuget.org/packages/EvoPdf.Next.HtmlToPdf)

* **Word to PDF**  
  [https://www.nuget.org/packages/EvoPdf.Next.WordToPdf](https://www.nuget.org/packages/EvoPdf.Next.WordToPdf)

* **Excel to PDF**  
  [https://www.nuget.org/packages/EvoPdf.Next.ExcelToPdf](https://www.nuget.org/packages/EvoPdf.Next.ExcelToPdf)

* **RTF to PDF**  
  [https://www.nuget.org/packages/EvoPdf.Next.RtfToPdf](https://www.nuget.org/packages/EvoPdf.Next.RtfToPdf)

* **Markdown to PDF**  
  [https://www.nuget.org/packages/EvoPdf.Next.MarkdownToPdf](https://www.nuget.org/packages/EvoPdf.Next.MarkdownToPdf)

* **PDF Processor** (PDF to text, PDF to images, extract images)  
  [https://www.nuget.org/packages/EvoPdf.Next.PdfProcessor](https://www.nuget.org/packages/EvoPdf.Next.PdfProcessor)

## EvoPdf.Next namespace

All components of the EvoPdf Next for .NET library share the same namespace **EvoPdf.Next** and can be used together in the same application.

To use the library in your own code, add the following using directive at the top of your C# source file:

```csharp
using EvoPdf.Next;
```

## Getting Started

For documentation and code samples, please visit:
[https://www.evopdf.com/evopdf-next-dotnet](https://www.evopdf.com/evopdf-next-dotnet)

You can copy the C# code lines from the section below to create a PDF document from a web page or from an HTML string and save the resulting PDF to a memory buffer for further processing, to a PDF file or send it to the browser for download in ASP.NET applications.

### C# Code Samples

To convert a HTML string or an URL to a PDF file you can use the C# code below.

```csharp
// create the converter object in your code where you want to run conversion
HtmlToPdfConverter converter = new HtmlToPdfConverter();

// convert the HTML string to a PDF file
converter.ConvertHtmlToFile("<b>Hello World</b> from EVO PDF !", null, "HtmlToFile.pdf");

// convert HTML page from URL to a PDF file
string htmlPageURL = "https://www.evopdf.com";
converter.ConvertUrlToFile(htmlPageURL, "UrlToFile.pdf");
```

To convert a HTML string or an URL to a PDF document in a memory buffer and then save it to a file you can use the C# code below.

```csharp
// create the converter object in your code where you want to run conversion
HtmlToPdfConverter converter = new HtmlToPdfConverter();

// convert a HTML string to a memory buffer
byte[] htmlToPdfBuffer = converter.ConvertHtml("<b>Hello World</b> from EVO PDF !", null);

// write the memory buffer to a PDF file
System.IO.File.WriteAllBytes("HtmlToMemory.pdf", htmlToPdfBuffer);

// convert an URL to a memory buffer
string htmlPageURL = "https://www.evopdf.com";
byte[] urlToPdfBuffer = converter.ConvertUrl(htmlPageURL);

// write the memory buffer to a PDF file
System.IO.File.WriteAllBytes("UrlToMemory.pdf", urlToPdfBuffer);
```

To convert in your ASP.NET Core applications a HTML string or an URL to a PDF document in a memory buffer and then send it for download to browser you can use the C# code below.

```csharp
// create the converter object in your code where you want to run conversion
HtmlToPdfConverter converter = new HtmlToPdfConverter();

// convert a HTML string to a memory buffer
byte[] htmlToPdfBuffer = converter.ConvertHtml("<b>Hello World</b> from EVO PDF !", null);

FileResult fileResult = new FileContentResult(htmlToPdfBuffer, "application/pdf");
fileResult.FileDownloadName = "HtmlToPdf.pdf";
return fileResult;
```

## Free Trial

You can download the EvoPdf Next for .NET evaluation package from [EVO PDF Downloads](https://www.evopdf.com/download) page of the website.

The evaluation package contains a demo ASP.NET application with full C# code for all features of the library.

You can evaluate the library for free as long as it is needed to ensure that the solution fits your application needs.

## Licensing

The EVO PDF Software licenses are perpetual which means they never expire for a version of the product and include free maintenance for the first year. You can find [more details about licensing](https://www.evopdf.com/buy) on the website.

## Support

For technical and sales questions or for general inquiries about our software and company you can contact us using the email addresses from the [contact page](https://www.evopdf.com/contact) of the website. 
