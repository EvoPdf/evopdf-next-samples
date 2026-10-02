# EvoPdf Next demo applications

The .NET 10 demo applications from the official download package (`EvoPdf-Next-v14.84.0.zip`): one source tree each, with a project file per target platform in the same folder (each project keeps its own `obj`/`bin`, so they build side by side).

| Project suffix | Package | Runs on |
|---|---|---|
| `_Windows` / `_Windows.Arm64` | `EvoPdf.Next.Windows` / `.Windows.Arm64` | Windows x64 / ARM64 |
| `_Linux` / `_Linux.Arm64` | `EvoPdf.Next.Linux` / `.Linux.Arm64` | Linux x64 / ARM64 |
| `_MacOS` | `EvoPdf.Next.MacOS` | macOS (Apple Silicon) |
| `_MultiPlatform` / `_MultiPlatform.Arm64` | `EvoPdf.Next` / `.Windows.Arm64` + `.Linux.Arm64` | Windows + Linux from one build |

## AspNetDemo
The ASP.NET Core MVC application that runs at [evopdf.com](https://www.evopdf.com/evopdf-next-aspnet-demo/), with the C# source of every demo page under `Controllers/` (HTML to PDF, HTML to Image, PDF Creator, PDF Editor, Word / Excel / RTF / Markdown to PDF, PDF to Text, Find Text, PDF to Image, Extract PDF Images). Global Settings sets the license key, the HTML rendering mode and the maximum parallel conversions for the whole application; Persistent Renderer Process shows the state of the renderer process and its replacements; HTML to PDF Benchmark converts the same document on several threads and reports the throughput and the latency.

`wwwroot` (styles, images, the demo input files) is not copied to `bin` by a build; ASP.NET Core serves it from the project folder at development time; so run the application in one of these ways, not by starting the executable from `bin`:
- **Visual Studio**: open the platform solution from the repository root (`EvoPdf.Next.Samples.<Platform>.sln`), set `EvoPdf_Next_AspNetDemo_<Platform>` as the startup project, F5.
- **.NET CLI**, from the `AspNetDemo` folder: `dotnet run --project EvoPdf_Next_AspNetDemo_Windows.csproj` (pick your platform).
- **Published**: `dotnet publish EvoPdf_Next_AspNetDemo_Linux.csproj -c Release -o publish` copies `wwwroot` next to the executable; the application then runs from the `publish` folder on any machine, under IIS or in a container.

Started from Visual Studio or with `dotnet run`, the demo opens in the browser at `http://localhost:27100`. The address, and the browser that opens, come from `Properties/launchSettings.json`, a file that Visual Studio and `dotnet run` read during development. The file is not copied to the `publish` folder, so the published application listens on `http://localhost:5000`, the default address of ASP.NET Core; start it with `--urls http://localhost:27100`, or set the `ASPNETCORE_URLS` environment variable, to keep the same address.

## ConsoleDemo
A single-file command-line HTML to PDF converter (`Program.cs`), the smallest complete application built on the library. It has no content files and runs straight from the build output:
```bash
dotnet run --project EvoPdf_Next_ConsoleDemo_Linux.csproj -- https://www.evopdf.com
# or after a build: bin/Linux/Debug/net10.0/EvoPdf_Next_ConsoleDemo_Linux https://www.evopdf.com
```
The PDF is written to `output.pdf`, on A4 pages, with the page laid out as in a 1024 pixel browser window and scaled to the page width. Options go before the URL; run the console demo without arguments to list them all. Examples, one for each page layout:
```bash
# The same layout with a 1280 pixel browser window, on A4 landscape pages
dotnet run --project EvoPdf_Next_ConsoleDemo_Linux.csproj -- /outFileName:fit.pdf /htmlViewerWidth:1280 /orientation:Landscape https://www.evopdf.com
# A page designed for the paper size, laid out at the width of Letter pages
dotnet run --project EvoPdf_Next_ConsoleDemo_Linux.csproj -- /outFileName:page-width.pdf /layout:LayoutAtPageWidth /pageSize:Letter https://www.evopdf.com
# The output of the Save as PDF command of Chrome
dotnet run --project EvoPdf_Next_ConsoleDemo_Linux.csproj -- /outFileName:chrome.pdf /layout:PrintLikeChrome https://www.evopdf.com
# A page as wide as the browser window, with all the content on one page
dotnet run --project EvoPdf_Next_ConsoleDemo_Linux.csproj -- /outFileName:window.pdf /layout:PageWidthFromBrowserWindow /singlePage:true https://www.evopdf.com
```

On Linux install the [system packages](https://www.evopdf.com/help/evopdf-next-dotnet/html/getting-started-on-linux.htm) first. The download package on evopdf.com also contains the .NET 8 variants of the same sources. Without a license the library runs in demo mode and stamps the generated documents. The ASP.NET demo applies the key in one place, the `DemoSettings` class in `Models/Settings`, and it can also be changed at runtime on the Global Settings page. The console demo sets it at the start of `Program.cs`.
