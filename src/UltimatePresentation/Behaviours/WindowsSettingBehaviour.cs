using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using UltimatePresentation.Native;
using UltimatePresentation.Native.Win32;
using UltimatePresentation.Presentation;

namespace UltimatePresentation.Behaviours
{
        public class WindowsSettingBehaviour : Behavior<Window>
        {
                protected override void OnAttached()
                {
                        if (AssociatedObject != null && (bool)AssociatedObject.GetValue(WindowHelper.SaveWindowPositionProperty))
                        {
                                // save with custom settings class or use the default way
                                var windowPlacementSettings = new WindowApplicationSettings(this.AssociatedObject);
                                WindowSettings.SetSave(AssociatedObject, windowPlacementSettings);
                        }
                }
        }
}
