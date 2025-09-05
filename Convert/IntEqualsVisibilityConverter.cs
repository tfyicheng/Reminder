using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Reminder
{
    public class IntEqualsVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int actual))
                return Visibility.Collapsed;

            if (!(parameter is string expectedStr))
                return Visibility.Collapsed;

            // 支持 "0|1|2" 这种多值写法
            string[] values = expectedStr.Split('|', (char)StringSplitOptions.RemoveEmptyEntries);

            foreach (string str in values)
            {
                if (int.TryParse(str.Trim(), out int expected) && actual == expected)
                {
                    return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
