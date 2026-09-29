var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() => GlobalSettings.StartPersistentRenderer());
app.Lifetime.ApplicationStopping.Register(() => GlobalSettings.StopPersistentRenderer());
