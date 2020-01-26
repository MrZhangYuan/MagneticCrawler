using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace UltimatePresentation.Native
{
        public static class UtilityMethods
        {
                private const double FloatStepX = 20.0;
                private const double FloatStepY = 20.0;
                private const int PositionRetries = 100;
                private static System.Windows.Point lastContentPos = new System.Windows.Point(double.NaN, double.NaN);
                private static System.Windows.Point currentFloatPos = new System.Windows.Point(double.NaN, double.NaN);
                public static void HitTestVisibleElements(Visual visual, HitTestResultCallback resultCallback, HitTestParameters parameters)
                {
                        VisualTreeHelper.HitTest(visual, new HitTestFilterCallback(UtilityMethods.ExcludeNonVisualElements), resultCallback, parameters);
                }
                private static HitTestFilterBehavior ExcludeNonVisualElements(DependencyObject potentialHitTestTarget)
                {
                        if (!(potentialHitTestTarget is Visual))
                        {
                                return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                        }
                        UIElement uIElement = potentialHitTestTarget as UIElement;
                        if (uIElement == null || (uIElement.IsVisible && uIElement.IsEnabled))
                        {
                                return HitTestFilterBehavior.Continue;
                        }
                        return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                }
                internal static bool ModifyStyle(IntPtr hWnd, int styleToRemove, int styleToAdd)
                {
                        int windowLong = NativeMethodsUltimate.GetWindowLong(hWnd, NativeMethodsUltimate.GWL.STYLE);
                        int num = (windowLong & ~styleToRemove) | styleToAdd;
                        if (num == windowLong)
                        {
                                return false;
                        }
                        NativeMethodsUltimate.SetWindowLong(hWnd, NativeMethodsUltimate.GWL.STYLE, num);
                        return true;
                }
        }

}
