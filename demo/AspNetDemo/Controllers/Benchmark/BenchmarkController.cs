using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers
{
    public class BenchmarkController : Controller
    {
        private const int MaxThreads = 32;
        private const int MaxConversionsPerThread = 100;

        public IActionResult Index()
        {
            // The first run measures the configuration of the Global Settings page
            return View(WithCurrent(new BenchmarkViewModel { Threads = Math.Min(MaxThreads, Math.Max(1, GlobalSettings.MaxParallelConversions)) }));
        }

        private static BenchmarkViewModel WithCurrent(BenchmarkViewModel model)
        {
            model.CurrentMode = GlobalSettings.HtmlRendererMode == HtmlRendererMode.PersistentProcess ? "persistent process" : "process per conversion";
            model.CurrentMaxParallelConversions = GlobalSettings.MaxParallelConversions;
            return model;
        }

        [HttpPost]
        public async Task<IActionResult> Run(BenchmarkViewModel model)
        {
            if (model.Threads < 1 || model.Threads > MaxThreads || model.ConversionsPerThread < 1 || model.ConversionsPerThread > MaxConversionsPerThread)
            {
                model.Error = $"Threads must be between 1 and {MaxThreads}, conversions per thread between 1 and {MaxConversionsPerThread}.";
                return View("Index", WithCurrent(model));
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            string html = string.IsNullOrWhiteSpace(model.Url) ? SamplePage() : null;
            var report = new BenchmarkReport { Mode = GlobalSettings.HtmlRendererMode.ToString(), Threads = model.Threads };
            int restartsBefore = GlobalSettings.PersistentRendererStatus.Restarts;

            // One conversion first: the engine startup and the warm-up are reported apart from the run
            var first = Stopwatch.StartNew();
            try
            {
                byte[] pdf = Convert(html, model.Url, model.LoadLazyImages);
                report.FirstMs = first.ElapsedMilliseconds;
                report.PdfBytes = pdf.Length;
                report.PdfPages = CountPages(pdf);
            }
            catch (Exception ex)
            {
                model.Error = "The first conversion failed: " + ex.Message;
                return View("Index", WithCurrent(model));
            }

            var times = new List<long>();
            var errors = new List<string>();
            var gate = new object();
            var total = Stopwatch.StartNew();
            var workers = Enumerable.Range(0, model.Threads).Select(_ => Task.Run(() =>
            {
                for (int i = 0; i < model.ConversionsPerThread; i++)
                {
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        Convert(html, model.Url, model.LoadLazyImages);
                        lock (gate) times.Add(sw.ElapsedMilliseconds);
                    }
                    catch (Exception ex)
                    {
                        lock (gate) { if (errors.Count < 10) errors.Add(ex.Message); report.Failures++; }
                    }
                }
            })).ToArray();
            await Task.WhenAll(workers);
            report.TotalMs = total.ElapsedMilliseconds;

            times.Sort();
            report.Conversions = times.Count + report.Failures;
            if (times.Count > 0)
            {
                report.ConversionsPerSecond = Math.Round(times.Count * 1000.0 / Math.Max(1, report.TotalMs), 2);
                report.MedianMs = times[times.Count / 2];
                report.P95Ms = times[Math.Max(0, (int)Math.Ceiling(times.Count * 0.95) - 1)];
                report.MinMs = times[0];
                report.MaxMs = times[times.Count - 1];
            }
            report.Errors = errors;
            report.ProcessRestarts = GlobalSettings.PersistentRendererStatus.Restarts - restartsBefore;
            model.Report = report;
            return View("Index", WithCurrent(model));
        }

        private static byte[] Convert(string html, string url, bool loadLazyImages)
        {
            var converter = new HtmlToPdfConverter();
            converter.LoadLazyImages = loadLazyImages;
            converter.NavigationTimeout = 60;
            return html != null ? converter.ConvertHtml(html, "http://localhost/") : converter.ConvertUrl(url);
        }

        // Counts the page objects of the PDF
        private static int CountPages(byte[] pdf)
        {
            string text = System.Text.Encoding.ASCII.GetString(pdf);
            return System.Text.RegularExpressions.Regex.Matches(text, @"/Type\s*/Page(?![a-zA-Z])").Count;
        }

        // A two page invoice-like document with a table, inline styles and an inline SVG, without external resources
        private static string SamplePage()
        {
            var rows = new System.Text.StringBuilder();
            decimal total = 0;
            for (int i = 1; i <= 60; i++)
            {
                int quantity = (i * 5) % 11 + 1;
                decimal unitPrice = i * 3.5m;
                decimal lineTotal = quantity * unitPrice;
                total += lineTotal;
                rows.Append($"<tr><td>{i}</td><td>Item {i}</td><td>{quantity}</td><td>{unitPrice:F2}</td><td>{lineTotal:F2}</td></tr>");
            }
            return "<html><head><style>body{font-family:Arial,sans-serif;font-size:12px;margin:24px}h1{color:#1E6FB8}table{border-collapse:collapse;width:100%}" +
                "td,th{border:1px solid #ccc;padding:4px 8px}th{background:#f0f4f8}tfoot td{font-weight:bold}</style></head><body>" +
                "<h1>Benchmark invoice</h1><p>Generated for the benchmark page. Sixty rows, an SVG chart, two pages.</p>" +
                "<svg width=\"400\" height=\"80\"><rect x=\"0\" y=\"30\" width=\"120\" height=\"20\" fill=\"#1E6FB8\"/><rect x=\"0\" y=\"55\" width=\"260\" height=\"20\" fill=\"#A33B35\"/></svg>" +
                "<table><thead><tr><th>#</th><th>Description</th><th>Qty</th><th>Unit</th><th>Total</th></tr></thead><tbody>" + rows +
                $"</tbody><tfoot><tr><td colspan=\"4\">Total</td><td>{total:F2}</td></tr></tfoot></table></body></html>";
        }
    }
}
