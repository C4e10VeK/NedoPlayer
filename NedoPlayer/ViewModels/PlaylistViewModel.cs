using System;
using System.Linq;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
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
    
    public PlaylistViewModel(IEventAggregator aggregator) : base(aggregator)
    {
        _playlist = new Playlist();
        _playedMediaIndex = -1;

        Aggregator.GetEvent<PlaylistUpdateEvent>().Subscribe(playlist => Playlist = playlist);
        Aggregator.GetEvent<UpdatePlayedMediaIndex>().Subscribe(index => _playedMediaIndex = index);
        Aggregator.GetEvent<CloseAllWindowEvent>().Subscribe(() => CloseRequested?.Invoke(this, EventArgs.Empty));

        DeleteMediaFromPlaylistCommand = new RelayCommand(_ => DeleteMediaFile(), _ => Playlist.MediaInfos.Any());
        RepeatMediaCommand = new RelayCommand(
            _ => Playlist[SelectedMediaIndex].Repeat = !Playlist[SelectedMediaIndex].Repeat,
            _ => Playlist.MediaInfos.Any()
        );

        CloseCommand = new RelayCommand(Close);
    }

    private void Close(object? obj)
    {
        Aggregator.GetEvent<ClosePlaylistWindowEvent>().Publish();
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
    
    private void DeleteMediaFile()
    {
        if (_playedMediaIndex == SelectedMediaIndex) return;
        Playlist.MediaInfos.RemoveAt(SelectedMediaIndex);
        var temp = Playlist.MediaInfos.AsEnumerable();
        Playlist.MediaInfos = new(temp);
    }
}