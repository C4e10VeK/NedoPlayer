using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using NedoPlayer.Models;
using NedoPlayer.NedoEventAggregator;
using NedoPlayer.Utils;

namespace NedoPlayer.ViewModels;

public class PlaylistViewModel : BaseViewModel
{
    public event EventHandler? CloseRequested;

    private int _playedMediaIndex;
    
    private Playlist _playlist;
    public Playlist Playlist
    {
        get => _playlist;
        set
        {
            _playlist = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged(nameof(Playlist));
        }
    }
    
    private int _selectedMediaIndex;
    public int SelectedMediaIndex
    {
        get => _selectedMediaIndex;
        set
        {
            _selectedMediaIndex = value;
            OnPropertyChanged(nameof(SelectedMediaIndex));
        }
    }

    public ICommand DeleteMediaFromPlaylistCommand { get; }
    public ICommand RepeatMediaCommand { get; }
    public ICommand CloseCommand { get; }

    private readonly SubscriptionToken _playlistUpdateToken;
    private readonly SubscriptionToken _updatePlayedMediaIndexToken;
    private readonly SubscriptionToken _closeAllWindowToken;
    
    public PlaylistViewModel(IEventAggregator aggregator) : base(aggregator)
    {
        _playlist = new Playlist();
        _playedMediaIndex = -1;

        _playlistUpdateToken = Aggregator.GetEvent<PlaylistUpdateEvent>().Subscribe(playlist => Playlist = playlist);
        _updatePlayedMediaIndexToken = Aggregator.GetEvent<UpdatePlayedMediaIndexEvent>().Subscribe(index => _playedMediaIndex = index);
        _closeAllWindowToken = Aggregator.GetEvent<CloseAllWindowEvent>().Subscribe(() => CloseRequested?.Invoke(this, EventArgs.Empty));

        DeleteMediaFromPlaylistCommand =
            new RelayCommand(_ => Aggregator.GetEvent<DeleteMediaEvent>().Publish(SelectedMediaIndex),
                _ => Playlist.MediaInfos.Any());
        RepeatMediaCommand = new RelayCommand(
            _ => Aggregator.GetEvent<RepeatMediaEvent>().Publish(SelectedMediaIndex),
            _ => Playlist.MediaInfos.Any()
        );

        CloseCommand = new RelayCommand(Close);
    }

    private void Close(object? obj)
    {
        Aggregator.GetEvent<ClosePlaylistWindowEvent>().Publish();
        Aggregator.GetEvent<PlaylistUpdateEvent>().Unsubscribe(_playlistUpdateToken);
        Aggregator.GetEvent<UpdatePlayedMediaIndexEvent>().Unsubscribe(_updatePlayedMediaIndexToken);
        Aggregator.GetEvent<CloseAllWindowEvent>().Unsubscribe(_closeAllWindowToken);
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}