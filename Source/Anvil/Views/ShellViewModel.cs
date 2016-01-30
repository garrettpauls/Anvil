using ReactiveUI;

namespace Anvil.Views
{
    public sealed class ShellViewModel : ReactiveObject, IScreen
    {
        public ShellViewModel(RoutingState router)
        {
            Router = router;
        }

        public RoutingState Router { get; }
    }
}