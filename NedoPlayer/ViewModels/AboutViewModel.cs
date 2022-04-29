using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Input;
using NedoPlayer.NedoEventAggregator;
using NedoPlayer.Utils;

namespace NedoPlayer.ViewModels;

public class AboutViewModel : BaseViewModel
{
    private string _osName;
    public string OsName => _osName;

    private string _appVersion;
    public string AppVersion => _appVersion;

    private string _architecture;
    public string Architecture => _architecture;

    private string _nicknameCreator;
    public string NicknameCreator => _nicknameCreator;

    public event EventHandler? CloseRequested;

    public ICommand CloseCommand { get; }
    
    public AboutViewModel(IEventAggregator aggregator) : base(aggregator)
    {
        var os = Environment.OSVersion;
        _osName = os.VersionString;

        _appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        _architecture = RuntimeInformation.ProcessArchitecture.ToString("G");
        _nicknameCreator = "Che10VeK";

        CloseCommand = new RelayCommand(_ => CloseRequested?.Invoke(this, EventArgs.Empty));
    }
}