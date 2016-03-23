using System.Collections.ObjectModel;

using ReactiveUI;

namespace Anvil.Views.Notification
{
    public interface ITrayIconMenuItem : IReactiveObject
    {
        ReadOnlyObservableCollection<ITrayIconMenuItem> Children { get; }

        bool IsGroup { get; }

        IReactiveCommand<object> LaunchCommand { get; }

        string Name { get; }
    }
}
