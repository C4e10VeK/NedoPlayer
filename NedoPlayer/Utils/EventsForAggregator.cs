using System;
using NedoPlayer.NedoEventAggregator;

namespace NedoPlayer.Utils;

class MaximizePlayer : PubSubEvent { }
class ChangeVolume : PubSubEvent<double> { }