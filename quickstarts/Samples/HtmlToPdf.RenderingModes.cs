using System.Diagnostics;
using EvoPdf.Next;

namespace EvoPdf.Next.Samples
{
    // Run with: dotnet run --project Quickstarts_<Platform>.csproj -- HtmlToPdf.RenderingModes [url]
    public static class HtmlToPdf_RenderingModes
    {
        public static void Run(string[] args)
        {
            // Set the license key from an environment variable; without it the output is watermarked (demo mode).
            string? licenseKey = Environment.GetEnvironmentVariable("EVOPDF_LICENSE_KEY");
            if (!string.IsNullOrEmpty(licenseKey))
                Licensing.LicenseKey = licenseKey;

            string url = args.Length > 0 ? args[0] : "https://www.evopdf.com";

            // One renderer process kept by the application for all the HTML conversions, the default mode.
            // The global settings apply to every conversion started after they are set
            GlobalSettings.HtmlRendererMode = HtmlRendererMode.PersistentProcess;

            // The maximum number of pages loaded at the same time; it can be set only before the first conversion
            GlobalSettings.MaxParallelConversions = 4;

            // Replace the process after 500 conversions and stop it after 5 minutes without conversions
            GlobalSettings.PersistentRenderer.MaxConversions = 500;
            GlobalSettings.PersistentRenderer.IdleTimeoutSeconds = 300;

            GlobalSettings.PersistentRendererRestarted += (sender, e) =>
                Console.WriteLine($"renderer process replaced, reason {e.Reason}, new process id {e.ProcessId}");

            // Start the process now, so that the first conversion does not wait for the engine to start
            var start = Stopwatch.StartNew();
            GlobalSettings.StartPersistentRenderer();
            Console.WriteLine($"renderer process started in {start.ElapsedMilliseconds} ms");

            for (int i = 1; i <= 3; i++)
            {
                var sw = Stopwatch.StartNew();
                byte[] pdf = new HtmlToPdfConverter().ConvertUrl(url);
                File.WriteAllBytes(SampleFiles.Output($"rendering-{i}.pdf"), pdf);
                Console.WriteLine($"output/rendering-{i}.pdf: {sw.ElapsedMilliseconds} ms, {pdf.Length:N0} bytes");
            }

            PersistentRendererStatus status = GlobalSettings.PersistentRendererStatus;
            Console.WriteLine($"process {status.ProcessId} running since {status.StartedAt:HH:mm:ss}, " +
                              $"{status.ConversionsSent} conversions, {status.Restarts} replacements");

            // Stop the process when the application no longer converts
            GlobalSettings.StopPersistentRenderer();
        }
    }
}
