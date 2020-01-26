using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UltimatePresentation.Behaviours;

namespace UltimatePresentation.Presentation
{
        public class WindowHelper
        {
                public static bool GetSaveWindowPosition(DependencyObject obj)
                {
                        return (bool)obj.GetValue(SaveWindowPositionProperty);
                }
                public static void SetSaveWindowPosition(DependencyObject obj, bool value)
                {
                        obj.SetValue(SaveWindowPositionProperty, value);
                }
                public static readonly DependencyProperty SaveWindowPositionProperty = DependencyProperty.RegisterAttached("SaveWindowPosition", typeof(bool), typeof(WindowHelper), new PropertyMetadata(false, callback));

                private static void callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                        Window window = d as Window;
                        if (window == null)
                        {
                                throw new Exception("SaveWindowPosition附加属性只能应用在Window类型上。");
                        }

                        if ((bool)e.NewValue)
                        {
                                StylizedBehaviors.SetBehaviors(window, new StylizedBehaviorCollection
                                {
                                        new WindowsSettingBehaviour()
                                });
                        }
                        else
                        {
                                StylizedBehaviors.SetBehaviors(window, null);
                        }
                }
        }
}
