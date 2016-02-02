using System.Windows;
using System.Windows.Controls;

namespace Anvil.Framework.Xaml.Controls.Extensions
{
    public static class TextBoxEx
    {
        public static readonly DependencyProperty SelectAllOnFocusProperty = DependencyProperty.RegisterAttached(
            "SelectAllOnFocus", typeof(bool), typeof(TextBoxEx), new PropertyMetadata(false, _HandleSelectAllOnFocusChanged));

        private static void _HandleSelectAllOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBox;
            if(tb != null)
            {
                if(GetSelectAllOnFocus(d))
                {
                    tb.GotFocus += _SelectAllOnTextBoxFocus;
                }
                else
                {
                    tb.GotFocus -= _SelectAllOnTextBoxFocus;
                }
            }
        }

        private static void _SelectAllOnTextBoxFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            tb?.SelectAll();
        }

        public static bool GetSelectAllOnFocus(DependencyObject element)
        {
            return (bool) element.GetValue(SelectAllOnFocusProperty);
        }

        public static void SetSelectAllOnFocus(DependencyObject element, bool value)
        {
            element.SetValue(SelectAllOnFocusProperty, value);
        }
    }
}