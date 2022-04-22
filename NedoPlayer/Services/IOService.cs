using NedoPlayer.ViewModels;

namespace NedoPlayer.Services
{
    internal interface IOService
    {
        public string OpenFileDialog(BaseViewModel parent, string path);
    }
}
