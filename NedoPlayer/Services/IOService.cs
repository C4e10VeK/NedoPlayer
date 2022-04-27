namespace NedoPlayer.Services;

public interface IOService
{
    public string OpenFileDialog(string path);

    public string OpenFolderDialog(string path);
}