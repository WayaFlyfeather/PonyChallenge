using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PonyChallenge.Converters
{
    public class PonyToWallColorConverter : IValueConverter
    {
        static public Color RarityWallColor { get; } = Color.Black;
        static public Color PinkiePieWallColor { get; } = Color.Black;
        static public Color AppleJackWallColor { get; } = Color.Black;
        static public Color SpikeWallColor { get; } = Color.Black;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ponyName = value.ToString();

            switch (ponyName)
            {
                case "Pinkie Pie": return PinkiePieWallColor;
                case "Applejack": return AppleJackWallColor;
                case "Spike": return SpikeWallColor;
                case "Rarity":
                default:
                    return RarityWallColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
