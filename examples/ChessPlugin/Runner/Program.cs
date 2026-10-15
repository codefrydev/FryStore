using System;
using Avalonia;
using FryPdf.Runner.Shared;
using FryWorld.Sdk.Runner;
using FryPdf.Plugin.Chess;

namespace FryPdf.Plugin.Chess.Runner;

/// <summary>
/// Runs the plugin as a standalone app, with the same shell, settings and theme services it gets in FryWorld.
/// Settings are saved to {plugin id}.runner.settings.json next to the runner.
/// </summary>
public static class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    // Also used by the Avalonia previewer.
    public static AppBuilder BuildAvaloniaApp() =>
        FryRunner.Configure<ChessPlugin>(RunnerTheme.Apply, o => o.Title = "Chess")
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
