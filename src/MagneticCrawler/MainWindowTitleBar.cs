using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using Microsoft.VisualStudio.Shell;
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

namespace MagneticCrawler
{
        public sealed class MainWindowTitleBar : Border, INonClientArea
        {
                protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
                {
                        return (HitTestResult)new PointHitTestResult((Visual)this, hitTestParameters.HitPoint);
                }

                int INonClientArea.HitTest(Point point)
                {
                        return 2;
                }

                protected override void OnContextMenuOpening(ContextMenuEventArgs e)
                {
                        if (e.Handled)
                                return;
                        HwndSource source = PresentationSource.FromVisual((Visual)this) as HwndSource;
                        if (source != null)
                                CustomChromeWindow.ShowWindowMenu(source, (Visual)this, Mouse.GetPosition((IInputElement)this), this.RenderSize);
                        e.Handled = true;
                }
        }
}
