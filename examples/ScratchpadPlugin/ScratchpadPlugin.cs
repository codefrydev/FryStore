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

namespace PdfEditorApp.Plugins.Scratchpad;

/// <summary>
/// Plugin contributing a floating Markdown Review Scratchpad targeting the shell overlay.
/// </summary>
public class ScratchpadPlugin : IFryPlugin
{
    public string Id => "com.fryworld.scratchpad";
    public string Name => "Review Scratchpad";
    public Version Version => new(1, 0, 0);
    public IReadOnlyList<Type> RequiredServices => Array.Empty<Type>();

    public IReadOnlyDictionary<string, PluginSettingDefinition>? SettingsSchema => new Dictionary<string, PluginSettingDefinition>
    {
        ["AutoClearOnClose"] = new()
        {
            Label = "Auto-Clear Notes on Close",
            Description = "Automatically clear notes text when closing the scratchpad overlay",
            Type = PluginSettingType.Boolean,
            DefaultValue = false
        }
    };

    public Task ApplyAsync(IFryPluginContext ctx, CancellationToken ct = default)
    {
        void OpenScratchpad()
        {
            var fryWorldVmType = Type.GetType("FryWorld.ViewModels.MainViewModel, FryWorld");
            if (fryWorldVmType != null)
            {
                var mainVm = ctx.GetService(fryWorldVmType);
                fryWorldVmType.GetMethod("NavigateToPlugin")?.Invoke(mainVm, ["com.fryworld.scratchpad"]);
            }
        }

        // 1. Register Overlay
        var overlayReg = ctx.RegisterOverlay(new OverlayDescriptor
        {
            Id = Id,
            Title = "📝 Scratchpad",
            DefaultWidth = 360,
            DefaultHeight = 340,
            MinWidth = 280,
            MinHeight = 240,
            IconKind = "NotebookEditOutline",
            AutoOpenOnMount = false,
            ViewFactory = () => new ScratchpadView
            {
                DataContext = new ScratchpadViewModel(ctx)
            }
        });

        // 2. Register Command Palette
        var cmdReg = ctx.RegisterCommand(new CommandPaletteDescriptor
        {
            Id = "cmd.overlay.scratchpad",
            Title = "Toggle Review Scratchpad",
            Subtitle = "Open floating note pad to take notes during review",
            Category = "Notes",
            IconKind = "NotebookEditOutline",
            ShortcutKey = "Ctrl+Alt+N",
            Command = new RelayCommand(OpenScratchpad)
        });

        // 3. Register Footer Status Bar Widget
        var statusReg = ctx.RegisterStatusBarWidget(new StatusBarWidgetDescriptor
        {
            Id = "fryworld.status.scratchpad",
            Label = "📝 Notes",
            Tooltip = "Toggle Review Scratchpad",
            AlignRight = true,
            Order = 14,
            ClickCommand = new RelayCommand(OpenScratchpad)
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
