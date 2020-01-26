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
        public enum FlyoutLocation
        {
                Left,
                Right,
                Bottom,
                Top
        }
        public class Flyout : ContentControl
        {
                private FlyoutStacks _parentFlyoutStacks;
                internal FlyoutStacks ParentFlyoutStacks
                {
                        get
                        {
                                return this._parentFlyoutStacks;
                        }
                        set
                        {
                                this._parentFlyoutStacks = value;
                        }
                }

                /// <summary>
                /// 标题
                /// </summary>
                public object Head
                {
                        get { return (object)GetValue(HeadProperty); }
                        set { SetValue(HeadProperty, value); }
                }
                /// <summary>
                /// 标题
                /// </summary>
                public static readonly DependencyProperty HeadProperty = DependencyProperty.Register("Head", typeof(object), typeof(Flyout), new PropertyMetadata(null));

                /// <summary>
                /// 标题模板
                /// </summary>
                public DataTemplate HeadTemplate
                {
                        get { return (DataTemplate)GetValue(HeadTemplateProperty); }
                        set { SetValue(HeadTemplateProperty, value); }
                }
                /// <summary>
                /// 标题模板
                /// </summary>
                public static readonly DependencyProperty HeadTemplateProperty = DependencyProperty.Register("HeadTemplate", typeof(DataTemplate), typeof(Flyout), new PropertyMetadata(null));

                /// <summary>
                /// 标题可见性
                /// </summary>
                public Visibility HeadVisibility
                {
                        get { return (Visibility)GetValue(HeadVisibilityProperty); }
                        set { SetValue(HeadVisibilityProperty, value); }
                }
                /// <summary>
                /// 标题可见性
                /// </summary>
                public static readonly DependencyProperty HeadVisibilityProperty = DependencyProperty.Register("HeadVisibility", typeof(Visibility), typeof(Flyout), new PropertyMetadata(Visibility.Visible));

                /// <summary>
                /// 是否模态显示
                /// </summary>
                public bool IsModal
                {
                        get { return (bool)GetValue(IsModalProperty); }
                        set { SetValue(IsModalProperty, value); }
                }
                /// <summary>
                /// 是否模态显示
                /// </summary>
                public static readonly DependencyProperty IsModalProperty = DependencyProperty.Register("IsModal", typeof(bool), typeof(Flyout), new PropertyMetadata(true));

                /// <summary>
                /// 是否使背后UI不可用（IsModel为False时可用）
                /// </summary>
                public bool IsMarkBackElement
                {
                        get { return (bool)GetValue(IsMarkBackElementProperty); }
                        set { SetValue(IsMarkBackElementProperty, value); }
                }
                /// <summary>
                /// 是否使背后UI不可用（IsModel为False时可用）
                /// </summary>
                public static readonly DependencyProperty IsMarkBackElementProperty = DependencyProperty.Register("IsMarkBackElement", typeof(bool), typeof(Flyout), new PropertyMetadata(false));

                /// <summary>
                /// Flyout位置
                /// </summary>
                public FlyoutLocation FlyoutLocation
                {
                        get { return (FlyoutLocation)GetValue(FlyoutLocationProperty); }
                        set { SetValue(FlyoutLocationProperty, value); }
                }
                /// <summary>
                /// Flyout位置
                /// </summary>
                public static readonly DependencyProperty FlyoutLocationProperty = DependencyProperty.Register("FlyoutLocation", typeof(FlyoutLocation), typeof(Flyout), new PropertyMetadata(FlyoutLocation.Left));

                /// <summary>
                /// 是否正在加载
                /// </summary>
                public bool IsAnimationActive
                {
                        get { return (bool)GetValue(IsAnimationActiveProperty); }
                        set { SetValue(IsAnimationActiveProperty, value); }
                }
                /// <summary>
                /// 是否正在加载
                /// </summary>
                public static readonly DependencyProperty IsAnimationActiveProperty = ProgressRing.IsAnimationActiveProperty.AddOwner(typeof(Flyout), new PropertyMetadata(false));


                /// <summary>
                /// 指示此Flyout是否被取消了
                /// </summary>
                public bool IsCanceled
                {
                        get { return (bool)GetValue(IsCanceledProperty); }
                        set { SetValue(IsCanceledProperty, value); }
                }
                /// <summary>
                /// 指示此Flyout是否被取消了
                /// </summary>
                public static readonly DependencyProperty IsCanceledProperty = DependencyProperty.Register("IsCanceled", typeof(bool), typeof(Flyout), new PropertyMetadata(true));


                public bool IsEasyClose
                {
                        get
                        {
                                return (bool)GetValue(IsEasyCloseProperty);
                        }
                        set
                        {
                                SetValue(IsEasyCloseProperty, value);
                        }
                }
                public static readonly DependencyProperty IsEasyCloseProperty = DependencyProperty.Register("IsEasyClose", typeof(bool), typeof(Flyout), new PropertyMetadata(false));

                #region Events

                /// <summary>
                /// Flyout打开后触发
                /// </summary>
                public event RoutedEventHandler FlyoutOpened
                {
                        add
                        {
                                base.AddHandler(Flyout.FlyoutOpenedEvent, value);
                        }
                        remove
                        {
                                base.RemoveHandler(Flyout.FlyoutOpenedEvent, value);
                        }
                }
                /// <summary>
                /// Flyout打开后触发
                /// </summary>
                public static readonly RoutedEvent FlyoutOpenedEvent = EventManager.RegisterRoutedEvent("FlyoutOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Flyout));
                protected virtual void OnFlyoutOpened()
                {
                        RoutedEventArgs e = new RoutedEventArgs(Flyout.FlyoutOpenedEvent, this);
                        base.RaiseEvent(e);
                }


                /// <summary>
                /// Flyout关闭并且移除后触发
                /// </summary>
                public event RoutedEventHandler FlyoutClosed
                {
                        add
                        {
                                base.AddHandler(Flyout.FlyoutClosedEvent, value);
                        }
                        remove
                        {
                                base.RemoveHandler(Flyout.FlyoutClosedEvent, value);
                        }
                }
                /// <summary>
                /// Flyout关闭并且移除后触发
                /// </summary>
                public static readonly RoutedEvent FlyoutClosedEvent = EventManager.RegisterRoutedEvent("FlyoutClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Flyout));
                protected virtual void OnFlyoutClosed()
                {
                        RoutedEventArgs e = new RoutedEventArgs(Flyout.FlyoutClosedEvent, this);
                        base.RaiseEvent(e);
                }

                internal void RaiseEvents(RoutedEvent events)
                {
                        if (events == null)
                        {
                                return;
                        }
                        RoutedEventArgs e = new RoutedEventArgs(events, this);
                        base.RaiseEvent(e);
                }

                ///// <summary>
                ///// 过渡完成事件
                ///// </summary>
                //public event TransitionCompletedEventHandler TransitionCompleted
                //{
                //        add
                //        {
                //                base.AddHandler(Flyout.TransitionCompletedEvent, value);
                //        }
                //        remove
                //        {
                //                base.RemoveHandler(Flyout.TransitionCompletedEvent, value);
                //        }
                //}
                ///// <summary>
                ///// 过渡完成事件
                ///// </summary>
                //public static readonly RoutedEvent TransitionCompletedEvent = TransitionContentControl.TransitionCompletedEvent.AddOwner(typeof(Flyout));
                #endregion

                static Flyout()
                {
                        DefaultStyleKeyProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout)));
                }

                //public void Show()
                //{
                //        if (CustomWindow.Current != null)
                //        {
                //                CustomWindow.Current.ShowFlyout(this);
                //        }
                //}
                public void Show(IFlyoutContainer container)
                {
                        if (container != null)
                        {
                                container.ShowFlyout(this);
                        }
                }

                public void Close()
                {
                        if (this.ParentFlyoutStacks != null && this.ParentFlyoutStacks.Items.Contains(this) && object.ReferenceEquals(this.ParentFlyoutStacks.Items[this.ParentFlyoutStacks.Items.Count - 1], this))
                        {
                                this.ParentFlyoutStacks.CloseFlyout();
                        }
                        else
                        {
                                FrameworkElement fe = this.Parent as FrameworkElement;
                                while (fe != null && fe.GetType() != typeof(Window))
                                {
                                        if (fe.GetType() == typeof(FlyoutStacks))
                                        {
                                                FlyoutStacks flyoutstack = fe as FlyoutStacks;
                                                if (flyoutstack.Items.Contains(this) && object.ReferenceEquals(flyoutstack.Items[flyoutstack.Items.Count - 1], this))
                                                {
                                                        flyoutstack.CloseFlyout();
                                                        return;
                                                }
                                        }
                                        fe = fe.Parent as FrameworkElement;
                                }
                        }
                }
        }
}
