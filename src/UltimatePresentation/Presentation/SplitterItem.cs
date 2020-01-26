using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace UltimatePresentation.Presentation
{
        public class SplitterItem : ContentControl
        {
                static SplitterItem()
                {
                        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterItem), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(SplitterItem)));
                }

                public SplitterItem()
                {
                        AutomationProperties.SetAutomationId((DependencyObject)this, nameof(SplitterItem));
                }

                protected override void OnContentChanged(object oldContent, object newContent)
                {
                        base.OnContentChanged(oldContent, newContent);

                        //if (newContent != null
                        //        && newContent is DependencyObject)
                        //{
                        //        DependencyObject content = newContent as DependencyObject;

                        //        this.SetBinding(SplitterPanel.SplitterLengthProperty, new Binding
                        //        {
                        //                Source = content,
                        //                Path = new PropertyPath(SplitterPanel.SplitterLengthProperty),
                        //                Mode = BindingMode.TwoWay
                        //        });

                        //        this.SetBinding(SplitterPanel.MinimumLengthProperty, new Binding
                        //        {
                        //                Source = content,
                        //                Path = new PropertyPath(SplitterPanel.MinimumLengthProperty),
                        //                Mode = BindingMode.TwoWay
                        //        });

                        //        this.SetBinding(SplitterPanel.MaximumLengthProperty, new Binding
                        //        {
                        //                Source = content,
                        //                Path = new PropertyPath(SplitterPanel.MaximumLengthProperty),
                        //                Mode = BindingMode.TwoWay
                        //        });
                        //}
                }
        }

}
