using NedoPlayer.Models;
using NedoPlayer.NedoEventAggregator;

namespace NedoPlayer.Utils;

class PlaylistUpdateEvent : PubSubEvent<Playlist> { }
class DeleteMediaEvent : PubSubEvent<int> { }
class RepeatMediaEvent : PubSubEvent<int> { }
class ClosePlaylistWindowEvent : PubSubEvent { }
class AddMediaFileEvent : PubSubEvent { }
class AddFolderEvent : PubSubEvent { }
class ClearPlaylistEvent : PubSubEvent { }