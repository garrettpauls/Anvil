using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Anvil.Framework.Xaml.Behaviors
{
    public sealed class TreeViewSelectedItemBehavior : Behavior<TreeView>
    {
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem", typeof(object), typeof(TreeViewSelectedItemBehavior),
            new PropertyMetadata(default(object)));

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        private void _HandleSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
        }

        protected override void OnAttached()
        {
            AssociatedObject.SelectedItemChanged += _HandleSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectedItemChanged -= _HandleSelectedItemChanged;
        }
    }
}