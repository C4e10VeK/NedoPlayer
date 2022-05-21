using NedoPlayer.Models;

namespace NedoPlayer.Services;

public interface IFileService
{
    public string OpenFileDialog(string path);
    public string OpenFileDialog(string path, string filter);

    public string OpenFolderDialog(string path);

    public void SavePlaylist(Playlist playlist, string startPath);
    public Playlist OpenPlaylist(string path);
}