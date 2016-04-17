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

        class ColorScale
        {
            public static Color ColorFromHSL(double h, double s, double l)
            {
                h /= 360;
                s /= 100;
                l /= 100;
                double r = 0, g = 0, b = 0;
                if (l != 0)
                {
                    if (s == 0)
                        r = g = b = l;
                    else
                    {
                        double temp2;
                        if (l < 0.5)
                            temp2 = l * (1.0 + s);
                        else
                            temp2 = l + s - (l * s);

                        double temp1 = 2.0 * l - temp2;

                        r = GetColorComponent(temp1, temp2, h + 1.0 / 3.0);
                        g = GetColorComponent(temp1, temp2, h);
                        b = GetColorComponent(temp1, temp2, h - 1.0 / 3.0);
                    }
                }
                return Color.FromArgb(255, (byte)(255 * r), (byte)(255 * g), (byte)(255 * b));
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

            Color gradeColor = ColorScale.ColorFromHSL(hue, saturation, luminance); // hue(0-360), saturation(0-100), luminance(0-100)
            return new SolidColorBrush(gradeColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("RatingColorConverter cannot convert back");
        }
    }
}
