<p align="center">
  <a href="https://www.evopdf.com/evopdf-next-dotnet"><img src="https://raw.githubusercontent.com/EvoPdf/evopdf-files/main/next/evopdf-next-pdf-library-logo.png" alt="EvoPdf Next" height="72"></a>
</p>

<h1 align="center">EvoPdf Next — Samples</h1>

<p align="center">Runnable C# samples and the complete demo application for <b>EvoPdf Next</b>, the .NET PDF library: HTML, Word, Excel, RTF and Markdown to PDF · PDF to text, search, images · PDF creation and editing · PDF/UA and PDF/A.</p>

<p align="center">
  <a href="https://www.nuget.org/packages/EvoPdf.Next"><img src="https://img.shields.io/nuget/v/EvoPdf.Next?label=EvoPdf.Next&logo=nuget" alt="NuGet"></a>
  <a href="https://github.com/EvoPdf/evopdf-next-samples/actions/workflows/build.yml"><img src="https://github.com/EvoPdf/evopdf-next-samples/actions/workflows/build.yml/badge.svg" alt="build"></a>
  <a href="https://www.evopdf.com/help/evopdf-next-dotnet/"><img src="https://img.shields.io/badge/docs-evopdf.com-1E6FB8" alt="Documentation"></a>
  <img src="https://img.shields.io/badge/Windows%20%7C%20Linux%20%7C%20macOS-x64%20%7C%20ARM64-555" alt="Platforms">
</p>

---

## Three kinds of samples

