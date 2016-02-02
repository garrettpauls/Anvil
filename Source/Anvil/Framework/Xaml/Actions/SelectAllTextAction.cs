using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Anvil.Framework.Xaml.Actions
{
    public sealed class SelectAllTextAction : TriggerAction<TextBox>
    {
        protected override void Invoke(object parameter)
        {
            AssociatedObject?.SelectAll();
        }
    }
}