using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Anvil.Framework.Xaml.Converters
{
    public sealed class VisibilityOrMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var visibilities = new HashSet<Visibility>(values.Cast<Visibility>());

            if(visibilities.Contains(Visibility.Visible))
            {
                return Visibility.Visible;
            }

            if(visibilities.Contains(Visibility.Hidden))
            {
                return Visibility.Hidden;
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Enumerable.Repeat(DependencyProperty.UnsetValue, targetTypes.Length).ToArray();
        }
    }
}
