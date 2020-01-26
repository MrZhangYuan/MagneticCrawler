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
        internal class DialogContainer : ContentControl
        {
                //MouseDragElementBehavior _MouseDragElementBehavior = null;
                ContentPresenter content;
                Grid background;

                /// <summary>
                /// 是否拖动
                /// </summary>
                private bool CanDragMove
                {
                        get { return (bool)GetValue(CanDragMoveProperty); }
                        set { SetValue(CanDragMoveProperty, value); }
                }
                /// <summary>
                /// 是否拖动
                /// </summary>
                private static readonly DependencyProperty CanDragMoveProperty = DependencyProperty.Register("CanDragMove", typeof(bool), typeof(DialogContainer), new PropertyMetadata(true, null,
                        (sender, e) =>
                        {
                                //DialogContainer DC = sender as DialogContainer;
                                //if ((bool)e)
                                //{
                                //        if (DC._MouseDragElementBehavior == null)
                                //        {
                                //                DC._MouseDragElementBehavior = new MouseDragElementBehavior() { ConstrainToParentBounds = true };
                                //        }
                                //        DC._MouseDragElementBehavior.Attach(DC.Content as FrameworkElement);
                                //}
                                //else
                                //{
                                //        try
                                //        {
                                //                if (DC._MouseDragElementBehavior != null)
                                //                {
                                //                        DC._MouseDragElementBehavior.Detach();
                                //                }
                                //        }
                                //        catch (Exception)
                                //        {

                                //        }
                                //}
                                return e;
                        }));

                static DialogContainer()
                {
                        DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogContainer), new FrameworkPropertyMetadata(typeof(DialogContainer)));
                }
                public override void OnApplyTemplate()
                {
                        base.OnApplyTemplate();
                        content = GetTemplateChild("PART_CONTENT") as ContentPresenter;
                        background = GetTemplateChild("PART_BACKGROUND") as Grid;
                        if (content == null || background == null)
                        {
                                throw new Exception();
                        }

                        Dialog currentDialog = this.Content as Dialog;
                        if (currentDialog != null)
                        {
                                this.SetBinding(DialogContainer.CanDragMoveProperty, new Binding
                                {
                                        Source = currentDialog,
                                        Path = new PropertyPath("IsDragMove"),
                                        Mode = BindingMode.OneWay
                                });

                                background.MouseDown += Background_MouseDown;
                        }
                }

                private void Background_MouseDown(object sender, MouseButtonEventArgs e)
                {
                        Grid grid = sender as Grid;

                        if (object.ReferenceEquals(sender, e.OriginalSource))
                        {
                                Dialog currentDialog = this.Content as Dialog;
                                if (currentDialog != null
                                        && currentDialog.IsEasyClose)
                                {
                                        grid.MouseDown -= Background_MouseDown;

                                        currentDialog.Close();
                                }
                        }
                }
        }

}
