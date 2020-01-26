using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MagneticCrawler.Converters
{
        internal class ViewElementToViewConverter : IValueConverter
        {
                private static readonly Lazy<ViewElementToViewConverter> instance = new Lazy<ViewElementToViewConverter>(true);

                public static ViewElementToViewConverter Instance
                {
                        get
                        {
                                return ViewElementToViewConverter.instance.Value;
                        }
                }

                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        return (object)(value as View);
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        throw new NotSupportedException();
                }
        }
}
