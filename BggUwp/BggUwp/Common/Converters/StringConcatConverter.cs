using BggUwp.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BggUwp.Common.Converters
{
    public class StringConcatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var searchResult = value as SearchResultDataItem;
            var yearDisplay = String.Concat(" (", searchResult.YearPublished.ToString(), ")");

            if (searchResult.YearPublished == -1)
                yearDisplay = "";

            return String.Concat(searchResult.BoardGameName, yearDisplay);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
