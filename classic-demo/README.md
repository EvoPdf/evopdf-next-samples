# EvoPdf Classic demo on the Next engine

The ASP.NET Core demo application of EvoPdf Classic, built on the `EvoPdf.HtmlToPdf` platform packages, from the download package on evopdf.com (`EvoPdf-Classic-Demo-v15.0.3.zip`). The application keeps the Classic API, the `EvoPdf` namespace included, and the pages render with the Chromium engine of EvoPdf Next. It is the same application a Classic customer runs after changing the package reference, so its pages show how a Classic application looks on the new engine.

| Project suffix | Package | Runs on |
|---|---|---|
| `_Windows` / `_Windows.Arm64` | `EvoPdf.HtmlToPdf.Windows` / `.Windows.Arm64` | Windows x64 / ARM64 |
| `_Linux` / `_Linux.Arm64` | `EvoPdf.HtmlToPdf.Linux` / `.Linux.Arm64` | Linux x64 / ARM64 |
| `_MacOS` | `EvoPdf.HtmlToPdf.MacOS` | macOS (Apple Silicon) |
| `_MultiPlatform` / `_MultiPlatform.Arm64` | `EvoPdf.HtmlToPdf.Windows` + `.Linux` / `.Windows.Arm64` + `.Linux.Arm64` | Windows + Linux from one build |

The source files exist once; each project keeps its own `obj`/`bin`, so the projects build side by side.

## Running the demo

- **Visual Studio**: open the platform solution from the repository root (`EvoPdf.Next.Samples.<Platform>.sln`), set `EvoPdf_Classic_AspNetDemo_<Platform>` as the startup project, F5.
- **.NET CLI**, from the `AspNetDemo` folder: `dotnet run --project EvoPdf_Classic_AspNetDemo_Windows.csproj` (pick your platform). The address comes from `Properties/launchSettings.json`, `http://localhost:27110`.
- **Published**: `dotnet publish EvoPdf_Classic_AspNetDemo_Linux.csproj -c Release -o publish`; the published application listens on `http://localhost:5000` unless an address is given with `--urls` or with the `ASPNETCORE_URLS` environment variable.

## The Classic code and the Next options

The code of the pages is the Classic code. The converter runs over an EvoPdf Next converter, created for each conversion: the `Next` property exposes it and the `NextConverterCreated` event sets on it the options that have no Classic counterpart. The Getting Started page selects a PDF/UA or PDF/A standard this way and the Create Electronic Invoice page embeds a Factur-X / ZUGFeRD invoice:

```csharp
htmlToPdfConverter.NextConverterCreated += nextConverter =>
{
    nextConverter.PdfDocumentOptions.PdfStandard = EvoPdf.Next.PdfStandard.PdfA3b;
    nextConverter.PdfDocumentOptions.AddElectronicInvoice(invoice);
};
```

## Linux and macOS

On Linux the rendering engine needs a few system libraries, listed in the [Getting Started on Linux](https://www.evopdf.com/help/evopdf-next-dotnet/html/getting-started-on-linux.htm) topic. On macOS it needs nothing else.

The pages that create fonts or images from `System.Drawing` objects (the pages that draw text with a system font or place a `System.Drawing.Image`, and Convert HTML to Image, which returns a `System.Drawing.Image`) also need libgdiplus, the native library of System.Drawing on Linux and macOS. Azure App Service for Linux has it installed; elsewhere:

```bash
apt-get install libgdiplus        # Debian and Ubuntu; libgdiplus on Red Hat and Amazon Linux through EPEL
brew install mono-libgdiplus      # macOS
```

On macOS with Apple Silicon .NET does not look in the Homebrew folder, so the library is linked in the folder of the application (the build output folder or the publish folder): `ln -s /opt/homebrew/lib/libgdiplus.dylib libgdiplus.dylib`. The other pages do not use libgdiplus.

## License

Without a license key the pages carry the evaluation stamp. The license key is set with the `LicenseKey` property of the Classic converter and document objects, as in the Classic application. The HTML to PDF license key of version 15 is the same for EvoPdf Classic and EvoPdf Next.

Migration guide: https://www.evopdf.com/evopdf-classic-to-next-migration
