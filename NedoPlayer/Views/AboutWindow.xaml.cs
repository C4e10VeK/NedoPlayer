using NedoPlayer.ViewModels;

namespace NedoPlayer.Views;

public partial class AboutWindow
{
    public AboutWindow()
    {
        InitializeComponent();
        
        if (DataContext is not AboutViewModel vm)
            return;

        vm.CloseRequested += (_, _) => Close();
    }
}