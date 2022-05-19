using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using ControlzEx.Theming;

namespace NedoPlayer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public static string[]? Args;
    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAccent;
        ThemeManager.Current.SyncTheme();
        ThemeManager.Current.ChangeThemeBaseColor(this, "Dark");
        
        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        
        if (e.Args.Length <= 0) return;
        Args = e.Args;
    }
}