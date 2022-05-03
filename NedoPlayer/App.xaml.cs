using System.Windows;
using ControlzEx.Theming;

namespace NedoPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static string[]? Args;
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
            ThemeManager.Current.SyncTheme();
            
            if (e.Args.Length > 0)
                Args = e.Args;
        }
    }
}