using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        internal class FlyoutStacks : ItemsControl
        {
                /// <summary>
                /// 当前位于最上层的Flyout
                /// </summary>
                public Flyout CurrentFlyout
                {
                        get { return (Flyout)GetValue(CurrentFlyoutProperty); }
                        set { SetValue(CurrentFlyoutProperty, value); }
                }
                /// <summary>
                /// 当前位于最上层的Flyout
                /// </summary>
                public static readonly DependencyProperty CurrentFlyoutProperty = DependencyProperty.Register("CurrentFlyout", typeof(Flyout), typeof(FlyoutStacks), new PropertyMetadata(null));

                static FlyoutStacks()
                {
                        DefaultStyleKeyProperty.OverrideMetadata(typeof(FlyoutStacks), new FrameworkPropertyMetadata(typeof(FlyoutStacks)));

                        CommandManager.RegisterClassCommandBinding
                                (
                                        typeof(FlyoutStacks),
                                        new CommandBinding
                                        (
                                                ControlCommands.CloseFlyout,
                                                new ExecutedRoutedEventHandler(FlyoutStacks.OnCloseFlyout),
                                                new CanExecuteRoutedEventHandler(FlyoutStacks.OnCanCloseFlyout)
                                        )
                                );

                        CommandManager.RegisterClassInputBinding
                                (
                                        typeof(FlyoutStacks),
                                        new KeyBinding
                                        (
                                                ControlCommands.CloseFlyout,
                                                Key.Escape,
                                                ModifierKeys.None
                                        )
                                );
                }

                private static void OnCanCloseFlyout(object sender, CanExecuteRoutedEventArgs e)
                {
                        FlyoutStacks flyoutstacks = (FlyoutStacks)sender;
                        e.CanExecute = flyoutstacks.Items.Count > 0;
                        e.Handled = true;
                }

                private static void OnCloseFlyout(object sender, ExecutedRoutedEventArgs e)
                {
                        ((FlyoutStacks)sender).CloseFlyout();
                }

                protected override DependencyObject GetContainerForItemOverride()
                {
                        return new FlyoutContainer();
                }
                protected override bool IsItemItsOwnContainerOverride(object item)
                {
                        return item is FlyoutContainer;
                }
                public void ShowFlyout(Flyout flyout)
                {
                        if (flyout != null)
                        {
                                this.Items.Add(flyout);
                                flyout.ParentFlyoutStacks = this;
                                this.CurrentFlyout = flyout;
                                flyout.RaiseEvents(Flyout.FlyoutOpenedEvent);
                        }
                }
                public void CloseFlyout(object content)
                {
                        if (content != null)
                        {
                                object target = null;

                                foreach (var item in this.Items)
                                {
                                        if (object.ReferenceEquals(item, content))
                                        {
                                                target = item;
                                                break;
                                        }
                                        else
                                        {
                                                ContentControl contentControl = item as ContentControl;
                                                if (contentControl != null
                                                        && object.ReferenceEquals(contentControl.Content, content))
                                                {
                                                        target = item;
                                                        break;
                                                }
                                        }
                                }
                                if (target != null)
                                {
                                        this.Items.Remove(target);

                                        Flyout removedflyout = target as Flyout;
                                        if (removedflyout != null)
                                        {
                                                removedflyout.RaiseEvents(Flyout.FlyoutClosedEvent);
                                        }
                                }
                        }
                }
                public void CloseFlyout()
                {
                        if (this.Items.Count > 0)
                        {
                                object obj = this.Items[this.Items.Count - 1];
                                this.Items.Remove(obj);
                                if (this.Items.Count > 0)
                                {
                                        this.CurrentFlyout = this.Items[this.Items.Count - 1] as Flyout;
                                }
                                else
                                {
                                        this.CurrentFlyout = null;
                                }

                                Flyout removedflyout = obj as Flyout;
                                if (removedflyout != null)
                                {
                                        removedflyout.RaiseEvents(Flyout.FlyoutClosedEvent);
                                }
                                //CustomWindow.Current.CloseSoftkeyboard();
                        }
                }
        }

}
