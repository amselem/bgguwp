using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BggUwp.Common.Converters
{
    public class NullValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return NullString;

            if (String.IsNullOrEmpty(value.ToString()))
                return NullString;

            double number = 0;
            if (Double.TryParse(value.ToString(), out number))
            {
                if (number == 0)
                    return NullString;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return NullValue;

            if (String.IsNullOrEmpty(value.ToString()))
                return NullValue;

            return System.Convert.ChangeType(value, targetType);
        }

        public string NullString { get; set; }
        public int NullValue { get; set; }
    }
}
