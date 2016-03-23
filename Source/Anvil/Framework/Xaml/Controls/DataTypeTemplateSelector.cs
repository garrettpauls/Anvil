using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Anvil.Framework.Xaml.Controls
{
    [ContentProperty(nameof(Templates))]
    public sealed class DataTypeTemplateSelector : DataTemplateSelector
    {
        public ObservableCollection<DataTemplate> Templates { get; } = new ObservableCollection<DataTemplate>();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var targetType = item?.GetType();
            return Templates.FirstOrDefault(template => targetType == (Type) template.DataType) ??
                   Templates.FirstOrDefault(template => template.DataType == null);
        }
    }
}
