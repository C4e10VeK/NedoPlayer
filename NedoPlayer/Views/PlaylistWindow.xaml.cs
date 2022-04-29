using NedoPlayer.ViewModels;

namespace NedoPlayer.Views;

public partial class PlaylistWindow
{
    public PlaylistWindow()
    {
        InitializeComponent();
        
        if (DataContext is not PlaylistViewModel vm) return;

        vm.CloseRequested += (_, _) => Close();
    }
}