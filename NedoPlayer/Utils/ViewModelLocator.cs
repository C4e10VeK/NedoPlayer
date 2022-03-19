using NedoPlayer.ViewModels;

namespace NedoPlayer.Utils;

public class ViewModelLocator
{
    private MainViewModel? _mainViewModel;

    public MainViewModel MainViewModel => _mainViewModel ??= new MainViewModel();
}