| Folder | What it is | How to use it |
|---|---|---|
| [`quickstarts/`](quickstarts) | 16 minimal samples, one feature each, ~20 lines, shared by one .NET 10 console project per platform | `dotnet run --project Quickstarts_<Platform>.csproj -- <SampleName>` — the fastest way to see a feature work |
| [`docs-samples/`](docs-samples) | The 142 code samples from 67 documentation pages, verbatim, each linked to its page | Copy into your project; read the page for the explanation |
| [`demo/`](demo) | The demo applications from the official download package: the ASP.NET Core demo that runs at [evopdf.com](https://www.evopdf.com/evopdf-next-aspnet-demo/) with the source of every demo page, plus the console demo — one source tree each, a project per platform | open the platform solution and run |

All samples use the current API (`EvoPdf.Next` namespace, version 14.36) and read the license key from the `EVOPDF_LICENSE_KEY` environment variable; without it they run in demo mode (watermarked output).

## Quickstarts

```bash
git clone https://github.com/EvoPdf/evopdf-next-samples
cd evopdf-next-samples/quickstarts
dotnet run --project Quickstarts_Windows.csproj -- HtmlToPdf.Basic https://www.evopdf.com   # or _Linux, _MacOS, _Windows.Arm64, …
dotnet run --project Quickstarts_Windows.csproj                                             # lists the sample names
```
The samples (`quickstarts/Samples/<Name>.cs`) are shared by seven platform projects in the same folder (`Quickstarts_<Platform>.csproj`), one per NuGet package, each with its own `obj`/`bin` — the same layout as the demo applications. One project per platform keeps the build small: the package copies the native rendering runtimes into the output folder of every project that references it. Each sample file is self-contained — copy `Run` into your own project.

Every sample runs without arguments: the input documents (`Files/` — the Word, Excel, RTF, Markdown and PDF documents of the demo application) are copied next to the executable, and results are written to an `output` folder there. Pass your own file or URL as the argument to convert something else: `Quickstarts.exe WordToPdf C:\docs\report.docx`.

| Project | Shows |
|---|---|
| [`HtmlToPdf.Basic`](quickstarts/Samples/HtmlToPdf.Basic.cs) | Convert a URL and an HTML string to PDF |
| [`HtmlToPdf.PageSetup`](quickstarts/Samples/HtmlToPdf.PageSetup.cs) | Fixed A4 page, margins, orientation, viewer width |
| [`HtmlToPdf.HeadersFooters`](quickstarts/Samples/HtmlToPdf.HeadersFooters.cs) | HTML header and footer with page numbers |
| [`HtmlToPdf.DynamicContent`](quickstarts/Samples/HtmlToPdf.DynamicContent.cs) | Wait for JavaScript content: conversion delay and manual triggering |
| [`HtmlToPdf.Standards`](quickstarts/Samples/HtmlToPdf.Standards.cs) | PDF/UA and PDF/A output |
| [`HtmlToPdf.Security`](quickstarts/Samples/HtmlToPdf.Security.cs) | Passwords, permissions and document metadata |
| [`HtmlToImage`](quickstarts/Samples/HtmlToImage.cs) | HTML page to PNG screenshot |
| [`WordToPdf`](quickstarts/Samples/WordToPdf.cs) | Word DOCX to PDF |
| [`ExcelToPdf`](quickstarts/Samples/ExcelToPdf.cs) | Excel XLSX to PDF |
| [`RtfToPdf`](quickstarts/Samples/RtfToPdf.cs) | RTF to PDF |
| [`MarkdownToPdf`](quickstarts/Samples/MarkdownToPdf.cs) | Markdown to PDF |
| [`PdfProcessor.PdfToText`](quickstarts/Samples/PdfProcessor.PdfToText.cs) | Extract text from a PDF, original layout or reading order |
| [`PdfProcessor.FindText`](quickstarts/Samples/PdfProcessor.FindText.cs) | Search text in a PDF and print the position of every match |
| [`PdfProcessor.PdfToImage`](quickstarts/Samples/PdfProcessor.PdfToImage.cs) | Render PDF pages to PNG images |
| [`PdfProcessor.ExtractImages`](quickstarts/Samples/PdfProcessor.ExtractImages.cs) | Extract the images embedded in a PDF |
| [`PdfEditor.Stamp`](quickstarts/Samples/PdfEditor.Stamp.cs) | Stamp an existing PDF with an HTML template |

## Running the demo applications

- **ASP.NET Core demo**: run it from Visual Studio (open the platform solution, set `EvoPdf_Next_AspNetDemo_<Platform>` as startup project, F5) or from the CLI in its project folder (`dotnet run --project EvoPdf_Next_AspNetDemo_<Platform>.csproj`), or publish it (`dotnet publish -c Release -o publish`) and run from the `publish` folder. Starting the executable from `bin` does not work: a build does not copy `wwwroot` there, so styles, images and the demo input files are not found. Details in [`demo/README.md`](demo/README.md).
- **Console demo**: runs straight from the build output — `EvoPdf_Next_ConsoleDemo_<Platform> https://www.evopdf.com output.pdf`.
- **Quickstarts**: `Quickstarts.exe <SampleName> [input]` from the build output (`quickstarts/bin/<Platform>/…`), or `dotnet run --project Quickstarts_<Platform>.csproj -- <SampleName>`; the input documents ship with the build.

## Solutions, one per platform

| Solution | Package | Runs on |
|---|---|---|
| `EvoPdf.Next.Samples.Windows.sln` | `EvoPdf.Next.Windows` | Windows x64 |
| `EvoPdf.Next.Samples.Windows.Arm64.sln` | `EvoPdf.Next.Windows.Arm64` | Windows ARM64 |
| `EvoPdf.Next.Samples.Linux.sln` | `EvoPdf.Next.Linux` | Linux x64 |
| `EvoPdf.Next.Samples.Linux.Arm64.sln` | `EvoPdf.Next.Linux.Arm64` | Linux ARM64 |
| `EvoPdf.Next.Samples.MacOS.sln` | `EvoPdf.Next.MacOS` | macOS (Apple Silicon) |
| `EvoPdf.Next.Samples.MultiPlatform.sln` | `EvoPdf.Next` | Windows x64 + Linux x64 |
| `EvoPdf.Next.Samples.MultiPlatform.Arm64.sln` | `EvoPdf.Next.Windows.Arm64` + `EvoPdf.Next.Linux.Arm64` | Windows ARM64 + Linux ARM64 |

Each solution holds the quickstarts, the ASP.NET Core demo and the console demo of its platform. The CI builds all seven on Windows, Linux and macOS runners. On Linux install the [system packages](https://www.evopdf.com/help/evopdf-next-dotnet/html/getting-started-on-linux.htm) first.

## Documentation samples

| Documentation page | Samples |
|---|---|
| [Access a HTML Page Using GET and POST HTTP Methods](docs-samples/access-html-pages-with-get-and-post) | 1 |
| [Add Attachments to Generated PDF](docs-samples/add-attachments-to-generated-pdf) | 3 |
| [Add Cookies to HTML Page Request](docs-samples/add-cookies-to-html-page-request) | 1 |
| [Add File Attachments to Existing PDF](docs-samples/add-file-attachments-to-existing-pdf) | 3 |
| [Add HTML Stamp with Page Numbering to Existing PDF](docs-samples/add-html-stamp-to-existing-pdf) | 3 |
| [Add HTML Stamp with Page Numbering to Generated PDF](docs-samples/add-html-stamp-to-generated-pdf) | 1 |
| [Add HTTP Headers to HTML Page Request](docs-samples/add-http-headers-to-html-page-request) | 1 |
| [Add Images to Existing PDF](docs-samples/add-images-to-existing-pdf) | 1 |
| [Add Link Annotations to Existing PDF](docs-samples/add-link-annotations-to-existing-pdf) | 2 |
| [Add Polylines, Polygons and Paths to Existing PDF](docs-samples/add-paths-and-polygons-to-existing-pdf) | 1 |
| [Add Shapes to Existing PDF](docs-samples/add-shapes-to-existing-pdf) | 1 |
| [Add Text Annotations to Existing PDF](docs-samples/add-text-annotations-to-existing-pdf) | 1 |
| [Add Text to Existing PDF](docs-samples/add-text-to-existing-pdf) | 3 |
| [Auto Create Hierarchical Bookmarks](docs-samples/auto-create-hierarchical-bookmarks) | 1 |
| [Auto Create Table of Contents](docs-samples/auto-create-table-of-contents) | 2 |
| [Avoid Page Breaks Inside HTML Elements Using CSS](docs-samples/avoid-page-break-inside-elements-uising-css) | 1 |
| [Convert the Current HTML Page to PDF](docs-samples/convert-current-html-page-to-pdf) | 1 |
| [Convert Excel XLSX to PDF](docs-samples/convert-excel-xlsx-to-pdf) | 6 |
| [Convert a HTML Page to PDF in Same Session](docs-samples/convert-html-page-to-pdf-in-same-session) | 1 |
| [Convert HTML Pages with Authentication](docs-samples/convert-html-pages-with-authentication) | 2 |
| [Convert HTML with SVG to PDF](docs-samples/convert-html-with-svg-to-pdf) | 1 |
| [Convert HTML with Web Fonts to PDF](docs-samples/convert-html-with-web-fonts-to-pdf) | 1 |
| [Convert Internal Links from HTML to PDF](docs-samples/convert-internal-links-from-html-to-pdf) | 1 |
| [Convert Markdown to PDF](docs-samples/convert-markdown-to-pdf) | 5 |
| [Convert Multiple HTML Pages to PDF in Parallel](docs-samples/convert-multiple-html-to-pdf-in-parallel) | 1 |
| [Convert PDF Pages to Images](docs-samples/convert-pdf-pages-to-images) | 2 |
| [Convert PDF to Text](docs-samples/convert-pdf-to-text) | 2 |
| [Convert RTF to PDF](docs-samples/convert-rtf-to-pdf) | 10 |
| [Convert Word DOCX to PDF](docs-samples/convert-word-docx-to-pdf) | 6 |
| [Create PDF Documents with File Attachments](docs-samples/create-pdf-documents-with-file-attachments) | 3 |
| [Create PDF Documents with Images](docs-samples/create-pdf-documents-with-images) | 3 |
| [Create PDF Documents with Link Annotations](docs-samples/create-pdf-documents-with-link-annotations) | 2 |
| [Create PDF Documents with Polylines, Polygons and Paths](docs-samples/create-pdf-documents-with-paths-and-polygons) | 1 |
| [Create PDF Documents with Shapes](docs-samples/create-pdf-documents-with-shapes) | 1 |
| [Create PDF/UA and PDF/A Documents](docs-samples/create-pdf-documents-with-standards) | 2 |
| [Create PDF Documents with Text Annotations](docs-samples/create-pdf-documents-with-text-annotations) | 1 |
| [Create PDF Documents with Text](docs-samples/create-pdf-documents-with-text) | 5 |
| [Create PDF Documents](docs-samples/create-pdf-documents) | 8 |
| [Create PDF Forms from HTML Forms](docs-samples/create-pdf-forms-from-html-forms) | 1 |
| [Create PDF/UA and PDF/A Compliant Documents](docs-samples/create-pdfua-and-pdfa-documents) | 1 |
| [Add a Digital Signature to Generated PDF Document](docs-samples/digitally-sign-the-generated-pdf) | 1 |
| [EvoPdf Next for .NET Overview](docs-samples/evopdf-next-overview) | 3 |
| [EvoPdf Next for .NET Overview](docs-samples/evopdf-next-user-guide) | 3 |
| [Extract Images from PDF](docs-samples/extract-images-from-pdf) | 2 |
| [Getting Started with EvoPdf Next for .NET on Linux](docs-samples/getting-started-on-linux) | 5 |
| [Getting Started with EvoPdf Next for .NET on macOS](docs-samples/getting-started-on-macos) | 5 |
| [Getting Started with EvoPdf Next for .NET on Windows](docs-samples/getting-started-on-windows) | 5 |
| [Add Header and Footer to PDF from Multiple HTML](docs-samples/header-and-footer-on-pdf-from-multiple-html) | 1 |
| [Add HTML in Header and Footer Using Browser Mode](docs-samples/html-header-and-footer-in-browser-mode) | 1 |
| [Add HTML in Header and Footer with Page Numbers](docs-samples/html-header-and-footer-with-page-numbers) | 1 |
| [HTML to Image Converter Overview](docs-samples/html-to-image-converter-overview) | 1 |
| [HTML To PDF Converter Options](docs-samples/html-to-pdf-converter-options) | 1 |
| [HTML to PDF Converter Overview](docs-samples/html-to-pdf-converter-overview) | 1 |
| [Insert Page Breaks in PDF Using CSS in HTML](docs-samples/insert-page-breaks-in-pdf-using-css) | 1 |
| [Licensing for EvoPdf Next](docs-samples/licensing-for-evopdf-next) | 1 |
| [Merge Multiple HTML to PDF](docs-samples/merge-multiple-html-to-pdf) | 1 |
| [Repeat HTML Table Header and Footer in PDF Pages](docs-samples/repeat-html-table-header-and-footer-in-pdf) | 1 |
| [Retrieve HTML Element Positions in PDF](docs-samples/retrieve-html-element-positions-in-pdf) | 1 |
| [Search for Text in PDF](docs-samples/search-for-text-in-pdf) | 2 |
| [Select Conversion Triggering Mode](docs-samples/select-conversion-triggering-mode) | 5 |
| [Select HTML Elements to Convert to Image](docs-samples/select-html-elements-to-convert-to-image) | 1 |
| [Select HTML Elements to Convert to PDF](docs-samples/select-html-elements-to-convert-to-pdf) | 1 |
| [Select HTML Elements to Exclude from Image](docs-samples/select-html-elements-to-exclude-from-image) | 1 |
| [Select HTML Elements to Exclude from PDF](docs-samples/select-html-elements-to-exclude-from-pdf) | 1 |
| [Select Media Type for Screen or Print](docs-samples/select-media-type-for-screen-or-print) | 1 |
| [Set Permissions and Password of the Generated PDF Document](docs-samples/set-pdf-permissions-and-password) | 1 |
| [Set PDF Viewer Preferences for the Generated PDF Document](docs-samples/set-pdf-viewer-preferences) | 1 |

## Writing EvoPdf Next code with an AI assistant

The companion repository [**evopdf-agent-skills**](https://github.com/EvoPdf/evopdf-agent-skills) packages the API rules and these scenarios as skills for Claude Code, GitHub Copilot, Cursor, Codex and other assistants — including a Classic → Next migration skill. Install it and point your assistant at this repository for runnable code.

## Related
- [EvoPdf Next documentation](https://www.evopdf.com/help/evopdf-next-dotnet/) · [All components](https://www.evopdf.com/evopdf-next-dotnet) · [NuGet packages](https://www.nuget.org/profiles/EvoPdf)
- [Download the full demo package](https://www.evopdf.com/download) (Windows, Linux and macOS variants, .NET 8 and .NET 10)
- [Pricing & licensing](https://www.evopdf.com/buy) · [Support](https://www.evopdf.com/support)

## License
The samples are MIT licensed. EvoPdf Next is commercial software with a free, time-unlimited evaluation. The demo application contains a public evaluation key intended only to run the demo without watermarks.
