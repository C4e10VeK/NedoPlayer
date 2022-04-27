using System.Windows;

namespace NedoPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[]? Args;
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
                Args = e.Args;
        }
    }
}