﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UltimatePresentation.Presentation
{
        /// <summary>
        /// 居中的WrapPanel
        /// </summary>
        public class GreedyWrapPanel : Panel
        {
                public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(GreedyWrapPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));
                public static readonly DependencyProperty MaxWrappingLevelsProperty = DependencyProperty.Register(nameof(MaxWrappingLevels), typeof(int), typeof(GreedyWrapPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)int.MaxValue, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange), new ValidateValueCallback(GreedyWrapPanel.ValidateWrappingLevels));
                private static readonly DependencyPropertyKey ActualWrappingLevelsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ActualWrappingLevels), typeof(int), typeof(GreedyWrapPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)1), new ValidateValueCallback(GreedyWrapPanel.ValidateWrappingLevels));
                public static readonly DependencyProperty ActualWrappingLevelsProperty = GreedyWrapPanel.ActualWrappingLevelsPropertyKey.DependencyProperty;
                private const Orientation NaturalOrientation = Orientation.Vertical;

                public Orientation Orientation
                {
                        get
                        {
                                return (Orientation)this.GetValue(GreedyWrapPanel.OrientationProperty);
                        }
                        set
                        {
                                this.SetValue(GreedyWrapPanel.OrientationProperty, (object)value);
                        }
                }

                public int MaxWrappingLevels
                {
                        get
                        {
                                return (int)this.GetValue(GreedyWrapPanel.MaxWrappingLevelsProperty);
                        }
                        set
                        {
                                this.SetValue(GreedyWrapPanel.MaxWrappingLevelsProperty, (object)value);
                        }
                }

                public int ActualWrappingLevels
                {
                        get
                        {
                                return (int)this.GetValue(GreedyWrapPanel.ActualWrappingLevelsProperty);
                        }
                        protected set
                        {
                                this.SetValue(GreedyWrapPanel.ActualWrappingLevelsPropertyKey, (object)value);
                        }
                }

                private static bool ValidateWrappingLevels(object value)
                {
                        return (int)value > 0;
                }

                protected override Size MeasureOverride(Size availableSize)
                {
                        this.MeasureNonNullChildren();
                        List<AbstractSize> collapsedChildrenSizes = this.GetNonCollapsedChildrenSizes();
                        if (collapsedChildrenSizes.Count == 0)
                                return new Size(0.0, 0.0);
                        AbstractSize abstractSize = this.ConvertToAbstractSize(availableSize);
                        GreedyWrapPanel.Measurer measurer = new GreedyWrapPanel.Measurer(collapsedChildrenSizes, abstractSize, this.MaxWrappingLevels);
                        this.ActualWrappingLevels = measurer.ActualWrappingLevels;
                        return measurer.TotalSize.RealSize;
                }

                private void MeasureNonNullChildren()
                {
                        Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
                        foreach (UIElement internalChild in this.InternalChildren)
                                internalChild?.Measure(availableSize);
                }

                private List<AbstractSize> GetNonCollapsedChildrenSizes()
                {
                        return this.NonCollapsedChildren.Cast<UIElement>().Select<UIElement, AbstractSize>((Func<UIElement, AbstractSize>)(child => this.ConvertToAbstractSize(child.DesiredSize))).ToList<AbstractSize>();
                }

                private IEnumerable<UIElement> NonCollapsedChildren
                {
                        get
                        {
                                return this.InternalChildren.Cast<UIElement>().Where<UIElement>(new Func<UIElement, bool>(this.ChildIsNonNullAndNonCollapsed));
                        }
                }

                private bool ChildIsNonNullAndNonCollapsed(UIElement child)
                {
                        if (child != null)
                                return child.Visibility != Visibility.Collapsed;
                        return false;
                }

                private AbstractSize ConvertToAbstractSize(Size realSize)
                {
                        return new AbstractSize(Orientation.Vertical, this.Orientation, realSize);
                }

                protected override Size ArrangeOverride(Size finalSize)
                {
                        int totalCount = this.NonCollapsedChildren.Count<UIElement>();
                        if (totalCount == 0)
                                return finalSize;
                        GreedyWrapPanel.ChildArranger childArranger = new GreedyWrapPanel.ChildArranger(new AbstractPoint(Orientation.Vertical, this.Orientation, 0.0, 0.0), GreedyWrapPanel.CalculateCountPerWrappingLevel(totalCount, this.ActualWrappingLevels));
                        foreach (UIElement nonCollapsedChild in this.NonCollapsedChildren)
                        {
                                AbstractSize abstractSize = this.ConvertToAbstractSize(nonCollapsedChild.DesiredSize);
                                AbstractPoint positionOfNextChild = childArranger.GetPositionOfNextChild(abstractSize);
                                nonCollapsedChild.Arrange(new Rect(positionOfNextChild.RealPoint, nonCollapsedChild.DesiredSize));
                        }
                        return childArranger.TotalSize.RealSize;
                }

                internal static int CalculateCountPerWrappingLevel(int totalCount, int wrappingLevels)
                {
                        return (int)Math.Ceiling((double)totalCount / (double)wrappingLevels);
                }

                internal class Measurer
                {
                        private List<AbstractSize> _childSizes;
                        private double _availableWidth;
                        private int _maxWrappingLevels;
                        private int _actualWrappingLevels;
                        private AbstractSize _totalSize;

                        public Measurer(List<AbstractSize> childSizes, AbstractSize availableSize, int maxWrappingLevels)
                        {
                                this._childSizes = childSizes;
                                this._availableWidth = availableSize.AbstractWidth;
                                this._maxWrappingLevels = maxWrappingLevels;
                                this._totalSize = new AbstractSize(availableSize.NaturalOrientation, availableSize.ActualOrientation, 0.0, 0.0);
                                this._actualWrappingLevels = 0;
                                this.CalculateTotalSizeAndUpdateActualWrappingLevels();
                        }

                        private void CalculateTotalSizeAndUpdateActualWrappingLevels()
                        {
                                this._totalSize.AbstractWidth = this.CalculateWidthAndWrappingLevelCount(this.EstimateMaxWrappingLevels());
                                this._totalSize.AbstractHeight = this.CalculateHeight();
                        }

                        private int EstimateMaxWrappingLevels()
                        {
                                int val1 = Math.Min(this._maxWrappingLevels, this._childSizes.Count);
                                if (!double.IsPositiveInfinity(this._availableWidth) && this._childSizes.Count != 0)
                                {
                                        double num = this._childSizes.Min<AbstractSize>((Func<AbstractSize, double>)(child => child.AbstractWidth));
                                        if (num != 0.0)
                                                val1 = Math.Min(val1, Convert.ToInt32(this._availableWidth / num));
                                }
                                return Math.Max(val1, 1);
                        }

                        private double CalculateWidthAndWrappingLevelCount(int estimatedMaxWrappingLevels)
                        {
                                double totalWidth = 0.0;
                                int num;
                                for (num = estimatedMaxWrappingLevels; num > 0; --num)
                                {
                                        totalWidth = this.GetSumOfWrappingLevelWidths(num, totalWidth);
                                        if (totalWidth <= this._availableWidth)
                                                break;
                                }
                                this._actualWrappingLevels = Math.Max(num, 1);
                                return totalWidth;
                        }

                        private double GetSumOfWrappingLevelWidths(int wrappingLevels, double totalWidth)
                        {
                                double num = 0.0;
                                int perWrappingLevel = GreedyWrapPanel.CalculateCountPerWrappingLevel(this._childSizes.Count, wrappingLevels);
                                int start = 0;
                                while (start < this._childSizes.Count)
                                {
                                        num += this.GetMaxWidthForWrappingLevel(start, perWrappingLevel);
                                        start += perWrappingLevel;
                                }
                                return num;
                        }

                        private double CalculateHeight()
                        {
                                int perWrappingLevel = GreedyWrapPanel.CalculateCountPerWrappingLevel(this._childSizes.Count, this._actualWrappingLevels);
                                double num = 0.0;
                                int start = 0;
                                while (start < this._childSizes.Count)
                                {
                                        double forWrappingLevel = this.GetHeightForWrappingLevel(start, perWrappingLevel);
                                        if (forWrappingLevel > num)
                                                num = forWrappingLevel;
                                        start += perWrappingLevel;
                                }
                                return num;
                        }

                        private double GetHeightForWrappingLevel(int start, int childrenPerWrappingLevel)
                        {
                                return GreedyWrapPanel.Measurer.SubList<AbstractSize>(this._childSizes, start, childrenPerWrappingLevel).Sum<AbstractSize>((Func<AbstractSize, double>)(size => size.AbstractHeight));
                        }

                        private static IEnumerable<T> SubList<T>(List<T> list, int start, int count)
                        {
                                for (int i = start; i < list.Count && i < start + count; ++i)
                                        yield return list[i];
                        }

                        private double GetMaxWidthForWrappingLevel(int start, int childrenPerWrappingLevel)
                        {
                                return GreedyWrapPanel.Measurer.SubList<AbstractSize>(this._childSizes, start, childrenPerWrappingLevel).Max<AbstractSize>((Func<AbstractSize, double>)(size => size.AbstractWidth));
                        }

                        public AbstractSize TotalSize
                        {
                                get
                                {
                                        return this._totalSize;
                                }
                        }

                        public int ActualWrappingLevels
                        {
                                get
                                {
                                        return this._actualWrappingLevels;
                                }
                        }
                }

                internal class ChildArranger
                {
                        private int _childrenPerWrappingLevel;
                        private int _currentWrappingLevelChildrenCount;
                        private double _currentWrappingLevelWidth;
                        private AbstractPoint _currentOffset;
                        private double _biggestHeight;

                        public ChildArranger(AbstractPoint startingOffset, int childrenPerWrappingLevel)
                        {
                                this._childrenPerWrappingLevel = childrenPerWrappingLevel;
                                this._currentOffset = startingOffset;
                                this._currentWrappingLevelChildrenCount = 0;
                                this._currentWrappingLevelWidth = 0.0;
                                this._biggestHeight = 0.0;
                        }

                        public AbstractPoint GetPositionOfNextChild(AbstractSize childSize)
                        {
                                if (this._currentWrappingLevelChildrenCount >= this._childrenPerWrappingLevel)
                                        this.ShiftToNewWrappingLevel();
                                AbstractPoint currentOffset = this._currentOffset;
                                if (childSize.AbstractWidth > this._currentWrappingLevelWidth)
                                        this._currentWrappingLevelWidth = childSize.AbstractWidth;
                                this.ShiftToBelowPreviousChild(childSize.AbstractHeight);
                                return currentOffset;
                        }

                        private void ShiftToNewWrappingLevel()
                        {
                                this._currentWrappingLevelChildrenCount = 0;
                                this._currentOffset.AbstractX += this._currentWrappingLevelWidth;
                                this._currentWrappingLevelWidth = 0.0;
                                this._currentOffset.AbstractY = 0.0;
                        }

                        private void ShiftToBelowPreviousChild(double previousChildHeight)
                        {
                                this._currentOffset.AbstractY += previousChildHeight;
                                ++this._currentWrappingLevelChildrenCount;
                                if (this._currentOffset.AbstractY <= this._biggestHeight)
                                        return;
                                this._biggestHeight = this._currentOffset.AbstractY;
                        }

                        public AbstractSize TotalSize
                        {
                                get
                                {
                                        return new AbstractSize(this._currentOffset.NaturalOrientation, this._currentOffset.ActualOrientation)
                                        {
                                                AbstractWidth = this._currentOffset.AbstractX + this._currentWrappingLevelWidth,
                                                AbstractHeight = this._biggestHeight
                                        };
                                }
                        }
                }
        }

        public struct AbstractSize
        {
                private readonly Orientation _naturalOrientation;
                private readonly Orientation _actualOrientation;
                private Size _abstractSize;

                public AbstractSize(Orientation naturalOrientation, Orientation actualOrientation)
                {
                        this._naturalOrientation = naturalOrientation;
                        this._actualOrientation = actualOrientation;
                        this._abstractSize = new Size(0.0, 0.0);
                }

                public AbstractSize(Orientation naturalOrientation, Orientation actualOrientation, Size realSize)
                {
                        this = new AbstractSize(naturalOrientation, actualOrientation);
                        this._abstractSize = this.IsNatural ? realSize : AbstractSize.Invert(realSize);
                }

                public AbstractSize(Orientation naturalOrientation, Orientation actualOrientation, double realWidth, double realHeight)
                {
                        this = new AbstractSize(naturalOrientation, actualOrientation);
                        this._abstractSize = this.IsNatural ? new Size(realWidth, realHeight) : new Size(realHeight, realWidth);
                }

                public Orientation NaturalOrientation
                {
                        get
                        {
                                return this._naturalOrientation;
                        }
                }

                public Orientation ActualOrientation
                {
                        get
                        {
                                return this._actualOrientation;
                        }
                }

                public bool IsNatural
                {
                        get
                        {
                                return this._naturalOrientation == this._actualOrientation;
                        }
                }

                public double AbstractWidth
                {
                        get
                        {
                                return this._abstractSize.Width;
                        }
                        set
                        {
                                this._abstractSize.Width = value;
                        }
                }

                public double AbstractHeight
                {
                        get
                        {
                                return this._abstractSize.Height;
                        }
                        set
                        {
                                this._abstractSize.Height = value;
                        }
                }

                public Size RealSize
                {
                        get
                        {
                                if (!this.IsNatural)
                                        return AbstractSize.Invert(this._abstractSize);
                                return this._abstractSize;
                        }
                        set
                        {
                                this._abstractSize = this.IsNatural ? value : AbstractSize.Invert(value);
                        }
                }

                public double RealWidth
                {
                        get
                        {
                                if (!this.IsNatural)
                                        return this.AbstractHeight;
                                return this.AbstractWidth;
                        }
                        set
                        {
                                if (this.IsNatural)
                                        this.AbstractWidth = value;
                                else
                                        this.AbstractHeight = value;
                        }
                }

                public double RealHeight
                {
                        get
                        {
                                if (!this.IsNatural)
                                        return this.AbstractWidth;
                                return this.AbstractHeight;
                        }
                        set
                        {
                                if (this.IsNatural)
                                        this.AbstractHeight = value;
                                else
                                        this.AbstractWidth = value;
                        }
                }

                public override string ToString()
                {
                        return string.Format("Abstract: {0}  Real: {1}", (object)this._abstractSize, (object)this.RealSize);
                }

                public static Size Invert(Size size)
                {
                        return new Size(size.Height, size.Width);
                }
        }
        public struct AbstractPoint
        {
                private readonly Orientation _naturalOrientation;
                private readonly Orientation _actualOrientation;
                private Point _abstractPoint;

                public AbstractPoint(Orientation naturalOrientation, Orientation actualOrientation)
                {
                        this._naturalOrientation = naturalOrientation;
                        this._actualOrientation = actualOrientation;
                        this._abstractPoint = new Point(0.0, 0.0);
                }

                public AbstractPoint(Orientation naturalOrientation, Orientation actualOrientation, Point realPoint)
                {
                        this = new AbstractPoint(naturalOrientation, actualOrientation);
                        this._abstractPoint = this.IsNatural ? realPoint : AbstractPoint.Invert(realPoint);
                }

                public AbstractPoint(Orientation naturalOrientation, Orientation actualOrientation, double realX, double realY)
                {
                        this = new AbstractPoint(naturalOrientation, actualOrientation);
                        this._abstractPoint = this.IsNatural ? new Point(realX, realY) : new Point(realY, realX);
                }

                public Orientation NaturalOrientation
                {
                        get
                        {
                                return this._naturalOrientation;
                        }
                }

                public Orientation ActualOrientation
                {
                        get
                        {
                                return this._actualOrientation;
                        }
                }

                public bool IsNatural
                {
                        get
                        {
                                return this._naturalOrientation == this._actualOrientation;
                        }
                }

                public double AbstractX
                {
                        get
                        {
                                return this._abstractPoint.X;
                        }
                        set
                        {
                                this._abstractPoint.X = value;
                        }
                }

                public double AbstractY
                {
                        get
                        {
                                return this._abstractPoint.Y;
                        }
                        set
                        {
                                this._abstractPoint.Y = value;
                        }
                }

                public Point RealPoint
                {
                        get
                        {
                                if (!this.IsNatural)
                                        return AbstractPoint.Invert(this._abstractPoint);
                                return this._abstractPoint;
                        }
                        set
                        {
                                this._abstractPoint = this.IsNatural ? value : AbstractPoint.Invert(value);
                        }
                }

                public double RealX
                {
                        get
                        {
                                if (!this.IsNatural)
                                        return this.AbstractY;
                                return this.AbstractX;
                        }
                        set
                        {
                                if (this.IsNatural)
                                        this.AbstractX = value;
                                else
                                        this.AbstractY = value;
                        }
                }

                public double RealY
                {
                        get
                        {
                                if (!this.IsNatural)
                                        return this.AbstractX;
                                return this.AbstractY;
                        }
                        set
                        {
                                if (this.IsNatural)
                                        this.AbstractY = value;
                                else
                                        this.AbstractX = value;
                        }
                }

                public override string ToString()
                {
                        return string.Format("Abstract: {0}  Real: {1}", (object)this._abstractPoint, (object)this.RealPoint);
                }

                public static Point Invert(Point point)
                {
                        return new Point(point.Y, point.X);
                }
        }
}
