using System;
using System.Globalization;
using audioteca.Models.Api;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca.Helpers
{
    public class GetDuration : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var abook = (value as AudioBookDetailResult);
            if (abook != null)
            {
                return $"Duracion: {abook.LengthHours} horas {abook.LengthMins} minutos";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
