using NedoPlayer.NedoEventAggregator;
using NedoPlayer.Services;
using NedoPlayer.ViewModels;

namespace NedoPlayer.Utils;

public class ViewModelLocator
{
    private MainViewModel? _mainViewModel;
    private AboutViewModel? _aboutViewModel;
    private PlaylistViewModel? _playlistViewModel;

    public MainViewModel MainViewModel => _mainViewModel ??=
        new MainViewModel(EventAggregator.Instance, 
            ServiceLocator.Instance.Container.Get<IFileService>(),
            ServiceLocator.Instance.Container.Get<IStateService>(),
            ServiceLocator.Instance.Container.Get<IWindowService>(),
            ServiceLocator.Instance.Container.Get<IConfigFileService>());

    public AboutViewModel AboutViewModel => _aboutViewModel ??= new AboutViewModel(EventAggregator.Instance);

    public PlaylistViewModel PlaylistViewModel => _playlistViewModel ??= new PlaylistViewModel(EventAggregator.Instance);
}