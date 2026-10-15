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

namespace PdfEditorApp.Plugins.Snake;

/// <summary>
/// Modular plugin contributing a playable, draggable Snake game targeting the shell overlay.
/// </summary>
public class SnakeGamePlugin : IFryPlugin
{
    public string Id => "com.fryworld.snake";
    public string Name => "Snake";
    public Version Version => new(1, 0, 0);
    public bool AutoOpenOverlay => false;
    public IReadOnlyList<Type> RequiredServices => Array.Empty<Type>();

    public IReadOnlyDictionary<string, PluginSettingDefinition>? SettingsSchema => new Dictionary<string, PluginSettingDefinition>
    {
        ["InitialSpeed"] = new()
        {
            Label = "Game Speed",
            Description = "Default tick speed for snake movement",
            Type = PluginSettingType.Select,
            Options = new() { "Normal", "Fast", "Zen" },
            DefaultValue = "Normal"
        },
        ["WallCollision"] = new()
        {
            Label = "Wall Collision Mode",
            Description = "When enabled, hitting the border causes game over. When disabled, the snake wraps around edges.",
            Type = PluginSettingType.Boolean,
            DefaultValue = true
        }
    };

    public Task ApplyAsync(IFryPluginContext ctx, CancellationToken ct = default)
    {
        // Opens this plugin's overlay in whichever shell hosts it: FryWorld, FryPDF or the standalone runner.
        void OpenSnake()
        {
            if (ctx.TryGetService<IShellService>(out var shell)) shell.ShowOverlay(Id);
        }

        // 1. Register Shell Overlay Descriptor
        ctx.RegisterOverlay(new OverlayDescriptor
        {
            Id = Id,
            Title = "🐍 Snake",
            DefaultWidth = 320,
            DefaultHeight = 420,
            MinWidth = 280,
            MinHeight = 360,
            IconKind = "GamepadVariantOutline",
            AutoOpenOnMount = false,
            ViewFactory = () => new SnakeGameView
            {
                DataContext = new SnakeGameViewModel(ctx)
            }
        });

        // 2. Register Command Palette action
        ctx.RegisterCommand(new CommandPaletteDescriptor
        {
            Id = "cmd.overlay.snake",
            Title = "Play Snake Game",
            Subtitle = "Launch retro-arcade Snake game floating over the application",
            Category = "Games",
            IconKind = "GamepadVariantOutline",
            ShortcutKey = "Ctrl+Alt+S",
            Command = new RelayCommand(OpenSnake)
        });

        // 3. Register Footer Status Bar Widget (Clickable 🐍 Snake pill)
        ctx.RegisterStatusBarWidget(new StatusBarWidgetDescriptor
        {
            Id = "fryworld.status.snake",
            Label = "🐍 Snake",
            Tooltip = "Play Snake Game (Floating Shell Overlay)",
            AlignRight = true,
            Order = 15,
            ClickCommand = new RelayCommand(OpenSnake)
        });

        // Everything registered through ctx is removed automatically when the plugin unmounts.

        return Task.CompletedTask;
    }
}
