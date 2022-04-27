using System;
using System.Collections.Generic;
using System.Windows;

namespace NedoPlayer.Services;

public class WindowStateService : IStateService
{
    private struct CurrentState
    {
        public WindowState WndState { get; set; }
        public WindowStyle WndStyle { get; set; }
        public double WndWidth { get; set; }
        public double WndHeight { get; set; }
    }

    private readonly Dictionary<Type, CurrentState> _states;
    
    public WindowStateService() => _states = new Dictionary<Type, CurrentState>();

    public void SaveState(object? o)
    {
        if (o is not Window window) 
            return;
        
        _states[o.GetType()] = new CurrentState
        {
            WndHeight = window.Height, 
            WndWidth = window.Width, 
            WndState = window.WindowState,
            WndStyle = window.WindowStyle
        };
    }

    public void LoadState(object? o)
    {
        if (o is not Window window)
            return;

        var state = _states[o.GetType()];
        window.Height = state.WndHeight;
        window.Width = state.WndWidth;
        window.WindowState = state.WndState;
        window.WindowStyle = state.WndStyle;
    }
}