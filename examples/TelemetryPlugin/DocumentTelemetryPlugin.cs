using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using FryWorld.Sdk.Descriptors;
using FryWorld.Sdk.Models;
using FryWorld.Sdk.Plugins;
using FryWorld.Sdk.Services;

namespace PdfEditorApp.Plugins.Telemetry;

/// <summary>
/// Plugin contributing a live diagnostics & memory telemetry HUD targeting the shell overlay.
/// </summary>
public class DocumentTelemetryPlugin : IFryPlugin
{
    public string Id => "com.fryworld.telemetry";
    public string Name => "Document Telemetry HUD";
    public Version Version => new(1, 0, 0);
    public IReadOnlyList<Type> RequiredServices => Array.Empty<Type>();

    public IReadOnlyDictionary<string, PluginSettingDefinition>? SettingsSchema => null;

    public Task ApplyAsync(IFryPluginContext ctx, CancellationToken ct = default)
    {
        void OpenTelemetry()
        {
            var fryWorldVmType = Type.GetType("FryWorld.ViewModels.MainViewModel, FryWorld");
            if (fryWorldVmType != null)
            {
                var mainVm = ctx.GetService(fryWorldVmType);
                fryWorldVmType.GetMethod("NavigateToPlugin")?.Invoke(mainVm, ["com.fryworld.telemetry"]);
            }
        }

        // 1. Register Overlay
        var overlayReg = ctx.RegisterOverlay(new OverlayDescriptor
        {
            Id = Id,
            Title = "⚡ Telemetry HUD",
            DefaultWidth = 320,
            DefaultHeight = 250,
            MinWidth = 260,
            MinHeight = 200,
            IconKind = "ChartTimelineVariant",
            AutoOpenOnMount = false,
            ViewFactory = () => new DocumentTelemetryView
            {
                DataContext = new DocumentTelemetryViewModel(ctx)
            }
        });

        // 2. Register Command Palette
        var cmdReg = ctx.RegisterCommand(new CommandPaletteDescriptor
        {
            Id = "cmd.overlay.telemetry",
            Title = "Toggle Telemetry HUD",
            Subtitle = "Display real-time memory and engine diagnostics",
            Category = "Diagnostics",
            IconKind = "ChartTimelineVariant",
            ShortcutKey = "Ctrl+Alt+T",
            Command = new RelayCommand(OpenTelemetry)
        });

        // 3. Register Footer Status Bar Widget
        var statusReg = ctx.RegisterStatusBarWidget(new StatusBarWidgetDescriptor
        {
            Id = "fryworld.status.telemetry",
            Label = "⚡ HUD",
            Tooltip = "Toggle Document Telemetry HUD",
            AlignRight = true,
            Order = 13,
            ClickCommand = new RelayCommand(OpenTelemetry)
        });

        // Clean rollback on unload
        ctx.RegisterEffect(() =>
        {
            overlayReg.Dispose();
            cmdReg.Dispose();
            statusReg.Dispose();
        });

        return Task.CompletedTask;
    }
}
