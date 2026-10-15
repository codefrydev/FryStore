using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace PdfEditorApp.Plugins.Snake;

public partial class SnakeGameView : UserControl
{
    public SnakeGameView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Focus();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Focus();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (DataContext is not SnakeGameViewModel vm) return;

        switch (e.Key)
        {
            case Key.Up:
            case Key.W:
                vm.ChangeDirection(SnakeDirection.Up);
                e.Handled = true;
                break;
            case Key.Down:
            case Key.S:
                vm.ChangeDirection(SnakeDirection.Down);
                e.Handled = true;
                break;
            case Key.Left:
            case Key.A:
                vm.ChangeDirection(SnakeDirection.Left);
                e.Handled = true;
                break;
            case Key.Right:
            case Key.D:
                vm.ChangeDirection(SnakeDirection.Right);
                e.Handled = true;
                break;
            case Key.Space:
                vm.TogglePlayPause();
                e.Handled = true;
                break;
            case Key.R:
                vm.ResetGame();
                vm.StartGame();
                e.Handled = true;
                break;
        }
    }
}
