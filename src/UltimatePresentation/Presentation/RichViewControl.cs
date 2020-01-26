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
        [TemplatePart(Name = PART_FLYOUTSTACKS, Type = typeof(FlyoutStacks))]
        [TemplatePart(Name = PART_DIALOGSTACKS, Type = typeof(DialogStacks))]
        public class RichViewControl : ItemsControl, IDialogContainer, IFlyoutContainer
        {
                private const string PART_FLYOUTSTACKS = "PART_FLYOUTSTACKS";
                private const string PART_DIALOGSTACKS = "PART_DIALOGSTACKS";

                private FlyoutStacks _flyoutStacks = null;
                private DialogStacks _dialogStacks = null;

                public Dialog TopDialog
                {
                        get
                        {
                                if (this._dialogStacks != null)
                                {
                                        return this._dialogStacks.CurrentDialog;
                                }
                                return null;
                        }
                }
                public Flyout TopFlyout
                {
                        get
                        {
                                if (this._flyoutStacks != null)
                                {
                                        return this._flyoutStacks.CurrentFlyout;
                                }
                                return null;
                        }
                }

                public bool IsAnimationActive
                {
                        get { return (bool)GetValue(IsAnimationActiveProperty); }
                        set { SetValue(IsAnimationActiveProperty, value); }
                }
                public static readonly DependencyProperty IsAnimationActiveProperty = ProgressRing.IsAnimationActiveProperty.AddOwner(typeof(RichViewControl), new PropertyMetadata(false));

                static RichViewControl()
                {
                        DefaultStyleKeyProperty.OverrideMetadata(typeof(RichViewControl), new FrameworkPropertyMetadata(typeof(RichViewControl)));
                }
                public override void OnApplyTemplate()
                {
                        base.OnApplyTemplate();
                        _flyoutStacks = GetTemplateChild(PART_FLYOUTSTACKS) as FlyoutStacks;
                        _dialogStacks = GetTemplateChild(PART_DIALOGSTACKS) as DialogStacks;
                        if (_flyoutStacks == null
                                && _dialogStacks == null)
                        {
                                throw new Exception();
                        }
                }
                protected override bool IsItemItsOwnContainerOverride(object item)
                {
                        return item is RichViewItem;
                }

                protected override DependencyObject GetContainerForItemOverride()
                {
                        return new RichViewItem();
                }
                public void ShowDialog(Dialog dialog)
                {
                        this._dialogStacks.ShowDialog(dialog);
                }

                public void CloseTopDialog()
                {
                        this._dialogStacks.CloseDialog();
                }

                public void ShowFlyout(Flyout flyout)
                {
                        this._flyoutStacks.ShowFlyout(flyout);
                }

                public void CloseFlyout()
                {
                        if (this._flyoutStacks != null)
                        {
                                this._flyoutStacks.CloseFlyout();
                        }
                }

                public void CloseDialog(object content)
                {
                        this._dialogStacks.CloseDialog(content);
                }

                public void CloseFlyout(object content)
                {
                        this._flyoutStacks.CloseFlyout(content);
                }
        }
}
