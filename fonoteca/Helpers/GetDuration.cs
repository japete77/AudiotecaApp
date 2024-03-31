using fonoteca.Models.Api;
using System.Globalization;

namespace fonoteca.Helpers
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
