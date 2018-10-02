using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PonyChallenge.Converters
{
    public class PonyToMazeBackgroundColorConverter : IValueConverter
    {
        static public Color RarityMazeBackgroundColor { get; } = Color.FromHex("38C14C");
        static public Color PinkiePieMazeBackgroundColor { get; } = Color.FromHex("D2EA67");
        static public Color ApplejackMazeBackgroundColor { get; } = Color.FromHex("F377FF");
        static public Color SpikeMazeBackgroundColor { get; } = Color.FromHex("FF3D5D");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ponyName = value.ToString();

            switch (ponyName)
            {
                case "Pinkie Pie": return PinkiePieMazeBackgroundColor;
                case "Applejack": return ApplejackMazeBackgroundColor;
                case "Spike": return SpikeMazeBackgroundColor;
                case "Rarity":
                default:
                    return RarityMazeBackgroundColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
