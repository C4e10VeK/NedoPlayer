using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NedoPlayer.Models;

namespace NedoPlayer.Controls;

public partial class PlaylistControl
{
    public Playlist PlaylistSource
    {
        get => (Playlist) GetValue(PlaylistSourceProperty);
        set => SetValue(PlaylistSourceProperty, value);
    }

    public static readonly DependencyProperty PlaylistSourceProperty =
        DependencyProperty.Register("PlaylistSource", typeof(Playlist), typeof(PlaylistControl),
            new PropertyMetadata(null));
    
    public MediaInfo SelectedMedia
    {
        get => (MediaInfo) GetValue(SelectedMediaProperty);
        set => SetValue(SelectedMediaProperty, value);
    }

    public static readonly DependencyProperty SelectedMediaProperty =
        DependencyProperty.Register("SelectedMedia", typeof(MediaInfo), typeof(PlaylistControl),
            new PropertyMetadata(null));
    
    public int SelectedMediaIndex
    {
        get => (int) GetValue(SelectedMediaIndexProperty);
        set => SetValue(SelectedMediaIndexProperty, value);
    }

    public static readonly DependencyProperty SelectedMediaIndexProperty =
        DependencyProperty.Register("SelectedMediaIndex", typeof(int), typeof(PlaylistControl),
            new PropertyMetadata(-1));

    public ICommand RepeatMediaCommand
    {
        get => (ICommand) GetValue(RepeatMediaCommandProperty);
        set => SetValue(RepeatMediaCommandProperty, value);
    }

    public static readonly DependencyProperty RepeatMediaCommandProperty =
        DependencyProperty.Register(nameof(RepeatMediaCommand), typeof(ICommand), typeof(PlaylistControl),
            new UIPropertyMetadata(null));
    
    public ICommand DeleteMediaCommand
    {
        get => (ICommand) GetValue(DeleteMediaCommandProperty);
        set => SetValue(DeleteMediaCommandProperty, value);
    }

    public static readonly DependencyProperty DeleteMediaCommandProperty =
        DependencyProperty.Register(nameof(DeleteMediaCommand), typeof(ICommand), typeof(PlaylistControl),
            new UIPropertyMetadata(null));

    public ICommand ClearPlaylistCommand
    {
        get => (ICommand)GetValue(ClearPlaylistCommandProperty);
        set => SetValue(ClearPlaylistCommandProperty, value);
    }

    public static readonly DependencyProperty ClearPlaylistCommandProperty =
        DependencyProperty.Register(nameof(ClearPlaylistCommand), typeof(ICommand), typeof(PlaylistControl),
            new UIPropertyMetadata(null));

    public ICommand AddMediaCommand
    {
        get => (ICommand)GetValue(AddMediaCommandProperty);
        set => SetValue(AddMediaCommandProperty, value);
    }

    public static readonly DependencyProperty AddMediaCommandProperty =
        DependencyProperty.Register(nameof(AddMediaCommand), typeof(ICommand), typeof(PlaylistControl),
            new UIPropertyMetadata(null));

    public ICommand AddFolderCommand
    {
        get => (ICommand)GetValue(AddFolderCommandProperty);
        set => SetValue(AddFolderCommandProperty, value);
    }

    public static readonly DependencyProperty AddFolderCommandProperty =
        DependencyProperty.Register(nameof(AddFolderCommand), typeof(ICommand), typeof(PlaylistControl),
            new UIPropertyMetadata(null));

    public PlaylistControl()
    {
        InitializeComponent();
    }
}