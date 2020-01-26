using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace UltimatePresentation.Converters
{
        public class MultiValueIndentityConverter : IMultiValueConverter
        {
                private static MultiValueIndentityConverter _instance = null;
                public static MultiValueIndentityConverter Instance
                {
                        get => _instance ?? (_instance = new MultiValueIndentityConverter());
                }
                private MultiValueIndentityConverter()
                {

                }

                public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
                {
                        return values;
                }

                public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
                {
                        throw new NotImplementedException();
                }
        }
}
