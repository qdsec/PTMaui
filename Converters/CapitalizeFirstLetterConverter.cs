using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace TuProyecto.Converters
{
    public class CapitalizeFirstLetterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            string text = value.ToString();

            return char.ToUpper(text[0]) + text.Substring(1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
