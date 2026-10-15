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
        // Opens this plugin's overlay in whichever shell hosts it: FryWorld, FryPDF or the standalone runner.
        void OpenTicTacToe()
        {
            if (ctx.TryGetService<IShellService>(out var shell)) shell.ShowOverlay(Id);
        }

        // 1. Register Shell Overlay Descriptor
        ctx.RegisterOverlay(new OverlayDescriptor
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
        ctx.RegisterCommand(new CommandPaletteDescriptor
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
        ctx.RegisterStatusBarWidget(new StatusBarWidgetDescriptor
        {
            Id = "fryworld.status.tictactoe",
            Label = "🎮 Tic-Tac-Toe",
            Tooltip = "Play Tic-Tac-Toe (Floating Shell Overlay)",
            AlignRight = true,
            Order = 17,
            ClickCommand = new RelayCommand(OpenTicTacToe)
        });

        // Everything registered through ctx is removed automatically when the plugin unmounts.

        return Task.CompletedTask;
    }
}
