using EvoPdf.Next;

internal class HtmlToPdfDemo
{
    static string urlToConvert = null;
    static string outFileName = "output.pdf";

    static string pageLayout = "FitBrowserWindowToPage";
    static int htmlViewerWidth = 1024;
    static double zoom = 100;
    static bool singlePage = false;
    static PdfPageSize pageSize = PdfPageSize.A4;
    static PdfPageOrientation pageOrientation = PdfPageOrientation.Portrait;
    static int conversionDelaySeconds = 0;

    static public void Main(string[] args)
    {
        // parse arguments
        if (!ParseArguments(args))
        {
            ShowUsage();
            return;
        }

        // convert HTML to PDF
        try
        {
            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Set an additional delay, in seconds, to wait for asynchronous content after the initial load
            // The default value is 0
            htmlToPdfConverter.ConversionDelay = conversionDelaySeconds;

            // Select the page layout. The page size can be a predefined size like A4 or a custom size in points
            switch (pageLayout)
            {
                case "LayoutAtPageWidth":
                    // Fixed page size: the HTML is laid out at the content width of the page, one CSS pixel
                    // being 0.75 points at zoom 100. For HTML templates designed for the paper size
                    htmlToPdfConverter.LayoutAtPageWidth(pageSize, pageOrientation, zoom: zoom, singlePage: singlePage);
                    break;

                case "PrintLikeChrome":
                    // The output of the Save as PDF command of Chrome: the print media type, 1 cm margins,
                    // no background colors or images, drawn at the zoom
                    htmlToPdfConverter.PrintLikeChrome(pageSize, pageOrientation, zoom: zoom, singlePage: singlePage);
                    break;

                case "PageWidthFromBrowserWindow":
                    // The PDF page width follows the browser window width and the HTML is drawn at the zoom,
                    // 1:1 at 100; the page height comes from the page size and the orientation
                    htmlToPdfConverter.PageWidthFromBrowserWindow(htmlViewerWidth, singlePage: singlePage, zoom: zoom);
                    htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = pageSize;
                    htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = pageOrientation;
                    break;

                default:
                    // Fixed page size: the HTML is laid out as in a browser window of the given width and the result
                    // is scaled to the content width of the page, so a responsive site keeps its desktop layout.
                    // This is the default layout of the converter, with an A4 page and a 1024 pixel window
                    htmlToPdfConverter.FitBrowserWindowToPage(pageSize, pageOrientation, windowWidth: htmlViewerWidth, singlePage: singlePage);
                    break;
            }

            // Convert the HTML page given by an URL to a PDF document in a memory buffer
            byte[] pdfBytes = htmlToPdfConverter.ConvertUrl(urlToConvert);

            // Write the PDF buffer to the output file
            System.IO.File.WriteAllBytes(outFileName, pdfBytes);

            Console.WriteLine("Conversion succeeded. Please check the output folder.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format("Error: {0}", ex.Message));
        }
    }

    static bool ParseArguments(string[] args)
    {
        if (args == null || args.Length == 0)
            return false;

        const string outPrefix = "/outFileName:";
        const string pageSizePrefix = "/pageSize:";
        const string orientationPrefix = "/orientation:";
        const string delayPrefix = "/delay:";
        const string layoutPrefix = "/layout:";
        const string htmlViewerWidthPrefix = "/htmlViewerWidth:";
        const string zoomPrefix = "/zoom:";
        const string singlePagePrefix = "/singlePage:";

        urlToConvert = null;

        foreach (var argumentRaw in args)
        {
            string argument = RemoveQuotes(argumentRaw);

            if (argument.StartsWith(outPrefix, StringComparison.OrdinalIgnoreCase))
            {
                outFileName = RemoveQuotes(argument.Substring(outPrefix.Length));
            }
            else if (argument.StartsWith(pageSizePrefix, StringComparison.OrdinalIgnoreCase))
            {
                string sizeText = RemoveQuotes(argument.Substring(pageSizePrefix.Length));
                if (!TryParsePageSize(sizeText, out pageSize))
                {
                    Console.WriteLine("Invalid page size: " + sizeText);
                    return false;
                }
            }
            else if (argument.StartsWith(orientationPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string orientationText = RemoveQuotes(argument.Substring(orientationPrefix.Length));
                if (!Enum.TryParse(orientationText, ignoreCase: true, out pageOrientation))
                {
                    Console.WriteLine("Invalid orientation: " + orientationText);
                    return false;
                }
            }
            else if (argument.StartsWith(delayPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string delayText = RemoveQuotes(argument.Substring(delayPrefix.Length));
                if (!int.TryParse(delayText, out conversionDelaySeconds) || conversionDelaySeconds < 0)
                {
                    Console.WriteLine("Invalid delay (must be a non-negative integer): " + delayText);
                    return false;
                }
            }
            else if (argument.StartsWith(htmlViewerWidthPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string widthText = RemoveQuotes(argument.Substring(htmlViewerWidthPrefix.Length));
                if (!int.TryParse(widthText, out htmlViewerWidth) || htmlViewerWidth <= 0)
                {
                    Console.WriteLine("Invalid htmlViewerWidth (must be a positive integer): " + widthText);
                    return false;
                }
            }
            else if (argument.StartsWith(layoutPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string layoutText = RemoveQuotes(argument.Substring(layoutPrefix.Length));
                string[] layouts = { "FitBrowserWindowToPage", "LayoutAtPageWidth", "PrintLikeChrome", "PageWidthFromBrowserWindow" };
                pageLayout = Array.Find(layouts, l => string.Equals(l, layoutText, StringComparison.OrdinalIgnoreCase));
                if (pageLayout == null)
                {
                    Console.WriteLine("Invalid layout: " + layoutText);
                    return false;
                }
            }
            else if (argument.StartsWith(zoomPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string zoomText = RemoveQuotes(argument.Substring(zoomPrefix.Length));
                if (!double.TryParse(zoomText, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out zoom) || zoom <= 0)
                {
                    Console.WriteLine("Invalid zoom (must be a positive number): " + zoomText);
                    return false;
                }
            }
            else if (argument.StartsWith(singlePagePrefix, StringComparison.OrdinalIgnoreCase))
            {
                string singlePageText = RemoveQuotes(argument.Substring(singlePagePrefix.Length));
                if (!bool.TryParse(singlePageText, out singlePage))
                {
                    Console.WriteLine("Invalid singlePage (must be true or false): " + singlePageText);
                    return false;
                }
            }
            else if (!argument.StartsWith("/", StringComparison.Ordinal))
            {
                urlToConvert = argument;
            }
            else
            {
                Console.WriteLine("Unknown option: " + argument);
                return false;
            }
        }

        return !string.IsNullOrWhiteSpace(urlToConvert);
    }

    static bool TryParsePageSize(string sizeText, out PdfPageSize size)
    {
        size = PdfPageSize.A4;

        switch ((sizeText ?? "").Trim())
        {
            case "Letter": size = PdfPageSize.Letter; return true;
            case "Note": size = PdfPageSize.Note; return true;
            case "Legal": size = PdfPageSize.Legal; return true;

            case "A0": size = PdfPageSize.A0; return true;
            case "A1": size = PdfPageSize.A1; return true;
            case "A2": size = PdfPageSize.A2; return true;
            case "A3": size = PdfPageSize.A3; return true;
            case "A4": size = PdfPageSize.A4; return true;
            case "A5": size = PdfPageSize.A5; return true;
            case "A6": size = PdfPageSize.A6; return true;
            case "A7": size = PdfPageSize.A7; return true;
            case "A8": size = PdfPageSize.A8; return true;
            case "A9": size = PdfPageSize.A9; return true;
            case "A10": size = PdfPageSize.A10; return true;

            case "B0": size = PdfPageSize.B0; return true;
            case "B1": size = PdfPageSize.B1; return true;
            case "B2": size = PdfPageSize.B2; return true;
            case "B3": size = PdfPageSize.B3; return true;
            case "B4": size = PdfPageSize.B4; return true;
            case "B5": size = PdfPageSize.B5; return true;

            case "ArchA": size = PdfPageSize.ArchA; return true;
            case "ArchB": size = PdfPageSize.ArchB; return true;
            case "ArchC": size = PdfPageSize.ArchC; return true;
            case "ArchD": size = PdfPageSize.ArchD; return true;
            case "ArchE": size = PdfPageSize.ArchE; return true;

            case "Flsa": size = PdfPageSize.Flsa; return true;
            case "HalfLetter": size = PdfPageSize.HalfLetter; return true;
            case "Letter11x17": size = PdfPageSize.Letter11x17; return true;
            case "Ledger": size = PdfPageSize.Ledger; return true;

            default:
                return false;
        }
    }

    public static string RemoveQuotes(string quotedString)
    {
        string res = quotedString;
        if (res.StartsWith("\""))
        {
            res = res.Substring(1, res.Length - 1);
        }
        if (res.EndsWith("\""))
        {
            res = res.Substring(0, res.Length - 1);
        }

        return res;
    }

    private static void ShowUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet <ConsoleAppName>.dll [options] URL_to_convert");
        Console.WriteLine("  <ConsoleAppNameExe> [options] URL_to_convert");
        Console.WriteLine();

        Console.WriteLine("Options:");
        Console.WriteLine("  /outFileName:<file>                    Output PDF file name (default: output.pdf)");
        Console.WriteLine("  /pageSize:<A4|A3|Letter|Legal>         PDF page size (default: A4)");
        Console.WriteLine("  /orientation:<Portrait|Landscape>      PDF page orientation (default: Portrait)");
        Console.WriteLine("  /delay:<seconds>                       Conversion delay in seconds (default: 0)");
        Console.WriteLine("  /layout:<name>                         Page layout (default: FitBrowserWindowToPage):");
        Console.WriteLine("                                           FitBrowserWindowToPage, LayoutAtPageWidth,");
        Console.WriteLine("                                           PrintLikeChrome, PageWidthFromBrowserWindow");
        Console.WriteLine("  /htmlViewerWidth:<pixels>              Browser window width in pixels, used by FitBrowserWindowToPage");
        Console.WriteLine("                                           and PageWidthFromBrowserWindow (default: 1024)");
        Console.WriteLine("  /zoom:<percent>                        Zoom of the HTML content, used by LayoutAtPageWidth,");
        Console.WriteLine("                                           PrintLikeChrome and PageWidthFromBrowserWindow (default: 100)");
        Console.WriteLine("  /singlePage:<true|false>               All the content on one page (default: false)");
        Console.WriteLine();

        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet <ConsoleAppName>.dll /outFileName:out.pdf /delay:2 \"https://www.evopdf.com\"");
        Console.WriteLine("  <ConsoleAppNameExe> /outFileName:out.pdf /delay:2 \"https://www.evopdf.com\"");
        Console.WriteLine("  dotnet <ConsoleAppName>.dll /outFileName:out.pdf /pageSize:A4 /orientation:Landscape /delay:2 /layout:LayoutAtPageWidth \"https://www.evopdf.com\"");
        Console.WriteLine();
    }
}
