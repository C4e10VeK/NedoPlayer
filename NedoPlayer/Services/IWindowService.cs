using System.Windows;

namespace NedoPlayer.Services;

public interface IWindowService
{
    public void OpenWindow<T>(Window? parent = null) where T: Window, new();
    public bool? OpenDialogWindow<T>(Window? parent = null) where T: Window, new();
}