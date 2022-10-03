using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using NedoPlayer.Models;

namespace NedoPlayer.Controls;

public partial class PlaylistControl : UserControl, IDropTarget
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
    
    public ICommand PlaySelectedCommand
    {
        get => (ICommand)GetValue(PlaySelectedCommandProperty);
        set => SetValue(PlaySelectedCommandProperty, value);
    }

    public static readonly DependencyProperty PlaySelectedCommandProperty =
        DependencyProperty.Register(nameof(PlaySelectedCommand), typeof(ICommand), typeof(PlaylistControl),
            new UIPropertyMetadata(null));
    
    public ICommand DragOverItemCommand
    {
        get => (ICommand)GetValue(DragOverItemCommandProperty);
        set => SetValue(DragOverItemCommandProperty, value);
    }

    public static readonly DependencyProperty DragOverItemCommandProperty =
        DependencyProperty.Register(nameof(DragOverItemCommand), typeof(ICommand), typeof(PlaylistControl),
            new UIPropertyMetadata(null));
    
    public ICommand DropItemCommand
    {
        get => (ICommand)GetValue(DropItemCommandProperty);
        set => SetValue(DropItemCommandProperty, value);
    }

    public static readonly DependencyProperty DropItemCommandProperty =
        DependencyProperty.Register(nameof(DropItemCommand), typeof(ICommand), typeof(PlaylistControl),
            new UIPropertyMetadata(null));

    public PlaylistControl()
    {
        InitializeComponent();
    }

    void IDropTarget.DragEnter(IDropInfo dropInfo)
    {
        
    }

    void IDropTarget.DragOver(IDropInfo dropInfo)
    {
        if (DragOverItemCommand is null || !DragOverItemCommand.CanExecute(dropInfo)) return;
        DragOverItemCommand.Execute(dropInfo);
    }

    void IDropTarget.DragLeave(IDropInfo dropInfo)
    {
        
    }

    void IDropTarget.Drop(IDropInfo dropInfo)
    {
        if (DropItemCommand is null || !DropItemCommand.CanExecute(dropInfo)) return;
        DropItemCommand.Execute(dropInfo);
        // var sourceItem = dropInfo.Data as MediaInfo;
        // var targetItem = dropInfo.TargetItem as MediaInfo;
        //
        // if (sourceItem == null && targetItem == null) return;
        //
        // int sourceIndex = PlaylistSource.MediaInfos.IndexOf(sourceItem);
        // int targetIndex = PlaylistSource.MediaInfos.IndexOf(targetItem);
        //
        // if (sourceIndex < targetIndex)
        // {
        //     PlaylistSource.MediaInfos.Insert(targetIndex + 1, sourceItem);
        //     PlaylistSource.MediaInfos.RemoveAt(sourceIndex);
        // }
        // else if (sourceIndex > targetIndex)
        // {
        //     int removeIndex = sourceIndex + 1;
        //     if (PlaylistSource.MediaInfos.Count + 1 < removeIndex) return;
        //     
        //     PlaylistSource.MediaInfos.Insert(targetIndex, sourceItem);
        //     PlaylistSource.MediaInfos.RemoveAt(removeIndex);
        // }
        //
        // PlaylistSource.MediaInfos = new ObservableCollection<MediaInfo>(PlaylistSource.MediaInfos);
    }
}