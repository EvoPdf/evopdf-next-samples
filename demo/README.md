# EvoPdf Next demo applications

The .NET 10 demo applications from the official download package (`EvoPdf-Next-v14.36.0.zip`): one source tree each, with a project file per target platform in the same folder (each project keeps its own `obj`/`bin`, so they build side by side).

| Project suffix | Package | Runs on |
|---|---|---|
| `_Windows` / `_Windows.Arm64` | `EvoPdf.Next.Windows` / `.Windows.Arm64` | Windows x64 / ARM64 |
| `_Linux` / `_Linux.Arm64` | `EvoPdf.Next.Linux` / `.Linux.Arm64` | Linux x64 / ARM64 |
| `_MacOS` | `EvoPdf.Next.MacOS` | macOS (Apple Silicon) |
| `_MultiPlatform` / `_MultiPlatform.Arm64` | `EvoPdf.Next` / `.Arm64` | Windows + Linux from one build |

## AspNetDemo
The ASP.NET Core MVC application that runs at [evopdf.com](https://www.evopdf.com/evopdf-next-aspnet-demo/), with the C# source of every demo page under `Controllers/` (HTML to PDF, HTML to Image, PDF Creator, PDF Editor, Word / Excel / RTF / Markdown to PDF, PDF to Text, Find Text, PDF to Image, Extract PDF Images).

`wwwroot` (styles, images, the demo input files) is not copied to `bin` by a build — ASP.NET Core serves it from the project folder at development time — so run the application in one of these ways, not by starting the executable from `bin`:
- **Visual Studio**: open the platform solution from the repository root (`EvoPdf.Next.Samples.<Platform>.sln`), set `EvoPdf_Next_AspNetDemo_<Platform>` as the startup project, F5.
- **.NET CLI**, from this folder: `dotnet run --project EvoPdf_Next_AspNetDemo_Windows.csproj` (pick your platform), then open the URL printed by Kestrel.
- **Published**: `dotnet publish EvoPdf_Next_AspNetDemo_Linux.csproj -c Release -o publish` copies `wwwroot` next to the executable; the application then runs from the `publish` folder on any machine, under IIS or in a container.

## ConsoleDemo
A single-file command-line HTML to PDF converter (`Program.cs`), the smallest complete application built on the library. It has no content files and runs straight from the build output:
```bash
dotnet run --project EvoPdf_Next_ConsoleDemo_Linux.csproj -- https://www.evopdf.com output.pdf
# or after a build: bin/Linux/Debug/net10.0/EvoPdf_Next_ConsoleDemo_Linux https://www.evopdf.com output.pdf
```
Run it without arguments to see the options (URL, output file, viewer width, page size, orientation, conversion delay).

On Linux install the [system packages](https://www.evopdf.com/help/evopdf-next-dotnet/html/getting-started-on-linux.htm) first. The download package on evopdf.com also contains the .NET 8 variants of the same sources. The demo license key set in the sources is a public evaluation key that keeps the demo output free of watermarks; it is not a production key.
