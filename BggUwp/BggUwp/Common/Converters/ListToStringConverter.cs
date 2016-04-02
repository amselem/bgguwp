using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BggUwp.Common.Converters
{
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IList<string> list = value as IList<string>;

            if (list == null)
                return String.Empty;

            if (list.Count > 4)
            {
                return string.Join(Environment.NewLine, list.Take(4)) + Environment.NewLine + "...";
            }

            return string.Join(Environment.NewLine, list);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
