using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PonyChallenge.Converters
{
    public class SnappedIntToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDouble(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)Math.Round((double)value);
        }
    }
}