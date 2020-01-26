using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace UltimatePresentation.Converters
{
        public class BoolenOrMultiConverter : IMultiValueConverter
        {
                private static BoolenOrMultiConverter _instance = null;
                public static BoolenOrMultiConverter Instance
                {
                        get => _instance ?? (_instance = new BoolenOrMultiConverter());
                }
                private BoolenOrMultiConverter()
                {

                }
                public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
                {
                        if (values != null
                                && values.Length > 0)
                        {
                                for (int i = 0; i < values.Length; i++)
                                {
                                        if (bool.TryParse(values[i] + "", out bool result)
                                                && result)
                                        {
                                                return true;
                                        }
                                }
                        }

                        return false;
                }

                public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
                {
                        throw new NotImplementedException();
                }
        }
}
