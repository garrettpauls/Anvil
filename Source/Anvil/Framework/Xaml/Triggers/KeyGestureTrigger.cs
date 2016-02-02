using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Anvil.Framework.Xaml.Triggers
{
    public sealed class KeyGestureTrigger : TriggerBase<UIElement>
    {
        public KeyGesture Gesture { get; set; }

        private void _HandleKeyDown(object sender, KeyEventArgs args)
        {
            if(Gesture?.Matches(AssociatedObject, args) ?? false)
            {
                InvokeActions(null);
            }
        }

        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += _HandleKeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= _HandleKeyDown;
        }
    }
}