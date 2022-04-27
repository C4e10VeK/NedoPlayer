using System.Windows;
using System.Windows.Controls;
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
    
    public PlaylistControl()
    {
        InitializeComponent();
    }

    private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not ListView {View: GridView gridView} listView)
            return;

        var workWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
        var col1 = 0.6;
        var col2 = 0.45;

        gridView.Columns[0].Width = workWidth * col1;
        gridView.Columns[1].Width = workWidth * col2;
    }
}