using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;

namespace Anvil.Theme
{
    /// <summary>
    ///     Based on https://gist.github.com/usagirei/a2cfe768e4cf5fb34694
    /// </summary>
    public sealed class AeroColorProvider : DependencyObject, IDisposable
    {
        public static readonly DependencyProperty Accent1Property = DependencyProperty.Register
            (nameof(Accent), typeof(Color), typeof(AeroColorProvider), new PropertyMetadata(default(Color)));

        public static readonly DependencyProperty Accent2Property = DependencyProperty.Register
            (nameof(Accent2), typeof(Color), typeof(AeroColorProvider), new PropertyMetadata(default(Color)));

        public static readonly DependencyProperty Accent3Property = DependencyProperty.Register
            (nameof(Accent3), typeof(Color), typeof(AeroColorProvider), new PropertyMetadata(default(Color)));

        public static readonly DependencyProperty Accent4Property = DependencyProperty.Register
            (nameof(Accent4), typeof(Color), typeof(AeroColorProvider), new PropertyMetadata(default(Color)));

        public static readonly DependencyProperty HighlightProperty = DependencyProperty.Register
            (nameof(Highlight), typeof(Color), typeof(AeroColorProvider), new PropertyMetadata(default(Color)));

        private readonly IDisposable mUpdateColorsSubscription;

        public AeroColorProvider()
        {
            mUpdateColorsSubscription = Observable
                .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => SystemParameters.StaticPropertyChanged += handler,
                    handler => SystemParameters.StaticPropertyChanged -= handler)
                .Where(evt => evt.EventArgs.PropertyName == nameof(SystemParameters.WindowGlassColor))
                .Subscribe(evt => _UpdateColors());

            _UpdateColors();
        }

        public Color Accent
        {
            get { return (Color) GetValue(Accent1Property); }
            set { SetValue(Accent1Property, value); }
        }

        public Color Accent2
        {
            get { return (Color) GetValue(Accent2Property); }
            set { SetValue(Accent2Property, value); }
        }

        public Color Accent3
        {
            get { return (Color) GetValue(Accent3Property); }
            set { SetValue(Accent3Property, value); }
        }

        public Color Accent4
        {
            get { return (Color) GetValue(Accent4Property); }
            set { SetValue(Accent4Property, value); }
        }

        public Color Highlight
        {
            get { return (Color) GetValue(HighlightProperty); }
            set { SetValue(HighlightProperty, value); }
        }

        private void _HandleSystemParameterChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(SystemParameters.WindowGlassColor))
            {
                _UpdateColors();
            }
        }

        private void _UpdateColors()
        {
            var aero = SystemParameters.WindowGlassColor;

            Highlight = _SetIntensity(aero, 1.1f);
            Accent = _SetIntensity(aero, 0.95f);
            Accent2 = _SetIntensity(aero, 0.8f);
            Accent3 = _SetIntensity(aero, 0.65f);
            Accent4 = _SetIntensity(aero, 0.5f);
        }

        public void Dispose()
        {
            mUpdateColorsSubscription.Dispose();
        }

        private static byte _ClipByte(float input)
        {
            return (byte) ((input < 0 ? 0 : input > 1 ? 1 : input)*255);
        }

        private static Color _SetIntensity(Color input, float power)
        {
            var newR = (input.R/255f)*power;
            var newG = (input.G/255f)*power;
            var newB = (input.B/255f)*power;

            return Color.FromRgb(_ClipByte(newR), _ClipByte(newG), _ClipByte(newB));
        }
    }
}
