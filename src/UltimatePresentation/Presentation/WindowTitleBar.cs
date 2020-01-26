using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace UltimatePresentation.Presentation
{
        public class WindowTitleBar : Border, INonClientArea
        {
                /// <summary>
                /// 系统菜单的启用状态
                /// </summary>
                public bool IsSystemMenuEnabled
                {
                        get
                        {
                                return (bool)GetValue(IsSystemMenuEnabledProperty);
                        }
                        set
                        {
                                SetValue(IsSystemMenuEnabledProperty, value);
                        }
                }
                /// <summary>
                /// 系统菜单的启用状态
                /// </summary>
                public static readonly DependencyProperty IsSystemMenuEnabledProperty = DependencyProperty.Register(
                        "IsSystemMenuEnabled",
                        typeof(bool),
                        typeof(WindowTitleBar),
                        new PropertyMetadata(true));

                protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
                {
                        return new PointHitTestResult(this, hitTestParameters.HitPoint);
                }
                int INonClientArea.HitTest(System.Windows.Point point)
                {
                        return 2;
                }
                protected override void OnContextMenuOpening(ContextMenuEventArgs e)
                {
                        if (this.IsSystemMenuEnabled
                                && !e.Handled)
                        {
                                HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
                                if (hwndSource != null)
                                {
                                        CustomChromeWindow.ShowWindowMenu(hwndSource, this, Mouse.GetPosition(this), base.RenderSize);
                                }
                                e.Handled = true;
                        }
                }
        }
}
