using NedoPlayer.Models;
using NedoPlayer.NedoEventAggregator;

namespace NedoPlayer.Utils;

class PlaylistUpdateEvent : PubSubEvent<Playlist> { }
class UpdatePlayedMediaIndex : PubSubEvent<int> { }
class ClosePlaylistWindowEvent : PubSubEvent { }
class CloseAllWindowEvent : PubSubEvent { }