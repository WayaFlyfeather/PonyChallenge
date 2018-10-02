using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PonyChallenge.Converters
{
    public class PonyToMazeBackgroundColorConverter : IValueConverter
    {
        static public Color RarityMazeBackgroundColor { get; } = Color.LightCoral;
        static public Color PinkiePieMazeBackgroundColor { get; } = Color.LightGreen;
        static public Color ApplejackMazeBackgroundColor { get; } = Color.LightBlue;
        static public Color SpikeMazeBackgroundColor { get; } = Color.LightPink;

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
