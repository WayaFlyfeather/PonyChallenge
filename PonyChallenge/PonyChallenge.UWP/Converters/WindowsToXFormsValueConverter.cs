using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace PonyChallenge.UWP.Converters
{
    public class WindowsToXFormsValueConverter : IValueConverter
    {
        public Xamarin.Forms.IValueConverter XamarinFormsValueConverter { get; set; } = null;

        public WindowsToXFormsValueConverter(Xamarin.Forms.IValueConverter xamarinFormsValueConverter)
        {
            XamarinFormsValueConverter = xamarinFormsValueConverter;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //note: not implementing language/Culture Conversion!
            return XamarinFormsValueConverter?.Convert(value, targetType, parameter, null);    
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            //note: not implementing language/Culture Conversion!
            return XamarinFormsValueConverter?.ConvertBack(value, targetType, parameter, null);
        }
    }
}
