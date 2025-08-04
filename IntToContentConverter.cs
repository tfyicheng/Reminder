using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace Reminder
{
    public class IntToContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int intValue && intValue == 1 ? "活动" : "停止";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Content 不需要反向绑定
            return DependencyProperty.UnsetValue;
        }
    }
}