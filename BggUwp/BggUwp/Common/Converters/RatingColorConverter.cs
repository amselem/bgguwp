using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BggUwp.Common.Converters
{
    public class RatingColorConverter : IValueConverter
    {
        // ColorScale class by James Ramsden: http://james-ramsden.com/convert-from-hsl-to-rgb-colour-codes-in-c/

        /// <summary>
        /// Hue(0-360), Saturation(0-100), Luminosity(0-100)
        /// </summary>
        class ColorScale
        {
            public static Color ColorFromHSL(double hue, double saturation, double luminosity)
            {
                hue /= 360;
                saturation /= 100;
                luminosity /= 100;
                double red = 0, green = 0, blue = 0;
                if (luminosity != 0)
                {
                    if (saturation == 0)
                        red = green = blue = luminosity;
                    else
                    {
                        double temp2;
                        if (luminosity < 0.5)
                            temp2 = luminosity * (1.0 + saturation);
                        else
                            temp2 = luminosity + saturation - (luminosity * saturation);

                        double temp1 = 2.0 * luminosity - temp2;

                        red = GetColorComponent(temp1, temp2, hue + 1.0 / 3.0);
                        green = GetColorComponent(temp1, temp2, hue);
                        blue = GetColorComponent(temp1, temp2, hue - 1.0 / 3.0);
                    }
                }
                return Color.FromArgb(255, (byte)(255 * red), (byte)(255 * green), (byte)(255 * blue));
            }

            private static double GetColorComponent(double temp1, double temp2, double temp3)
            {
                if (temp3 < 0.0)
                    temp3 += 1.0;
                else if (temp3 > 1.0)
                    temp3 -= 1.0;

                if (temp3 < 1.0 / 6.0)
                    return temp1 + (temp2 - temp1) * 6.0 * temp3;
                else if (temp3 < 0.5)
                    return temp2;
                else if (temp3 < 2.0 / 3.0)
                    return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
                else
                    return temp1;
            }
        }

        private SolidColorBrush DefaultBrush = new SolidColorBrush(Colors.Black);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // FIX NULL VALUES
            if (value == null)
                return null;

            double val;
            if (!double.TryParse(value.ToString(), out val))
                return null;

            if (val < 1 || val > 10)
                return null;

            double hue = 0.375*val*val + 5.94166303*val + 4.383353333, saturation = 42, luminance = 60;
            Double.TryParse(parameter.ToString(), out luminance);

            Color gradeColor = ColorScale.ColorFromHSL(hue, saturation, luminance); // hue(0-360), saturation(0-100), luminosity(0-100)
            return new SolidColorBrush(gradeColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("RatingColorConverter cannot convert back");
        }
    }
}
