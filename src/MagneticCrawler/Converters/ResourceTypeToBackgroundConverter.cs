using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MagneticCrawler.Converters
{
        public class ResourceTypeToBackgroundConverter : IValueConverter
        {
                public static ResourceTypeToBackgroundConverter Instance
                {
                        get;
                }
                static ResourceTypeToBackgroundConverter()
                {
                        Instance = new ResourceTypeToBackgroundConverter();
                }

                private SolidColorBrush _default = new SolidColorBrush(Colors.Green);
                private SolidColorBrush _Video = new SolidColorBrush(Colors.OrangeRed);
                private SolidColorBrush _Audio = new SolidColorBrush(Colors.DarkOrange);
                private SolidColorBrush _File = new SolidColorBrush(Colors.Orange);
                private SolidColorBrush _Image = new SolidColorBrush(Colors.Orange);

                private ResourceTypeToBackgroundConverter()
                {

                }
                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        string text = (value + "").ToUpper();
                       
                        return this._default;
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        throw new NotImplementedException();
                }
        }
}
