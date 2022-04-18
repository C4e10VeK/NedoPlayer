using System;
using NedoPlayer.NedoEventAggregator;

namespace NedoPlayer.Utils;

class PlayPauseState : PubSubEvent { }
class MaximizePlayer : PubSubEvent<bool> { }
class ChangeVolume : PubSubEvent<double> { }
class MuteAudio : PubSubEvent<bool> { }