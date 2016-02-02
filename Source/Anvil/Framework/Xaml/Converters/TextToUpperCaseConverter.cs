using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Anvil.Framework.Xaml.Converters
{
    public sealed class TextToUpperCaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;

            if(str == null)
            {
                return DependencyProperty.UnsetValue;
            }

            return str.ToUpper(culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}