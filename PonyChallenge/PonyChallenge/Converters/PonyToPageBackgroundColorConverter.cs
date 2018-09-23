using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PonyChallenge.Converters
{
    public class PonyToPageBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string ponyName = value.ToString();

            switch (ponyName)
            {
                case "Pinkie Pie": return Color.DeepPink;
                case "Applejack": return Color.SandyBrown;
                case "Spike": return Color.LawnGreen;
                case "Rarity":
                default:
                    return Color.BlueViolet;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
