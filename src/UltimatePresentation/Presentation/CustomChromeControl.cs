using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UltimatePresentation.Presentation
{
        public class CustomChromeControl : UserControl
        {

                /// <summary>
                /// TransitionCompleted
                /// </summary>
                public event RoutedEventHandler TransitionCompleted
                {
                        add
                        {
                                base.AddHandler(CustomChromeControl.TransitionCompletedEvent, value);
                        }
                        remove
                        {
                                base.RemoveHandler(CustomChromeControl.TransitionCompletedEvent, value);
                        }
                }
                /// <summary>
                /// TransitionCompleted
                /// </summary>
                public static readonly RoutedEvent TransitionCompletedEvent =
                        EventManager.RegisterRoutedEvent(
                                "TransitionCompleted",
                                RoutingStrategy.Bubble,
                                typeof(RoutedEventHandler),
                                typeof(CustomChromeControl));

                protected internal virtual void OnTransitionCompleted()
                {
                        RoutedEventArgs e = new RoutedEventArgs(CustomChromeControl.TransitionCompletedEvent, this);
                        base.RaiseEvent(e);
                }


                /// <summary>
                /// ReTransitionCompleted
                /// </summary>
                public event RoutedEventHandler ReTransitionCompleted
                {
                        add
                        {
                                base.AddHandler(CustomChromeControl.ReTransitionCompletedEvent, value);
                        }
                        remove
                        {
                                base.RemoveHandler(CustomChromeControl.ReTransitionCompletedEvent, value);
                        }
                }
                /// <summary>
                /// ReTransitionCompleted
                /// </summary>
                public static readonly RoutedEvent ReTransitionCompletedEvent =
                        EventManager.RegisterRoutedEvent(
                                "ReTransitionCompleted",
                                RoutingStrategy.Bubble,
                                typeof(RoutedEventHandler),
                                typeof(CustomChromeControl));

                protected internal virtual void OnReTransitionCompleted()
                {
                        RoutedEventArgs e = new RoutedEventArgs(CustomChromeControl.ReTransitionCompletedEvent, this);
                        base.RaiseEvent(e);
                }

                static CustomChromeControl()
                {
                        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomChromeControl), new FrameworkPropertyMetadata(typeof(CustomChromeControl)));
                }
        }
}
