using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MagneticCrawler.Controls
{
        public class ControlsHelper
        {
                public static double GetWaterMarkFontSize(DependencyObject obj)
                {
                        return (double)obj.GetValue(WaterMarkFontSizeProperty);
                }
                public static void SetWaterMarkFontSize(DependencyObject obj, double value)
                {
                        obj.SetValue(WaterMarkFontSizeProperty, value);
                }
                public static readonly DependencyProperty WaterMarkFontSizeProperty = DependencyProperty.RegisterAttached("WaterMarkFontSize", typeof(double), typeof(ControlsHelper), new PropertyMetadata(12d));

        }
}
