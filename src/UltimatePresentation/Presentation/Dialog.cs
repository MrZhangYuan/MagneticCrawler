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
        [TemplatePart(Name = PART_FLYOUTDIALOG, Type = typeof(RichViewControl))]
        public class Dialog : ContentControl, IDialogContainer, IFlyoutContainer
        {
                private const string PART_FLYOUTDIALOG = "PART_FLYOUTDIALOG";

                private RichViewControl _flyoutDialogControl = null;

                public Dialog TopDialog
                {
                        get
                        {
                                if (this._flyoutDialogControl != null)
                                {
                                        return this._flyoutDialogControl.TopDialog;
                                }
                                return null;
                        }
                }
                public Flyout TopFlyout
                {
                        get
                        {
                                if (this._flyoutDialogControl != null)
                                {
                                        return this._flyoutDialogControl.TopFlyout;
                                }
                                return null;
                        }
                }

                private DialogStacks _parentDialogStacks;
                internal DialogStacks ParentDialogStacks
                {
                        get
                        {
                                return this._parentDialogStacks;
                        }
                        set
                        {
                                this._parentDialogStacks = value;
                        }
                }

                /// <summary>
                /// 标题内容
                /// </summary>
                public object Head
                {
                        get { return (object)GetValue(HeadProperty); }
                        set { SetValue(HeadProperty, value); }
                }
                /// <summary>
                /// 标题内容
                /// </summary>
                public static readonly DependencyProperty HeadProperty = DependencyProperty.Register("Head", typeof(object), typeof(Dialog), new PropertyMetadata(null));
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
                public static readonly DependencyProperty HeadTemplateProperty = DependencyProperty.Register("HeadTemplate", typeof(DataTemplate), typeof(Dialog), new PropertyMetadata(null));
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
                public static readonly DependencyProperty HeadVisibilityProperty = DependencyProperty.Register("HeadVisibility", typeof(Visibility), typeof(Dialog), new PropertyMetadata(Visibility.Visible));

                /// <summary>
                /// 是否拖动
                /// </summary>
                public bool IsDragMove
                {
                        get { return (bool)GetValue(IsDragMoveProperty); }
                        set { SetValue(IsDragMoveProperty, value); }
                }
                /// <summary>
                /// 是否拖动
                /// </summary>
                public static readonly DependencyProperty IsDragMoveProperty = DependencyProperty.Register("IsDragMove", typeof(bool), typeof(Dialog), new PropertyMetadata(true));
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
                public static readonly DependencyProperty IsModalProperty = DependencyProperty.Register("IsModal", typeof(bool), typeof(Dialog), new PropertyMetadata(true));

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
                public static readonly DependencyProperty IsMarkBackElementProperty = DependencyProperty.Register("IsMarkBackElement", typeof(bool), typeof(Dialog), new PropertyMetadata(false));

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
                public static readonly DependencyProperty IsAnimationActiveProperty = ProgressRing.IsAnimationActiveProperty.AddOwner(typeof(Dialog), new PropertyMetadata(false));


                /// <summary>
                /// 指示此Dialog是否被取消了
                /// </summary>
                public bool IsCanceled
                {
                        get { return (bool)GetValue(IsCanceledProperty); }
                        set { SetValue(IsCanceledProperty, value); }
                }
                /// <summary>
                /// 指示此Dialog是否被取消了
                /// </summary>
                public static readonly DependencyProperty IsCanceledProperty = DependencyProperty.Register("IsCanceled", typeof(bool), typeof(Dialog), new PropertyMetadata(true));


                /// <summary>
                /// 鼠标点击Dialog旁边关闭
                /// </summary>
                public bool IsEasyClose
                {
                        get { return (bool)GetValue(IsEasyCloseProperty); }
                        set { SetValue(IsEasyCloseProperty, value); }
                }
                /// <summary>
                /// 鼠标点击Dialog旁边关闭
                /// </summary>
                public static readonly DependencyProperty IsEasyCloseProperty = DependencyProperty.Register("IsEasyClose", typeof(bool), typeof(Dialog), new PropertyMetadata(false));




                #region Events
                /// <summary>
                /// Dialog打开后触发
                /// </summary>
                public event RoutedEventHandler DialogOpened
                {
                        add
                        {
                                base.AddHandler(Dialog.DialogOpenedEvent, value);
                        }
                        remove
                        {
                                base.RemoveHandler(Dialog.DialogOpenedEvent, value);
                        }
                }
                /// <summary>
                /// Dialog打开后触发
                /// </summary>
                public static readonly RoutedEvent DialogOpenedEvent = EventManager.RegisterRoutedEvent("DialogOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Dialog));
                protected virtual void OnDialogOpened()
                {
                        RoutedEventArgs e = new RoutedEventArgs(Dialog.DialogOpenedEvent, this);
                        base.RaiseEvent(e);
                }

                /// <summary>
                /// Dialog关闭并移除后触发
                /// </summary>
                public event RoutedEventHandler DialogClosed
                {
                        add
                        {
                                base.AddHandler(Dialog.DialogClosedEvent, value);
                        }
                        remove
                        {
                                base.RemoveHandler(Dialog.DialogClosedEvent, value);
                        }
                }
                /// <summary>
                /// Dialog关闭并移除后触发
                /// </summary>
                public static readonly RoutedEvent DialogClosedEvent = EventManager.RegisterRoutedEvent("DialogClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Dialog));
                protected virtual void OnDialogClosed()
                {
                        RoutedEventArgs e = new RoutedEventArgs(Dialog.DialogClosedEvent, this);
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
                //                base.AddHandler(Dialog.TransitionCompletedEvent, value);
                //        }
                //        remove
                //        {
                //                base.RemoveHandler(Dialog.TransitionCompletedEvent, value);
                //        }
                //}
                ///// <summary>
                ///// 过渡完成事件
                ///// </summary>
                //public static readonly RoutedEvent TransitionCompletedEvent = TransitionContentControl.TransitionCompletedEvent.AddOwner(typeof(Dialog));
                #endregion
                static Dialog()
                {
                        DefaultStyleKeyProperty.OverrideMetadata(typeof(Dialog), new FrameworkPropertyMetadata(typeof(Dialog)));
                }
                public override void OnApplyTemplate()
                {
                        base.OnApplyTemplate();
                        _flyoutDialogControl = GetTemplateChild(PART_FLYOUTDIALOG) as RichViewControl;
                        if (_flyoutDialogControl == null)
                        {
                                throw new Exception();
                        }
                }

                #region Flyout & Dialog
                public void ShowDialog(Dialog dialog)
                {
                        this._flyoutDialogControl.ShowDialog(dialog);
                }
                public void CloseTopDialog()
                {
                        this._flyoutDialogControl.CloseTopDialog();
                }

                public void ShowFlyout(Flyout flyout)
                {
                        this._flyoutDialogControl.ShowFlyout(flyout);
                }
                public void CloseFlyout()
                {
                        this._flyoutDialogControl.CloseFlyout();
                }
                #endregion


                //public void Show()
                //{
                //        if (CustomWindow.Current != null)
                //        {
                //                CustomWindow.Current.ShowDialog(this);
                //        }
                //}
                public void Show(IDialogContainer container)
                {
                        if (container != null)
                        {
                                container.ShowDialog(this);
                        }
                }

                public void Close()
                {
                        if (this.ParentDialogStacks != null
                                && this.ParentDialogStacks.Items.Contains(this)
                                && object.ReferenceEquals(this.ParentDialogStacks.Items[this.ParentDialogStacks.Items.Count - 1], this))
                        {
                                this.ParentDialogStacks.CloseDialog();
                        }
                        else
                        {
                                FrameworkElement fe = this.Parent as FrameworkElement;
                                while (fe != null && fe.GetType() != typeof(Window))
                                {
                                        if (fe.GetType() == typeof(DialogStacks))
                                        {
                                                DialogStacks dialogstack = fe as DialogStacks;
                                                if (dialogstack.Items.Contains(this) && object.ReferenceEquals(dialogstack.Items[dialogstack.Items.Count - 1], this))
                                                {
                                                        dialogstack.CloseDialog();
                                                        return;
                                                }
                                        }
                                        fe = fe.Parent as FrameworkElement;
                                }
                        }
                }

                public void CloseDialog(object content)
                {
                        this._flyoutDialogControl.CloseDialog(content);
                }

                public void CloseFlyout(object content)
                {
                        this._flyoutDialogControl.CloseFlyout(content);
                }
        }
}
