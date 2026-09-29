using System.Collections.Generic;
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Models
{
    public class SettingsViewModel
    {
        public string LicenseKey { get; set; }
        public string HtmlRendererMode { get; set; }
        public int MaxParallelConversions { get; set; }

        public int StartTimeoutSeconds { get; set; }
        public int IdleTimeoutSeconds { get; set; }
        public int MaxConversions { get; set; }
        public int MaxLifetimeMinutes { get; set; }

        public bool GpuRenderingEnabled { get; set; }
        public bool GpuCompositingEnabled { get; set; }
        public bool EnableSoftwareGpuRendering { get; set; }
        public bool DisableWebSecurity { get; set; }
        public bool AllowInsecureContent { get; set; }
        public bool IgnoreCertificateErrors { get; set; }

        public PersistentRendererStatus Status { get; set; }
        public IReadOnlyList<string> RestartEvents { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }

        public static SettingsViewModel FromCurrent(string message = null, string error = null)
        {
            var p = GlobalSettings.PersistentRenderer;
            return new SettingsViewModel
            {
                LicenseKey = DemoSettings.LicenseKey,
                HtmlRendererMode = GlobalSettings.HtmlRendererMode.ToString(),
                MaxParallelConversions = GlobalSettings.MaxParallelConversions,
                StartTimeoutSeconds = p.StartTimeoutSeconds,
                IdleTimeoutSeconds = p.IdleTimeoutSeconds,
                MaxConversions = p.MaxConversions,
                MaxLifetimeMinutes = p.MaxLifetimeMinutes,
                GpuRenderingEnabled = p.GpuRenderingEnabled,
                GpuCompositingEnabled = p.GpuCompositingEnabled,
                EnableSoftwareGpuRendering = p.EnableSoftwareGpuRendering,
                DisableWebSecurity = p.DisableWebSecurity,
                AllowInsecureContent = p.AllowInsecureContent,
                IgnoreCertificateErrors = p.IgnoreCertificateErrors,
                Status = GlobalSettings.PersistentRendererStatus,
                RestartEvents = DemoSettings.RestartEvents,
                Message = message,
                Error = error
            };
        }
    }
}
