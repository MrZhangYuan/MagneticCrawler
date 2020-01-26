using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UltimatePresentation.Presentation
{
        public class StreachUniformPanel : Panel
        {
                public Orientation Orientation
                {
                        get { return (Orientation)GetValue(OrientationProperty); }
                        set { SetValue(OrientationProperty, value); }
                }
                public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StreachUniformPanel), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

                private IEnumerable<UIElement> NonCollapsedChildren
                {
                        get
                        {
                                return this.InternalChildren.Cast<UIElement>().Where<UIElement>((Func<UIElement, bool>)(e =>
                                {
                                        if (e != null)
                                                return e.Visibility != Visibility.Collapsed;
                                        return false;
                                }));
                        }
                }
                public StreachUniformPanel()
                {

                }

                private Size _eashSize = new Size();
                protected override Size MeasureOverride(Size availableSize)
                {
                        int count = this.NonCollapsedChildren.Count();

                        switch (this.Orientation)
                        {
                                case Orientation.Horizontal:
                                        if (double.IsInfinity(availableSize.Width))
                                        {
                                                throw new Exception("StreachUniformPanel:无法确定面板尺寸。");
                                        }
                                        this._eashSize = new Size(availableSize.Width / count, availableSize.Height);
                                        break;
                                case Orientation.Vertical:
                                        if (double.IsInfinity(availableSize.Height))
                                        {
                                                throw new Exception("StreachUniformPanel:无法确定面板尺寸。");
                                        }
                                        this._eashSize = new Size(availableSize.Width, availableSize.Height / count);
                                        break;
                        }

                        foreach (UIElement child in this.NonCollapsedChildren)
                        {
                                child.Measure(this._eashSize);
                        }

                        return availableSize;
                }

                protected override Size ArrangeOverride(Size finalSize)
                {
                        switch (this.Orientation)
                        {
                                case Orientation.Horizontal:
                                        double x = 0;
                                        foreach (UIElement child in this.NonCollapsedChildren)
                                        {
                                                child.Arrange(new Rect(new Point(x, 0), this._eashSize));
                                                x += this._eashSize.Width;
                                        }
                                        break;

                                case Orientation.Vertical:
                                        double y = 0;
                                        foreach (UIElement child in this.NonCollapsedChildren)
                                        {
                                                child.Arrange(new Rect(new Point(0, y), this._eashSize));
                                                y += this._eashSize.Height;
                                        }
                                        break;
                        }

                        return finalSize;
                }
        }

}
