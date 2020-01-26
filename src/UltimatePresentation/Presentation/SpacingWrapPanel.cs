using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UltimatePresentation.Presentation
{
        /// <summary>
        /// 自动分散空间的WrapPanel
        /// </summary>
        public class SpacingWrapPanel : WrapPanel
        {
                private static readonly DependencyProperty HorizontalItemSpacingProperty = DependencyProperty.Register(nameof(HorizontalItemSpacing), typeof(double), typeof(SpacingWrapPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange), new ValidateValueCallback(SpacingWrapPanel.ValidateWidthHeight));
                private static readonly DependencyProperty VerticalItemSpacingProperty = DependencyProperty.Register(nameof(VerticalItemSpacing), typeof(double), typeof(SpacingWrapPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange), new ValidateValueCallback(SpacingWrapPanel.ValidateWidthHeight));
                private const Orientation NaturalOrientation = Orientation.Horizontal;
                private readonly SpacingWrapPanel.MeasureInfo _measureInfo;

                public SpacingWrapPanel()
                {
                        this._measureInfo = new SpacingWrapPanel.MeasureInfo(this);
                }

                public double HorizontalItemSpacing
                {
                        get
                        {
                                return (double)this.GetValue(SpacingWrapPanel.HorizontalItemSpacingProperty);
                        }
                        set
                        {
                                this.SetValue(SpacingWrapPanel.HorizontalItemSpacingProperty, (object)value);
                        }
                }

                private static bool ValidateWidthHeight(object value)
                {
                        double d = (double)value;
                        if (d < 0.0 || double.IsPositiveInfinity(d))
                                return false;
                        double.IsNaN(d);
                        return true;
                }

                public double VerticalItemSpacing
                {
                        get
                        {
                                return (double)this.GetValue(SpacingWrapPanel.VerticalItemSpacingProperty);
                        }
                        set
                        {
                                this.SetValue(SpacingWrapPanel.VerticalItemSpacingProperty, (object)value);
                        }
                }

                private AbstractSize AbstractItemSpacing
                {
                        get
                        {
                                return this.MakeAbstractSize(this.HorizontalItemSpacing, this.VerticalItemSpacing);
                        }
                }

                protected override Size MeasureOverride(Size constraint)
                {
                        this._measureInfo.Reset();
                        double itemWidth = this.ItemWidth;
                        double itemHeight = this.ItemHeight;
                        bool flag1 = !double.IsNaN(itemWidth);
                        bool flag2 = !double.IsNaN(itemHeight);
                        Size availableSize = new Size(flag1 ? itemWidth : constraint.Width, flag2 ? itemHeight : constraint.Height);
                        AbstractSize abstractSize = this.MakeAbstractSize(constraint);
                        SpacingWrapPanel.RowInfo row = new SpacingWrapPanel.RowInfo(this, abstractSize.AbstractWidth);
                        foreach (UIElement nonCollapsedChild in this.NonCollapsedChildren)
                        {
                                nonCollapsedChild.Measure(availableSize);
                                AbstractSize childSize = this.MakeAbstractSize(flag1 ? itemWidth : nonCollapsedChild.DesiredSize.Width, flag2 ? itemHeight : nonCollapsedChild.DesiredSize.Height);
                                if (!row.AddChild(childSize))
                                {
                                        this._measureInfo.AddRow(row);
                                        row = new SpacingWrapPanel.RowInfo(this, abstractSize.AbstractWidth);
                                        row.AddChild(childSize);
                                }
                        }
                        this._measureInfo.AddRow(row);
                        return this._measureInfo.AbstractSize.RealSize;
                }

                protected override Size ArrangeOverride(Size constraint)
                {
                        UIElement[] array = this.NonCollapsedChildren.ToArray<UIElement>();
                        AbstractPoint abstractPoint = this.MakeAbstractPoint();
                        AbstractSize abstractSize1 = this.MakeAbstractSize(constraint);
                        AbstractSize abstractItemSpacing = this.AbstractItemSpacing;
                        double itemSpacing;
                        double remainder;
                        this.ComputeItemSpacing(this._measureInfo.RowCount, abstractSize1.AbstractHeight, this._measureInfo.AbstractSize.AbstractHeight, abstractItemSpacing.AbstractHeight, out itemSpacing, out remainder);
                        int index1 = 0;
                        for (int index2 = 0; index2 < this._measureInfo.RowCount; ++index2)
                        {
                                SpacingWrapPanel.RowInfo row = this._measureInfo.Rows[index2];
                                int childCount = row.ChildCount;
                                double abstractWidth1 = abstractSize1.AbstractWidth;
                                AbstractSize abstractSize2 = row.AbstractSize;
                                double abstractWidth2 = abstractSize2.AbstractWidth;
                                double abstractWidth3 = abstractItemSpacing.AbstractWidth;
                                double num1;
                                double num2;
                                this.ComputeItemSpacing(childCount, abstractWidth1, abstractWidth2, abstractWidth3, out num1, out num2);
                                int index3 = 0;
                                while (index3 < row.ChildCount)
                                {
                                        UIElement uiElement = array[index1];
                                        AbstractSize childSize = row.ChildSizes[index3];
                                        Rect finalRect = new Rect(abstractPoint.RealPoint, childSize.RealSize);
                                        uiElement.Arrange(finalRect);
                                        double num3 = num1 + ((double)index3 < num2 ? 1.0 : 0.0);
                                        abstractPoint.AbstractX += childSize.AbstractWidth + num3;
                                        ++index3;
                                        ++index1;
                                }
                                double num4 = itemSpacing + ((double)index2 < remainder ? 1.0 : 0.0);
                                abstractPoint.AbstractX = 0.0;
                                ref AbstractPoint local3 = ref abstractPoint;
                                double abstractY = local3.AbstractY;
                                abstractSize2 = row.AbstractSize;
                                double num5 = abstractSize2.AbstractHeight + num4;
                                local3.AbstractY = abstractY + num5;
                        }
                        return constraint;
                }

                private void ComputeItemSpacing(int itemCount, double totalSpace, double occupiedSpace, double nominalItemSpacing, out double itemSpacing, out double remainder)
                {
                        bool flag = !double.IsNaN(nominalItemSpacing);
                        itemSpacing = flag ? nominalItemSpacing : 0.0;
                        remainder = 0.0;
                        if (itemCount <= 1 || flag)
                                return;
                        double num = totalSpace - occupiedSpace;
                        itemSpacing = num / (double)(itemCount - 1);
                        if (!this.UseLayoutRounding)
                                return;
                        ref double local = ref itemSpacing;
                        local = (double)(int)local;
                        remainder = (double)(int)(num % (double)(itemCount - 1));
                }

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

                private AbstractSize MakeAbstractSize()
                {
                        return new AbstractSize(Orientation.Horizontal, this.Orientation);
                }

                private AbstractSize MakeAbstractSize(Size size)
                {
                        return new AbstractSize(Orientation.Horizontal, this.Orientation, size);
                }

                private AbstractSize MakeAbstractSize(double width, double height)
                {
                        return new AbstractSize(Orientation.Horizontal, this.Orientation, width, height);
                }

                private AbstractPoint MakeAbstractPoint()
                {
                        return new AbstractPoint(Orientation.Horizontal, this.Orientation);
                }

                private class MeasureInfo
                {
                        private readonly List<SpacingWrapPanel.RowInfo> _rows = new List<SpacingWrapPanel.RowInfo>();
                        private readonly SpacingWrapPanel _panel;
                        private AbstractSize _size;
                        private double _rowSpacing;

                        public MeasureInfo(SpacingWrapPanel panel)
                        {
                                this._panel = panel;
                        }

                        public List<SpacingWrapPanel.RowInfo> Rows
                        {
                                get
                                {
                                        return this._rows;
                                }
                        }

                        public int RowCount
                        {
                                get
                                {
                                        return this._rows.Count;
                                }
                        }

                        public AbstractSize AbstractSize
                        {
                                get
                                {
                                        return this._size;
                                }
                        }

                        public void Reset()
                        {
                                AbstractSize abstractItemSpacing = this._panel.AbstractItemSpacing;
                                this._size = this._panel.MakeAbstractSize();
                                this._rowSpacing = double.IsNaN(abstractItemSpacing.AbstractHeight) ? 0.0 : abstractItemSpacing.AbstractHeight;
                                this._rows.Clear();
                        }

                        public void AddRow(SpacingWrapPanel.RowInfo row)
                        {
                                AbstractSize abstractSize = row.AbstractSize;
                                double num1 = this._size.AbstractHeight + (abstractSize.AbstractHeight + (this._rows.Count > 0 ? this._rowSpacing : 0.0));
                                this._rows.Add(row);
                                ref AbstractSize local = ref this._size;
                                double abstractWidth1 = this._size.AbstractWidth;
                                abstractSize = row.AbstractSize;
                                double abstractWidth2 = abstractSize.AbstractWidth;
                                double num2 = Math.Max(abstractWidth1, abstractWidth2);
                                local.AbstractWidth = num2;
                                this._size.AbstractHeight = num1;
                        }
                }

                private class RowInfo
                {
                        private readonly List<AbstractSize> _childSizes = new List<AbstractSize>();
                        private readonly double _itemSpacing;
                        private readonly double _maxWidth;
                        private AbstractSize _size;

                        public RowInfo(SpacingWrapPanel panel, double maxWidth)
                        {
                                AbstractSize abstractItemSpacing = panel.AbstractItemSpacing;
                                this._size = panel.MakeAbstractSize();
                                this._itemSpacing = double.IsNaN(abstractItemSpacing.AbstractWidth) ? 0.0 : abstractItemSpacing.AbstractWidth;
                                this._maxWidth = maxWidth;
                        }

                        public int ChildCount
                        {
                                get
                                {
                                        return this._childSizes.Count;
                                }
                        }

                        public List<AbstractSize> ChildSizes
                        {
                                get
                                {
                                        return this._childSizes;
                                }
                        }

                        public AbstractSize AbstractSize
                        {
                                get
                                {
                                        return this._size;
                                }
                        }

                        public bool AddChild(AbstractSize childSize)
                        {
                                double num = this._size.AbstractWidth + (childSize.AbstractWidth + (this.ChildCount > 0 ? this._itemSpacing : 0.0));
                                if (this.ChildCount > 0 && num > this._maxWidth)
                                        return false;
                                this._childSizes.Add(childSize);
                                this._size.AbstractWidth = num;
                                this._size.AbstractHeight = Math.Max(this._size.AbstractHeight, childSize.AbstractHeight);
                                return true;
                        }
                }
        }
}
