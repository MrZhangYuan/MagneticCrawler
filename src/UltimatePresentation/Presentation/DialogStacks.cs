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
        internal class DialogStacks : ItemsControl
        {
                /// <summary>
                /// 当前位于最上层的Dialog
                /// </summary>
                public Dialog CurrentDialog
                {
                        get { return (Dialog)GetValue(CurrentDialogProperty); }
                        set { SetValue(CurrentDialogProperty, value); }
                }
                /// <summary>
                /// 当前位于最上层的Dialog
                /// </summary>
                public static readonly DependencyProperty CurrentDialogProperty = DependencyProperty.Register("CurrentDialog", typeof(Dialog), typeof(DialogStacks), new PropertyMetadata(null));

                static DialogStacks()
                {
                        DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogStacks), new FrameworkPropertyMetadata(typeof(DialogStacks)));
                        CommandManager.RegisterClassCommandBinding(typeof(DialogStacks), new CommandBinding(ControlCommands.CloseDialog, new ExecutedRoutedEventHandler(DialogStacks.OnDialogFlyout), new CanExecuteRoutedEventHandler(DialogStacks.OnCanDialogFlyout)));
                }
                private static void OnCanDialogFlyout(object sender, CanExecuteRoutedEventArgs e)
                {
                        DialogStacks dialogstacks = (DialogStacks)sender;
                        e.CanExecute = dialogstacks.Items.Count > 0;
                        e.Handled = true;
                }

                private static void OnDialogFlyout(object sender, ExecutedRoutedEventArgs e)
                {
                        ((DialogStacks)sender).CloseDialog();
                }

                protected override DependencyObject GetContainerForItemOverride()
                {
                        return new DialogContainer();
                }
                protected override bool IsItemItsOwnContainerOverride(object item)
                {
                        return item is DialogContainer;
                }
                public void ShowDialog(Dialog dialog)
                {
                        if (dialog != null && !this.Items.Contains(dialog))
                        {
                                this.Items.Add(dialog);
                                dialog.ParentDialogStacks = this;
                                this.CurrentDialog = dialog;
                                dialog.RaiseEvents(Dialog.DialogOpenedEvent);
                        }
                }
                public void CloseDialog(object content)
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

                                        Dialog removeddialog = target as Dialog;
                                        if (removeddialog != null)
                                        {
                                                removeddialog.RaiseEvents(Dialog.DialogClosedEvent);
                                        }
                                }
                        }
                }
                public void CloseDialog()
                {
                        if (this.Items.Count > 0)
                        {
                                object obj = this.Items[this.Items.Count - 1];

                                this.Items.Remove(obj);

                                if (this.Items.Count > 0)
                                {
                                        this.CurrentDialog = this.Items[this.Items.Count - 1] as Dialog;
                                }
                                else
                                {
                                        this.CurrentDialog = null;
                                }

                                Dialog removeddialog = obj as Dialog;
                                if (removeddialog != null)
                                {
                                        removeddialog.RaiseEvents(Dialog.DialogClosedEvent);
                                }
                                //CustomWindow.Current.CloseSoftkeyboard();
                        }
                }
        }

}
