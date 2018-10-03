using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PonyChallenge.Converters
{
    public class PonyToWallColorConverter : IValueConverter
    {
        static public Color RarityWallColor { get; } = Color.FromHex("490A1C");
        static public Color PinkiePieWallColor { get; } = Color.FromHex("17244C");
        static public Color ApplejackWallColor { get; } = Color.FromHex("0A3325");
        static public Color SpikeWallColor { get; } = Color.FromHex("02330C");

        static public Color GetWallColorForPony(string ponyName)
        {
            switch (ponyName)
            {
                case "Pinkie Pie": return PinkiePieWallColor;
                case "Applejack": return ApplejackWallColor;
                case "Spike": return SpikeWallColor;
                case "Rarity":
                default:
                    return RarityWallColor;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => GetWallColorForPony(value.ToString());

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
