using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PonyChallenge.Converters
{
    public class MovesPerSecondPowerToSpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int pow = System.Convert.ToInt32(value);
            switch (pow)
            {
                case -1: return "2 sec";
                case 0: return "1 sec";
                case 1: return "1/2 sec";
                case 2: return "1/4 sec";
                case 3:
                default: return "1/8 sec";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
