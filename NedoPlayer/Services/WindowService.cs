using System.Windows;

namespace NedoPlayer.Services;

public class WindowService : IWindowService
{
    public void OpenWindow<T>(Window? parent = null) where T : Window, new()
    {
        T wnd = new T
        {
            Owner = parent
        };
        wnd.Show();
    }

    public bool? OpenDialogWindow<T>(Window? parent = null) where T : Window, new()
    {
        T wnd = new T
        {
            Owner = parent
        };
        var res = wnd.ShowDialog();
        return res;
    }
}