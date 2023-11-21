using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Spray_Paint_Application.Converter
{
    public class SelectedToBorderBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Assuming value is a boolean indicating selection
            bool isSelected = (bool)value;
            return isSelected ? new SolidColorBrush(Colors.Gold) : new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported.");
        }
    }
}
