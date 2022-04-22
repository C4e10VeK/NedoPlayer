using NedoPlayer.ViewModels;
using System.Windows.Forms;
using NedoPlayer.Resources;

namespace NedoPlayer.Services;

internal class FileService : IOService
{
    public string OpenFileDialog(BaseViewModel parent, string path)
    {
        using var openFileDialog = new OpenFileDialog();
        openFileDialog.InitialDirectory = path;
        openFileDialog.Filter = Resource.MediaFileFilter;
        openFileDialog.Multiselect = false;
        openFileDialog.FilterIndex = 1;
        openFileDialog.RestoreDirectory = false;

        return openFileDialog.ShowDialog() == DialogResult.OK ? openFileDialog.FileName : string.Empty;
    }
}