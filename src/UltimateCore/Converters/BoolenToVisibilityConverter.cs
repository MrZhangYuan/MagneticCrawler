using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace UltimateCore.Converters
{

        public class BoolenToVisibilityConverter : IValueConverter
        {
                private static BoolenToVisibilityConverter _instance = null;
                public static BoolenToVisibilityConverter Instance
                {
                        get => _instance ?? (_instance = new BoolenToVisibilityConverter());
                }
                private BoolenToVisibilityConverter()
                {

                }

                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        if (value != null
                                && (bool)value)
                        {
                                return Visibility.Visible;
                        }

                        if (parameter + "" == "H")
                        {
                                return Visibility.Hidden;

                        }

                        return Visibility.Collapsed;
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        return value != null && (Visibility)value == Visibility.Visible;
                }
        }
}
