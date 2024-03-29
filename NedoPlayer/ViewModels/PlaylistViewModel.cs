﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using NedoPlayer.Models;
using NedoPlayer.NedoEventAggregator;
using NedoPlayer.Utils;

namespace NedoPlayer.ViewModels;

public class PlaylistViewModel : BaseViewModel
{
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
    public ICommand AddMediaCommand { get; }
    public ICommand AddFolderCommand { get; }
    public ICommand ClearPlaylistCommand { get; }
    public ICommand PlaySelectedCommand { get; }
    public ICommand DropCommand { get; }
    public ICommand DropItemCommand { get; }
    public ICommand DragOverItemCommand { get; }

    private readonly SubscriptionToken _playlistUpdateEventToken;

    public PlaylistViewModel(IEventAggregator aggregator) : base(aggregator)
    {
        _playlist = new Playlist();

        _playlistUpdateEventToken = Aggregator.GetEvent<PlaylistUpdateEvent>().Subscribe(playlist => Playlist = playlist);

        DeleteMediaFromPlaylistCommand =
            new RelayCommand(_ => Aggregator.GetEvent<DeleteMediaEvent>().Publish(SelectedMediaIndex),
                _ => Playlist.MediaInfos.Any());
        RepeatMediaCommand = new RelayCommand(
            _ => Aggregator.GetEvent<RepeatMediaEvent>().Publish(SelectedMediaIndex),
            _ => Playlist.MediaInfos.Any()
        );

        CloseCommand = new RelayCommand(Close);

        AddMediaCommand = new RelayCommand(_ => Aggregator.GetEvent<AddMediaFileEvent>().Publish());
        AddFolderCommand = new RelayCommand(_ => Aggregator.GetEvent<AddFolderEvent>().Publish());
        ClearPlaylistCommand = new RelayCommand(
            _ => Aggregator.GetEvent<ClearPlaylistEvent>().Publish(),
            _ => Playlist.MediaInfos.Any()
        );

        PlaySelectedCommand = new RelayCommand(o =>
        {
            if (o is not MouseButtonEventArgs) return;
            Aggregator.GetEvent<PlaySelectedEvent>().Publish(SelectedMediaIndex);
        },
            _ => Playlist.MediaInfos.Any());

        DropCommand = new RelayCommand(o => Aggregator.GetEvent<DropEvent>().Publish(o));

        DropItemCommand = new RelayCommand(o => Aggregator.GetEvent<DropItemEvent>().Publish((IDropInfo) o!));
        DragOverItemCommand = new RelayCommand(o => Aggregator.GetEvent<DragOverItemEvent>().Publish((IDropInfo) o!));
    }

    private void Close(object? obj)
    {
        if (obj is not Window wnd) return;
        
        Aggregator.GetEvent<ClosePlaylistWindowEvent>().Publish();
        Aggregator.GetEvent<PlaylistUpdateEvent>().Unsubscribe(_playlistUpdateEventToken);
        wnd.Close();
    }
}