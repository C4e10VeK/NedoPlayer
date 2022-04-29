using NedoPlayer.Models;
using NedoPlayer.NedoEventAggregator;

namespace NedoPlayer.Utils;

class PlaylistUpdateEvent : PubSubEvent<Playlist> { }
class UpdatePlayedMediaIndexEvent : PubSubEvent<int> { }
class DeleteMediaEvent : PubSubEvent<int> { }
class RepeatMediaEvent : PubSubEvent<int> { }
class ClosePlaylistWindowEvent : PubSubEvent { }
class CloseAllWindowEvent : PubSubEvent { }