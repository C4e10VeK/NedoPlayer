using System;
using System.Collections.ObjectModel;
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
    
    public PlaylistControl()
    {
        InitializeComponent();
    }

    private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not ListView listView)
            return;
        
        var gridView = listView.View as GridView;

        var workWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
        var col1 = 0.6;
        var col2 = 0.45;

        gridView.Columns[0].Width = workWidth * col1;
        gridView.Columns[1].Width = workWidth * col2;
    }
}