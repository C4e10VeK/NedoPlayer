using System;
using System.Windows;
using System.Windows.Input;
using NedoPlayer.NedoEventAggregator;
using NedoPlayer.Utils;

namespace NedoPlayer.ViewModels;

public class ControlViewModel : BaseViewModel
{
    public ICommand MaximizeCommand { get; }
    private WindowState _wndState;
    private WindowStyle _wndStyle;
    private double _wndWidth;
    private double _wndHeight;

    private double _volume;
    public double Volume
    {
        get => _volume;
        set
        {
            _volume = value;
            OnPropertyChanged(nameof(Volume));
            Aggregator.GetEvent<ChangeVolume>().Publish(_volume / 100);
        }
    }

    public ControlViewModel(IEventAggregator aggregator) : base(aggregator)
    {
        MaximizeCommand = new RelayCommand(Maximize);
        _volume = 100.0d;
    }

    private void Maximize(object? s)
    {
        var wnd = Application.Current.MainWindow;
        if (wnd is null) return;
        
        Aggregator.GetEvent<MaximizePlayer>().Publish();

        if (wnd.WindowState != WindowState.Maximized)
        {
            _wndState = wnd.WindowState;
            _wndStyle = wnd.WindowStyle;
            _wndWidth = wnd.Width;
            _wndHeight = wnd.Height;
            
            wnd.WindowState = WindowState.Maximized;
            wnd.WindowStyle = WindowStyle.None;
            return;
        }

        wnd.WindowState = _wndState;
        wnd.WindowStyle = _wndStyle;
        wnd.Width = _wndWidth;
        wnd.Height = _wndHeight;
    }
}