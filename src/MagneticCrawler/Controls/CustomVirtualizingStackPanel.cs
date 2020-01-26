using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MagneticCrawler.Controls
{
        public class CustomVirtualizingStackPanel : VirtualizingStackPanel
        {
                public event RoutedEventHandler MouseWheelDownEvent
                {
                        add
                        {
                                base.AddHandler(CustomVirtualizingStackPanel.MouseWheelDownEventEvent, value);
                        }
                        remove
                        {
                                base.RemoveHandler(CustomVirtualizingStackPanel.MouseWheelDownEventEvent, value);
                        }
                }
                public static readonly RoutedEvent MouseWheelDownEventEvent = EventManager.RegisterRoutedEvent("MouseWheelDownEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomVirtualizingStackPanel));
                protected virtual void OnMouseWheelDownEvent()
                {
                        RoutedEventArgs e = new RoutedEventArgs(CustomVirtualizingStackPanel.MouseWheelDownEventEvent, this);
                        base.RaiseEvent(e);
                }

                public override void MouseWheelDown()
                {
                        base.MouseWheelDown();
                        this.OnMouseWheelDownEvent();
                }
        }
}
