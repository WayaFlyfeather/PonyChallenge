using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace PonyChallenge.UWP.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public string StringFormat { get; set; } = "{0}";

        public StringFormatConverter(string stringFormat)
        {
            StringFormat = stringFormat;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string format = parameter is string && !String.IsNullOrEmpty((string)parameter) ? parameter.ToString() : StringFormat;
            return String.Format(format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
