using System;
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
        ThemeManager.Current.SyncTheme(ThemeSyncMode.SyncWithAccent);
        ThemeManager.Current.ChangeThemeBaseColor(this, "Dark");

        if (Environment.OSVersion.Version < Version.Parse("6.2.9200.0"))
            ThemeManager.Current.ChangeTheme(this, "Dark.Mauve");
        
        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        
        if (e.Args.Length <= 0) return;
        Args = e.Args;
    }
}