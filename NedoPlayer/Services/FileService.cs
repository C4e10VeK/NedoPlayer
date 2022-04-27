using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using NedoPlayer.Resources;
using Application = System.Windows.Application;

namespace NedoPlayer.Services;

internal class FileService : IOService
{
    public string OpenFileDialog(string path)
    {
        using var openFileDialog = new OpenFileDialog();
        openFileDialog.InitialDirectory = path;
        openFileDialog.Filter = Resource.MediaFileFilter;
        openFileDialog.Multiselect = false;
        openFileDialog.FilterIndex = 1;
        openFileDialog.RestoreDirectory = false;

        return openFileDialog.ShowDialog() == DialogResult.OK ? openFileDialog.FileName : string.Empty;
    }

    public string OpenFolderDialog(string path)
    {
        using var openFolderDialog = new CommonOpenFileDialog();
        openFolderDialog.InitialDirectory = path;
        openFolderDialog.IsFolderPicker = true;

        return openFolderDialog.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.Ok ? openFolderDialog.FileName : string.Empty;
    }
}