using System;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View(SettingsViewModel.FromCurrent());
        }

        public IActionResult Renderer()
        {
            return View(SettingsViewModel.FromCurrent());
        }

        [HttpPost]
        public IActionResult Save(SettingsViewModel model)
        {
            try
            {
                DemoSettings.LicenseKey = model.LicenseKey ?? string.Empty;
                DemoSettings.ApplyLicense();

                GlobalSettings.HtmlRendererMode = model.HtmlRendererMode == nameof(HtmlRendererMode.ProcessPerConversion)
                    ? HtmlRendererMode.ProcessPerConversion : HtmlRendererMode.PersistentProcess;
                GlobalSettings.MaxParallelConversions = model.MaxParallelConversions;
                string ignored = GlobalSettings.MaxParallelConversions != model.MaxParallelConversions
                    ? $" Maximum parallel conversions stays {GlobalSettings.MaxParallelConversions}: it can be set only before the first conversion of the application."
                    : string.Empty;

                var p = GlobalSettings.PersistentRenderer;
                p.StartTimeoutSeconds = model.StartTimeoutSeconds;
                p.IdleTimeoutSeconds = model.IdleTimeoutSeconds;
                p.MaxConversions = model.MaxConversions;
                p.MaxLifetimeMinutes = model.MaxLifetimeMinutes;
                p.GpuRenderingEnabled = model.GpuRenderingEnabled;
                p.GpuCompositingEnabled = model.GpuCompositingEnabled;
                p.EnableSoftwareGpuRendering = model.EnableSoftwareGpuRendering;
                p.DisableWebSecurity = model.DisableWebSecurity;
                p.AllowInsecureContent = model.AllowInsecureContent;
                p.IgnoreCertificateErrors = model.IgnoreCertificateErrors;

                return View("Index", SettingsViewModel.FromCurrent("Configuration saved. The next conversion uses it. A change of the process settings replaces the persistent renderer process at the next conversion." + ignored));
            }
            catch (Exception ex)
            {
                return View("Index", SettingsViewModel.FromCurrent(null, ex.Message));
            }
        }

        [HttpPost]
        public IActionResult Start()
        {
            try
            {
                var started = DateTime.Now;
                GlobalSettings.StartPersistentRenderer();
                return View("Renderer", SettingsViewModel.FromCurrent($"Persistent renderer process started in {(DateTime.Now - started).TotalMilliseconds:F0} ms."));
            }
            catch (Exception ex)
            {
                return View("Renderer", SettingsViewModel.FromCurrent(null, ex.Message));
            }
        }

        [HttpPost]
        public IActionResult Stop()
        {
            try
            {
                var started = DateTime.Now;
                GlobalSettings.StopPersistentRenderer();
                return View("Renderer", SettingsViewModel.FromCurrent($"Persistent renderer process stopped in {(DateTime.Now - started).TotalMilliseconds:F0} ms."));
            }
            catch (Exception ex)
            {
                return View("Renderer", SettingsViewModel.FromCurrent(null, ex.Message));
            }
        }
    }
}
