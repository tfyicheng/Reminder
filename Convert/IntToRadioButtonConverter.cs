using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace Reminder
{
    public class IntToRadioButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value = Type (int)
            // parameter = 期望的值，如 "1"、"2"
            if (value is int intValue && parameter is string strParam && int.TryParse(strParam, out int paramValue))
            {
                return intValue == paramValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value = IsChecked (bool)
            // parameter = 对应的 Type 值，如 "1"
            if (value is bool isChecked && isChecked && parameter is string strParam && int.TryParse(strParam, out int paramValue))
            {
                return paramValue;
            }
            // 如果不是选中状态，不要返回值（避免多次触发）
            return DependencyProperty.UnsetValue;
        }
    }
}