using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MagneticCrawler.Converters
{
        public class SizeToBackgroundConverter : IValueConverter
        {
                public static SizeToBackgroundConverter Instance
                {
                        get;
                }
                static SizeToBackgroundConverter()
                {
                        Instance = new SizeToBackgroundConverter();
                }

                private SolidColorBrush _default = new SolidColorBrush(Colors.NavajoWhite);
                private SolidColorBrush _GB = new SolidColorBrush(Colors.OrangeRed);
                private SolidColorBrush _MB = new SolidColorBrush(Colors.DarkOrange);
                private SolidColorBrush _KB = new SolidColorBrush(Colors.Orange);

                private SizeToBackgroundConverter()
                {

                }
                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        string text = (value + "").ToUpper();
                        if (text.Contains("GB")
                                || text.Contains("G"))
                        {
                                return this._GB;
                        }
                        else if (text.Contains("MB")
                                || text.Contains("M"))
                        {
                                return this._MB;
                        }
                        else if (text.Contains("KB")
                                || text.Contains("K"))
                        {
                                return this._KB;
                        }
                        return this._default;
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        throw new NotImplementedException();
                }
        }
}
