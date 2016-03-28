using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BggUwp.Common.Converters
{
    public class ImagePathToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string path = value as string;
            Uri uri = new Uri("ms-appx:///Assets/SampleCoverPic.jpg");
            if (!String.IsNullOrEmpty(path))
            {
                uri = new Uri("ms-appdata:///local/" + "CoverPics/" + path);
            }

            return uri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
