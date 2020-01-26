using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
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
using UltimatePresentation.Presentation;

namespace MagneticCrawler.Controls
{
        public class CustomMainWindow : Microsoft.VisualStudio.PlatformUI.Shell.Controls.CustomChromeWindow, IDialogContainer, IFlyoutContainer
        {
                private RichViewControl _RichViewControl = null;
                static CustomMainWindow()
                {
                        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomMainWindow), new FrameworkPropertyMetadata(typeof(CustomMainWindow)));
                }

                public Dialog TopDialog
                {
                        get
                        {
                                return this._RichViewControl?.TopDialog;
                        }
                }

                public Flyout TopFlyout
                {
                        get
                        {
                                return this._RichViewControl?.TopFlyout;
                        }
                }

                public void CloseDialog(object content)
                {
                        this._RichViewControl.CloseDialog(content);
                }

                public void CloseFlyout()
                {
                        this._RichViewControl?.CloseFlyout();
                }

                public void CloseFlyout(object content)
                {
                        this._RichViewControl.CloseFlyout(content);
                }

                public void CloseTopDialog()
                {
                        this._RichViewControl?.CloseTopDialog();
                }

                public override void OnApplyTemplate()
                {
                        base.OnApplyTemplate();
                        this._RichViewControl = this.GetTemplateChild("PART_RichViewControl") as RichViewControl;
                }

                public void ShowDialog(Dialog dialog)
                {
                        this._RichViewControl?.ShowDialog(dialog);
                }

                public void ShowFlyout(Flyout flyout)
                {
                        this._RichViewControl?.ShowFlyout(flyout);
                }
        }
}
