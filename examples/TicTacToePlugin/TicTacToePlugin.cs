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

namespace FryPdf.Plugin.TicTacToe;

/// <summary>
/// Modular plugin contributing an interactive, draggable Tic-Tac-Toe game targeting the shell overlay.
/// </summary>
public class TicTacToePlugin : IFryPlugin
{
    public string Id => "com.fryworld.tictactoe";
    public string Name => "Tic-Tac-Toe";
    public Version Version => new(1, 0, 0);

    public IReadOnlyList<Type> RequiredServices => Array.Empty<Type>();

    public IReadOnlyDictionary<string, PluginSettingDefinition>? SettingsSchema => new Dictionary<string, PluginSettingDefinition>
    {
        ["DefaultMode"] = new()
        {
            Type = PluginSettingType.Select,
            Label = "Default Game Mode",
            Description = "Initial opponent mode on game launch",
            DefaultValue = "PlayerVsAi",
            Options = new() { "PlayerVsAi", "TwoPlayers" }
        },
        ["AiDifficulty"] = new()
        {
            Type = PluginSettingType.Select,
            Label = "AI Difficulty",
            Description = "Tactical strategy level of computer player",
            DefaultValue = "Smart",
            Options = new() { "Smart", "Unbeatable", "Easy" }
        },
        ["SoundEnabled"] = new()
        {
            Type = PluginSettingType.Boolean,
            Label = "Sound Effects",
            Description = "Enable sound alerts for moves and game over events",
            DefaultValue = true
        }
    };

    public Task ApplyAsync(IFryPluginContext ctx, CancellationToken ct = default)
    {
        void OpenTicTacToe()
        {
            var fryWorldVmType = Type.GetType("FryWorld.ViewModels.MainViewModel, FryWorld");
            if (fryWorldVmType != null)
            {
                var mainVm = ctx.GetService(fryWorldVmType);
                fryWorldVmType.GetMethod("NavigateToPlugin")?.Invoke(mainVm, ["com.fryworld.tictactoe"]);
            }
        }

        // 1. Register Shell Overlay Descriptor
        var overlayReg = ctx.RegisterOverlay(new OverlayDescriptor
        {
            Id = Id,
            Title = "🎮 Tic-Tac-Toe",
            DefaultWidth = 330,
            DefaultHeight = 440,
            MinWidth = 280,
            MinHeight = 360,
            IconKind = "GamepadVariantOutline",
            AutoOpenOnMount = false,
            ViewFactory = () => new TicTacToeView
            {
                DataContext = new TicTacToeViewModel()
            }
        });

        // 2. Register Command in Command Palette (Ctrl+Alt+T)
        var cmdReg = ctx.RegisterCommand(new CommandPaletteDescriptor
        {
            Id = "cmd.overlay.tictactoe",
            Title = "Play Tic-Tac-Toe",
            Subtitle = "Launch floating Tic-Tac-Toe mini-game with local 2P & AI opponent",
            Category = "Games",
            IconKind = "GamepadVariantOutline",
            ShortcutKey = "Ctrl+Alt+T",
            Command = new RelayCommand(OpenTicTacToe)
        });

        // 3. Register Footer Status Bar Widget (Clickable 🎮 Tic-Tac-Toe pill)
        var statusReg = ctx.RegisterStatusBarWidget(new StatusBarWidgetDescriptor
        {
            Id = "fryworld.status.tictactoe",
            Label = "🎮 Tic-Tac-Toe",
            Tooltip = "Play Tic-Tac-Toe (Floating Shell Overlay)",
            AlignRight = true,
            Order = 17,
            ClickCommand = new RelayCommand(OpenTicTacToe)
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
