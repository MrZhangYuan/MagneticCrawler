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
        internal class FlyoutContainer : ContentControl
        {
                ContentPresenter content;
                Grid background;

                static FlyoutContainer()
                {
                        DefaultStyleKeyProperty.OverrideMetadata(typeof(FlyoutContainer), new FrameworkPropertyMetadata(typeof(FlyoutContainer)));
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

                        Flyout currentFlyout = this.Content as Flyout;
                        if (currentFlyout != null)
                        {
                                background.MouseDown += Background_MouseDown;
                        }
                }

                private void Background_MouseDown(object sender, MouseButtonEventArgs e)
                {
                        Grid grid = sender as Grid;

                        if (object.ReferenceEquals(sender, e.OriginalSource))
                        {
                                Flyout currentFlyout = this.Content as Flyout;
                                if (currentFlyout != null
                                        && currentFlyout.IsEasyClose)
                                {
                                        grid.MouseDown -= Background_MouseDown;

                                        currentFlyout.Close();
                                }
                        }
                }
        }

}
