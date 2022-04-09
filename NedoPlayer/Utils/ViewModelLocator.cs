using NedoPlayer.NedoEventAggregator;
using NedoPlayer.ViewModels;

namespace NedoPlayer.Utils;

public class ViewModelLocator
{
    private static readonly EventAggregator Aggregator = new EventAggregator();
    
    private MainViewModel? _mainViewModel;
    private ControlViewModel? _controlViewModel;

    public MainViewModel MainViewModel => _mainViewModel ??= new MainViewModel(Aggregator);
    public ControlViewModel ControlViewModel => _controlViewModel ??= new ControlViewModel(Aggregator);
}