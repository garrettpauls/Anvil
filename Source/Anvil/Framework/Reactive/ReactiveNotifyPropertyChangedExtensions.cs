using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Anvil.Framework.Reactive
{
    public static class ReactiveNotifyPropertyChangedExtensions
    {
        public static IObservable<PropertyChangedNotification<T>> ObservePropertyChanged<T>(this T source)
            where T : INotifyPropertyChanged
        {
            return Observable
                .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => source.PropertyChanged += handler,
                    handler => source.PropertyChanged -= handler)
                .Select(evt => new PropertyChangedNotification<T>((T) evt.Sender, evt.EventArgs.PropertyName));
        }
    }

    public sealed class PropertyChangedNotification<T>
    {
        public PropertyChangedNotification(T source, string propertyName)
        {
            Source = source;
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        public T Source { get; }
    }
}