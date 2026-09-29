using System.Collections.Generic;

namespace EvoPdf_Next_AspNetDemo.Models
{
    public class BenchmarkViewModel
    {
        public int Threads { get; set; } = 4;
        public int ConversionsPerThread { get; set; } = 10;
        public string Url { get; set; } = "";
        public bool LoadLazyImages { get; set; } = false;
        public BenchmarkReport Report { get; set; }
        public string CurrentMode { get; set; }
        public int CurrentMaxParallelConversions { get; set; }
        public string Error { get; set; }
    }

    public class BenchmarkReport
    {
        public string Mode { get; set; }
        public int Threads { get; set; }
        public int Conversions { get; set; }
        public int Failures { get; set; }
        public long TotalMs { get; set; }
        public double ConversionsPerSecond { get; set; }
        public long MedianMs { get; set; }
        public long P95Ms { get; set; }
        public long MinMs { get; set; }
        public long MaxMs { get; set; }
        public long FirstMs { get; set; }
        public int PdfBytes { get; set; }
        public int PdfPages { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public int ProcessRestarts { get; set; }
    }
}
