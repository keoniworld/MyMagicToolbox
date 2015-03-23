using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MyMagicCollection.wpf.Converter
{
    public class AdvancedBooleanToVisibilityConverter : IValueConverter
    {
        public AdvancedBooleanToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue= Visibility.Collapsed;
        }

        public Visibility TrueValue { get; set; }

        public Visibility FalseValue { get; set; }

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            return value != null
                ? (bool)value ? TrueValue : FalseValue
                : FalseValue;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}