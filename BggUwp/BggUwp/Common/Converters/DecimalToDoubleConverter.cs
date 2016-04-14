using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BggUwp.Common.Converters
{
    public class DecimalToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double number = 0;
            double.TryParse(value.ToString(), out number);
            return number;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            decimal number = 0;
            Decimal.TryParse(value.ToString(), out number);
            return number;
        }
    }
}
