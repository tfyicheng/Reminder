using System;
using System.Globalization;
using System.Windows.Data;


namespace Reminder
{
    public class IntToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // int Status → IsChecked: Status == 1
            return value is int intValue && intValue == 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // IsChecked → Status: true → 1, false → 0
            return value is bool boolValue && boolValue ? 1 : 0;
        }
    }
}