using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MagneticCrawler.Converters
{
        internal class BooleanToVisibilityConverter : IValueConverter
        {
                private static readonly Lazy<BooleanToVisibilityConverter> instance = new Lazy<BooleanToVisibilityConverter>(true);
                private const string MODE_HIDDEN = "Hidden";
                private const string MODE_NEGATE = "Negate";
                private const string MODE_NEGATE_HIDDEN = "NegateHidden";

                public static BooleanToVisibilityConverter Instance
                {
                        get
                        {
                                return BooleanToVisibilityConverter.instance.Value;
                        }
                }

                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        string str = parameter as string;
                        bool flag = value != null && value is bool && (bool)value;
                        Visibility visibility = Visibility.Visible;
                        if (str == "Negate" || str == "NegateHidden")
                                flag = !flag;
                        if (!flag)
                                visibility = str == "Hidden" || str == "NegateHidden" ? Visibility.Hidden : Visibility.Collapsed;
                        return (object)visibility;
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        throw new NotImplementedException();
                }
        }
}
