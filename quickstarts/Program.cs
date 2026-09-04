using EvoPdf.Next.Samples;

// One project for all quickstarts: the NuGet package copies the native runtimes (hundreds of MB per platform)
// into the output folder of every project that references it, so one shared project keeps the build small.
// Usage: dotnet run --project Quickstarts_<Platform>.csproj -- <SampleName> [input]   or   Quickstarts.exe <SampleName> [input]

var samples = new Dictionary<string, Action<string[]>>(StringComparer.OrdinalIgnoreCase)
{
    ["ExcelToPdf"] = ExcelToPdf.Run,
    ["HtmlToImage"] = HtmlToImage.Run,
    ["HtmlToPdf.Basic"] = HtmlToPdf_Basic.Run,
    ["HtmlToPdf.DynamicContent"] = HtmlToPdf_DynamicContent.Run,
    ["HtmlToPdf.HeadersFooters"] = HtmlToPdf_HeadersFooters.Run,
    ["HtmlToPdf.PageSetup"] = HtmlToPdf_PageSetup.Run,
    ["HtmlToPdf.Security"] = HtmlToPdf_Security.Run,
    ["HtmlToPdf.Standards"] = HtmlToPdf_Standards.Run,
    ["MarkdownToPdf"] = MarkdownToPdf.Run,
    ["PdfEditor.Stamp"] = PdfEditor_Stamp.Run,
    ["PdfProcessor.ExtractImages"] = PdfProcessor_ExtractImages.Run,
    ["PdfProcessor.FindText"] = PdfProcessor_FindText.Run,
    ["PdfProcessor.PdfToImage"] = PdfProcessor_PdfToImage.Run,
    ["PdfProcessor.PdfToText"] = PdfProcessor_PdfToText.Run,
    ["RtfToPdf"] = RtfToPdf.Run,
    ["WordToPdf"] = WordToPdf.Run,
};

if (args.Length == 0 || !samples.TryGetValue(args[0], out Action<string[]>? run))
{
    Console.WriteLine("EvoPdf Next quickstarts. Pass the name of a sample, optionally followed by its input:");
    Console.WriteLine();
    Console.WriteLine("  from the project folder:   dotnet run --project Quickstarts_Windows.csproj -- HtmlToPdf.Basic https://www.evopdf.com");
    Console.WriteLine("                             (Quickstarts_Linux.csproj, Quickstarts_MacOS.csproj, ... for other platforms)");
    Console.WriteLine("  from the build output:     Quickstarts.exe ExcelToPdf            (uses Files\\Excel_Document.xlsx)");
    Console.WriteLine("                             Quickstarts.exe ExcelToPdf my.xlsx    (your own file)");
    Console.WriteLine();
    Console.WriteLine("Results are written to the output folder next to the executable. Samples:");
    foreach (string name in samples.Keys)
        Console.WriteLine("  " + name);
    return;
}

run(args.Skip(1).ToArray());
