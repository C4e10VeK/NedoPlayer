using System.Windows;
using MahApps.Metro.Controls;

namespace NedoPlayer.Services;

public class WindowStateService
{
    private struct CurrentState
    {
        public WindowState WndState { get; set; }
        public WindowStyle WndStyle { get; set; }
        public double WndWidth { get; set; }
        public double WndHeight { get; set; }
    }

    private static CurrentState _state;
    
    public static void SaveCurrentWindowState(MetroWindow window)
    {
        _state.WndState = window.WindowState;
        _state.WndStyle = window.WindowStyle;
        _state.WndWidth = window.Width;
        _state.WndHeight = window.Height;
    }
    
    public static void LoadCurrentWindowState(ref MetroWindow window)
    {
        window.WindowState = _state.WndState;
        window.WindowStyle = _state.WndStyle;
        window.Width = _state.WndWidth;
        window.Height = _state.WndHeight;
    }
}