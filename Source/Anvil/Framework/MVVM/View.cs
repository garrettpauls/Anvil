using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Anvil.Framework.ComponentModel;

using ReactiveUI;

namespace Anvil.Framework.MVVM
{
    public interface IView : IViewFor
    {
    }

    public abstract class View<TViewModel> : UserControl, IViewFor<TViewModel>, IView, IDisposable
        where TViewModel : class
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(TViewModel), typeof(View<TViewModel>), new PropertyMetadata(default(TViewModel)));

        protected View()
        {
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext).TrackWith(Disposables);
            this.WhenActivated(_WhenActivated).TrackWith(Disposables);
        }

        protected DisposableTracker Disposables { get; } = new DisposableTracker();

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TViewModel) value; }
        }

        public TViewModel ViewModel
        {
            get { return (TViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private IEnumerable<IDisposable> _WhenActivated()
        {
            MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            return Enumerable.Empty<IDisposable>();
        }

        public void Dispose()
        {
            Disposables.Dispose();
        }
    }
}