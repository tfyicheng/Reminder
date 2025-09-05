using System;
using System.Globalization;
using System.Windows.Data;

namespace Reminder
{
    public class TimeFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[1] == null)
                return string.Empty;

            string timeStr = values[0].ToString();
            int type = 0;

            if (!int.TryParse(values[1].ToString(), out type))
                return timeStr;

            if (DateTime.TryParse(timeStr, out DateTime time))
            {
                switch (type)
                {
                    case 1:
                        return time.ToString("yyyy-MM-dd HH:mm:ss");
                    case 2:
                    case 3:
                    case 5:
                        return time.ToString("HH:mm:ss");
                    case 4:
                    case 6:
                        return time.ToString("dd HH:mm:ss");
                    default:
                        return time.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }

            return timeStr;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
