using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Anvil.Framework.Xaml.Actions
{
    public sealed class MoveFocusAction : TriggerAction<UIElement>
    {
        public FocusNavigationDirection Direction { get; set; } = FocusNavigationDirection.Next;

        protected override void Invoke(object parameter)
        {
            AssociatedObject.MoveFocus(new TraversalRequest(Direction));
        }
    }
}