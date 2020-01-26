using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UltimateCore.Converters
{
        public class ReverseVisibilityConverter : IValueConverter
        {
                private static ReverseVisibilityConverter _instance = null;
                public static ReverseVisibilityConverter Instance
                {
                        get => _instance ?? (_instance = new ReverseVisibilityConverter());
                }
                private ReverseVisibilityConverter()
                {

                }

                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        if (value != null)
                        {
                                Visibility visibility = (Visibility)value;
                                switch (visibility)
                                {
                                        case Visibility.Hidden:
                                        case Visibility.Collapsed:
                                                return Visibility.Visible;

                                        case Visibility.Visible:
                                                if (parameter + "" == "H")
                                                {
                                                        return Visibility.Hidden;
                                                }
                                                break;
                                }
                        }
                        return Visibility.Collapsed;
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        return value != null && (Visibility)value == Visibility.Visible;
                }
        }
}
