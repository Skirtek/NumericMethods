using System;
using System.Globalization;
using NumericMethods.Models;
using Xamarin.Forms;

namespace NumericMethods.Views.Converters
{
    public class EquationSizeToAutoSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is EquationSize) || parameter == null)
            {
                return null;
            }

            if ((EquationSize)value == EquationSize.Three && (string)parameter == "0")
            {
                return new GridLength(1, GridUnitType.Auto);
            }

            if ((EquationSize)value == EquationSize.Four && (string)parameter == "0")
            {
                return new GridLength(1, GridUnitType.Auto);
            }

            if ((EquationSize)value == EquationSize.Four && (string)parameter == "1")
            {
                return new GridLength(1, GridUnitType.Auto);
            }

            return new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
