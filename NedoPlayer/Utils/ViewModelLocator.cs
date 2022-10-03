using NedoPlayer.NedoEventAggregator;
using NedoPlayer.Services;
using NedoPlayer.ViewModels;

namespace NedoPlayer.Utils;

public class ViewModelLocator
{
    private MainViewModel? _mainViewModel;
    private AboutViewModel? _aboutViewModel;
    private PlaylistViewModel? _playlistViewModel;
    private readonly ServiceLocator _serviceLocator = new();
    private readonly EventAggregator _eventAggregator = new();

    public MainViewModel MainViewModel => _mainViewModel ??=
        new MainViewModel(_eventAggregator,
            _serviceLocator.Container.Get<IFileService>(),
            _serviceLocator.Container.Get<IStateService>(),
            _serviceLocator.Container.Get<IWindowService>(),
            _serviceLocator.Container.Get<IConfigFileService>());

    public AboutViewModel AboutViewModel => _aboutViewModel ??= new AboutViewModel(_eventAggregator);

    public PlaylistViewModel PlaylistViewModel => _playlistViewModel ??= new PlaylistViewModel(_eventAggregator);
}