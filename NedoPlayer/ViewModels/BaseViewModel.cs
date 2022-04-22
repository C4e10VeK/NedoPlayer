using System.ComponentModel;
using System.Runtime.CompilerServices;
using NedoPlayer.Annotations;
using NedoPlayer.NedoEventAggregator;

namespace NedoPlayer.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected IEventAggregator Aggregator { get; }

        protected BaseViewModel(IEventAggregator aggregator) => Aggregator = aggregator;
        
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}