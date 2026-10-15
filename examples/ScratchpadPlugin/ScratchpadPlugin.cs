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
        // Opens this plugin's overlay in whichever shell hosts it: FryWorld, FryPDF or the standalone runner.
        void OpenScratchpad()
        {
            if (ctx.TryGetService<IShellService>(out var shell)) shell.ShowOverlay(Id);
        }

        // 1. Register Overlay
        ctx.RegisterOverlay(new OverlayDescriptor
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
        ctx.RegisterCommand(new CommandPaletteDescriptor
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
        ctx.RegisterStatusBarWidget(new StatusBarWidgetDescriptor
        {
            Id = "fryworld.status.scratchpad",
            Label = "📝 Notes",
            Tooltip = "Toggle Review Scratchpad",
            AlignRight = true,
            Order = 14,
            ClickCommand = new RelayCommand(OpenScratchpad)
        });

        // Everything registered through ctx is removed automatically when the plugin unmounts.

        return Task.CompletedTask;
    }
}
