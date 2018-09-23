using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PonyChallenge.Converters
{
    public class PonyToMazeBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ponyName = value.ToString();

            switch (ponyName)
            {
                case "Pinkie Pie": return Color.LightGreen;
                case "Applejack": return Color.LightBlue;
                case "Spike": return Color.LightPink;
                case "Rarity":
                default:
                    return Color.LightCoral;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
