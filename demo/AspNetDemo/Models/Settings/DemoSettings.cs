using System;
using System.Collections.Generic;
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models
{
    // Demo application state kept for the application lifetime: the license key applied by every
    // demo page and the last renderer process replacements shown on the Persistent Renderer Process page
    public static class DemoSettings
    {
        private static readonly object eventsLock = new object();
        private static readonly List<string> restartEvents = new List<string>();
        private const int MaxRestartEvents = 20;

        // The key applied before each conversion. Empty means demo mode
        public static string LicenseKey { get; set; } = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

        static DemoSettings()
        {
            GlobalSettings.PersistentRendererRestarted += (sender, e) =>
            {
                lock (eventsLock)
                {
                    restartEvents.Insert(0, $"{DateTime.Now:HH:mm:ss}  process replaced, reason {e.Reason}, new process id {e.ProcessId}");
                    if (restartEvents.Count > MaxRestartEvents)
                        restartEvents.RemoveAt(restartEvents.Count - 1);
                }
            };
        }

        public static void ApplyLicense()
        {
            Licensing.LicenseKey = string.IsNullOrWhiteSpace(LicenseKey) ? null : LicenseKey.Trim();
        }

        public static IReadOnlyList<string> RestartEvents
        {
            get { lock (eventsLock) { return restartEvents.ToArray(); } }
        }
    }
}
