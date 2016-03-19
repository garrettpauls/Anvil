using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Anvil.Controls
{
    [ContentProperty(nameof(Content))]
    public class GeometryButton : ContentControl
    {
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof(object), typeof(GeometryButton), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(GeometryButton), new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register(
            "Geometry", typeof(Geometry), typeof(GeometryButton), new PropertyMetadata(default(Geometry)));

        static GeometryButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeometryButton), new FrameworkPropertyMetadata(typeof(GeometryButton)));
        }

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public Geometry Geometry
        {
            get { return (Geometry) GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
        }
    }
}
