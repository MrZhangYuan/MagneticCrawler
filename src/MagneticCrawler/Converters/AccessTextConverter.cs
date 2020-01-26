using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MagneticCrawler.Converters
{
        internal class AccessTextConverter : IValueConverter
        {
                private static readonly Lazy<AccessTextConverter> instance = new Lazy<AccessTextConverter>(true);

                public static AccessTextConverter Instance
                {
                        get
                        {
                                return AccessTextConverter.instance.Value;
                        }
                }

                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        if (value != null)
                                return RemoveAccessKeyUnderscore(value.ToString());
                        return (object)string.Empty;
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        throw new NotSupportedException();
                }

                public string RemoveAccessKeyUnderscore(string accessText)
                {
                        if (!string.IsNullOrEmpty(accessText))
                                return Regex.Replace(accessText, "^([^_]*)_([^_])", "$1$2");
                        return accessText;
                }

        }
}
