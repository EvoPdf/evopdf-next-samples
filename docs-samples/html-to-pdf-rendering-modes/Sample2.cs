GlobalSettings.PersistentRendererRestarted += (sender, e) =>
    logger.LogWarning("HTML rendering engine replaced: {Reason}, new process {ProcessId}", e.Reason, e.ProcessId);

var status = GlobalSettings.PersistentRendererStatus;
if (status.IsRunning)
    logger.LogInformation("Engine process {ProcessId}: {Sent} conversions sent, {InFlight} in progress",
        status.ProcessId, status.ConversionsSent, status.ConversionsInFlight);
