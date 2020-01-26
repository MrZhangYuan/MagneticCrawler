using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UltimatePresentation.Presentation
{

        public class SplitterItemsControl : ItemsControl
        {
                public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(SplitterItemsControl), (PropertyMetadata)new FrameworkPropertyMetadata((object)Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(SplitterItemsControl.OnOrientationChanged)));
                public static readonly DependencyProperty SplitterGripSizeProperty = DependencyProperty.RegisterAttached("SplitterGripSize", typeof(double), typeof(SplitterItemsControl), (PropertyMetadata)new FrameworkPropertyMetadata((object)5.0, FrameworkPropertyMetadataOptions.Inherits));

                static SplitterItemsControl()
                {
                        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterItemsControl), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(SplitterItemsControl)));
                }
                public static double GetSplitterGripSize(DependencyObject element)
                {
                        if (element == null)
                                throw new ArgumentNullException(nameof(element));
                        return (double)element.GetValue(SplitterItemsControl.SplitterGripSizeProperty);
                }

                public static void SetSplitterGripSize(DependencyObject element, double value)
                {
                        if (element == null)
                                throw new ArgumentNullException(nameof(element));
                        element.SetValue(SplitterItemsControl.SplitterGripSizeProperty, (object)value);
                }

                public Orientation Orientation
                {
                        get
                        {
                                return (Orientation)this.GetValue(SplitterItemsControl.OrientationProperty);
                        }
                        set
                        {
                                this.SetValue(SplitterItemsControl.OrientationProperty, (object)value);
                        }
                }

                protected override bool IsItemItsOwnContainerOverride(object item)
                {
                        return item is SplitterItem;
                }

                protected override DependencyObject GetContainerForItemOverride()
                {
                        return (DependencyObject)new SplitterItem();
                }

                private static void OnOrientationChanged(object sender, DependencyPropertyChangedEventArgs args)
                {
                        ((SplitterItemsControl)sender).OnOrientationChanged();
                }

                protected virtual void OnOrientationChanged()
                {
                }
        }

}
