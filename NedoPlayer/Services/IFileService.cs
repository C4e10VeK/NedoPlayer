using NedoPlayer.Models;

namespace NedoPlayer.Services;

public interface IFileService
{
    public string OpenFileDialog(string path);

    public string OpenFolderDialog(string path);

    public void SavePlaylist(Playlist playlist);
    public Playlist OpenPlaylist(string path);
}