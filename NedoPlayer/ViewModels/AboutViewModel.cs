using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using NedoPlayer.NedoEventAggregator;
using NedoPlayer.Utils;

namespace NedoPlayer.ViewModels;

public class AboutViewModel : BaseViewModel
{
    public string OsName { get; }

    public string AppVersion { get; }

    public string Architecture { get; }

    public string NicknameCreator { get; }

    public ICommand CloseCommand { get; }
    public ICommand OpenLinkCommand { get; }
    
    public AboutViewModel(IEventAggregator aggregator) : base(aggregator)
    {
        var os = Environment.OSVersion;
        OsName = os.VersionString;

        AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        Architecture = RuntimeInformation.ProcessArchitecture.ToString("G");
        NicknameCreator = "Che10VeK";

        CloseCommand = new RelayCommand(Close);
        OpenLinkCommand = new RelayCommand(OpenLink);
    }

    private void OpenLink(object? obj)
    {
        if (obj is not RequestNavigateEventArgs args) return;
        Process.Start(new ProcessStartInfo(args.Uri.AbsoluteUri));
        args.Handled = true;
    }

    private void Close(object? s)
    {
        if (s is not Window w) return;
        w.Close();
    }
}