using System;

using ReactiveUI;

namespace Anvil.Framework.MVVM
{
    public abstract class RoutableViewModel : DisposableViewModel, IRoutableViewModel, IDisposable
    {
        protected RoutableViewModel(string urlPathSegment, IScreen hostScreen)
        {
            UrlPathSegment = urlPathSegment;
            HostScreen = hostScreen;
        }

        public IScreen HostScreen { get; }

        public string UrlPathSegment { get; }
    }
}