using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Material.Icons.Avalonia;

namespace FryPdf.Runner.Shared;

/// <summary>
/// The Material 3 Expressive look FryWorld plugins are designed against, for standalone runners. Pass it to
/// <c>FryRunner.Configure&lt;TPlugin&gt;(RunnerTheme.Apply)</c> so plugin views find the same M3 brushes and
/// styles they get inside the host.
/// </summary>
public static class RunnerTheme
{
    private const string Assembly = "avares://FryPdf.Runner.Shared";

    public static void Apply(Application app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.RequestedThemeVariant = ThemeVariant.Dark;
        app.Resources.MergedDictionaries.Add(new ResourceInclude(new Uri(Assembly))
        {
            Source = new Uri($"{Assembly}/Styles/Material3ExpressiveTokens.axaml")
        });

        app.Styles.Add(new FluentTheme());
        app.Styles.Add(new MaterialIconStyles(null));
        app.Styles.Add(new StyleInclude(new Uri(Assembly))
        {
            Source = new Uri($"{Assembly}/Styles/Material3ExpressiveStyles.axaml")
        });
    }
}
