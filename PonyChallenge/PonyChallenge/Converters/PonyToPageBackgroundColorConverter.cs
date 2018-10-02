using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PonyChallenge.Converters
{
    public class PonyToPageBackgroundColorConverter : IValueConverter
    {
        static public Color RarityPageBackgroundColor { get; } = Color.FromHex("5F50A2");
        static public Color PinkiePiePageBackgroundColor { get; } = Color.FromHex("ED458D");
        static public Color ApplejackPageBackgroundColor { get; } = Color.FromHex("FFF797");
        static public Color SpikePageBackgroundColor { get; } = Color.FromHex("B18FC2");

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string ponyName = value.ToString();

            switch (ponyName)
            {
                case "Pinkie Pie": return PinkiePiePageBackgroundColor;
                case "Applejack": return ApplejackPageBackgroundColor;
                case "Spike": return SpikePageBackgroundColor;
                case "Rarity":
                default:
                    return RarityPageBackgroundColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
