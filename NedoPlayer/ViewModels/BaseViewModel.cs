using System.ComponentModel;
using System.Runtime.CompilerServices;
using NedoPlayer.Annotations;
using NedoPlayer.NedoEventAggregator;

namespace NedoPlayer.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private readonly IEventAggregator _aggregator;

        protected IEventAggregator Aggregator => _aggregator;

        protected BaseViewModel(IEventAggregator aggregator) => _aggregator = aggregator;
        
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}