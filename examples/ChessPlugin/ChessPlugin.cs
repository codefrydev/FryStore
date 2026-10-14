using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using FryPdf.Plugin.Chess.ViewModels;
using FryPdf.Plugin.Chess.Views;
using FryWorld.Sdk.Descriptors;
using FryWorld.Sdk.Models;
using FryWorld.Sdk.Plugins;
using FryWorld.Sdk.Services;

namespace FryPdf.Plugin.Chess;

/// <summary>
/// Modular plugin contributing an interactive, draggable Chess game (CPU vs User)
/// targeting the shell overlay with M3 Expressive aesthetics.
/// </summary>
public class ChessPlugin : IFryPlugin
{
    public string Id => "com.fryworld.chess";
    public string Name => "Chess";
    public Version Version => new(1, 0, 0);

    public IReadOnlyList<Type> RequiredServices => Array.Empty<Type>();

    public IReadOnlyDictionary<string, PluginSettingDefinition>? SettingsSchema => new Dictionary<string, PluginSettingDefinition>
    {
        ["AiDifficulty"] = new()
        {
            Type = PluginSettingType.Select,
            Label = "CPU Difficulty",
            Description = "Tactical depth and calculation level of the computer opponent",
            DefaultValue = "Medium",
            Options = new() { "Easy", "Medium", "Hard" }
        },
        ["BoardTheme"] = new()
        {
            Type = PluginSettingType.Select,
            Label = "Board Theme",
            Description = "Visual color styling of the 8x8 chessboard",
            DefaultValue = "Emerald",
            Options = new() { "Emerald", "ClassicWood", "MidnightSlate" }
        },
        ["ShowLegalMoveHints"] = new()
        {
            Type = PluginSettingType.Boolean,
            Label = "Highlight Legal Moves",
            Description = "Display target indicators on valid destination squares",
            DefaultValue = true
        },
        ["ShowCoordinates"] = new()
        {
            Type = PluginSettingType.Boolean,
            Label = "Show Board Coordinates",
            Description = "Display ranks (1-8) and files (a-h) along the board edge",
            DefaultValue = true
        }
    };

    public Task ApplyAsync(IFryPluginContext ctx, CancellationToken ct = default)
    {
        void OpenChess()
        {
            var fryWorldVmType = Type.GetType("FryWorld.ViewModels.MainViewModel, FryWorld");
            if (fryWorldVmType != null)
            {
                var mainVm = ctx.GetService(fryWorldVmType);
                fryWorldVmType.GetMethod("NavigateToPlugin")?.Invoke(mainVm, ["com.fryworld.chess"]);
            }
        }

        // 1. Register Shell Overlay Descriptor
        var overlayReg = ctx.RegisterOverlay(new OverlayDescriptor
        {
            Id = Id,
            Title = "♟️ Chess",
            DefaultWidth = 420,
            DefaultHeight = 560,
            MinWidth = 360,
            MinHeight = 480,
            IconKind = "ChessKnight",
            AutoOpenOnMount = false,
            ViewFactory = () => new ChessView
            {
                DataContext = new ChessViewModel()
            }
        });

        // 2. Register Command in Command Palette (Ctrl+Alt+C)
        var cmdReg = ctx.RegisterCommand(new CommandPaletteDescriptor
        {
            Id = "cmd.overlay.chess",
            Title = "Play Chess Game",
            Subtitle = "Launch floating Chess game with CPU opponent (Easy, Medium, Hard)",
            Category = "Games",
            IconKind = "ChessKnight",
            ShortcutKey = "Ctrl+Alt+C",
            Command = new RelayCommand(OpenChess)
        });

        // 3. Register Footer Status Bar Widget (Clickable ♟️ Chess pill)
        var statusReg = ctx.RegisterStatusBarWidget(new StatusBarWidgetDescriptor
        {
            Id = "fryworld.status.chess",
            Label = "♟️ Chess",
            Tooltip = "Play Chess (Floating Shell Overlay)",
            AlignRight = true,
            Order = 16,
            ClickCommand = new RelayCommand(OpenChess)
        });

        // 4. Reversible effects for clean unmount
        ctx.RegisterEffect(() =>
        {
            overlayReg.Dispose();
            cmdReg.Dispose();
            statusReg.Dispose();
        });

        return Task.CompletedTask;
    }
}
