using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using UltimateCore;

namespace UltimatePresentation.Presentation
{
        public class StretchVirtualizingWrapPanel : VirtualizingPanel, IScrollInfo
        {
                public double ItemWidth
                {
                        get
                        {
                                return (double)GetValue(ItemWidthProperty);
                        }
                        set
                        {
                                SetValue(ItemWidthProperty, value);
                        }
                }
                public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(StretchVirtualizingWrapPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

                public double ItemHeight
                {
                        get
                        {
                                return (double)GetValue(ItemHeightProperty);
                        }
                        set
                        {
                                SetValue(ItemHeightProperty, value);
                        }
                }
                public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(StretchVirtualizingWrapPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

                public int Rows
                {
                        get
                        {
                                return (int)GetValue(RowsProperty);
                        }
                        set
                        {
                                SetValue(RowsProperty, value);
                        }
                }
                public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(StretchVirtualizingWrapPanel), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

                public int Columns
                {
                        get
                        {
                                return (int)GetValue(ColumnsProperty);
                        }
                        set
                        {
                                SetValue(ColumnsProperty, value);
                        }
                }
                public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(StretchVirtualizingWrapPanel), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));


                public int LineIndex
                {
                        get
                        {
                                return (int)GetValue(LineIndexProperty);
                        }
                        set
                        {
                                SetValue(LineIndexProperty, value);
                        }
                }
                public static readonly DependencyProperty LineIndexProperty = DependencyProperty.Register("LineIndex", typeof(int), typeof(StretchVirtualizingWrapPanel), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, LineIndexChangedCallBack, LineIndexCoerceValueCallback));
                private static void LineIndexChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {

                }
                private static object LineIndexCoerceValueCallback(DependencyObject d, object baseValue)
                {
                        int value = (int)baseValue;
                        if (value < 0)
                        {
                                return 0;
                        }

                        StretchVirtualizingWrapPanel panel = (StretchVirtualizingWrapPanel)d;
                        if (value >= panel.LineCount - 1)
                        {
                                return panel.LineCount - 1;
                        }

                        return baseValue;
                }

                public int LineCount
                {
                        get
                        {
                                return (int)GetValue(LineCountProperty);
                        }
                        private set
                        {
                                SetValue(LineCountProperty, value);
                        }
                }
                public static readonly DependencyProperty LineCountProperty = DependencyProperty.Register("LineCount", typeof(int), typeof(StretchVirtualizingWrapPanel), new PropertyMetadata(0));


                private TimerBuffer<object> _buffer = null;
                private bool _isMeasureArrangedFirstFlag = false;

                public StretchVirtualizingWrapPanel()
                {
                        _buffer = new TimerBuffer<object>(DispatcherPriority.Loaded);
                        _buffer.DueTime = 50;
                        _buffer.Action = this.MeasureArrange;
                }

                private void MeasureArrange(object obj)
                {
                        this._isMeasureArrangedFirstFlag = true;
                        this.MeasureCore();
                        this.ArrangeCore();
                }

                private bool CheckItemWidth()
                {
                        return !double.IsInfinity(this.ItemWidth)
                                        && !double.IsNaN(this.ItemWidth)
                                        && this.ItemWidth > 0;
                }

                private bool CheckItemHeight()
                {
                        return !double.IsInfinity(this.ItemHeight)
                                        && !double.IsNaN(this.ItemHeight)
                                        && this.ItemHeight > 0;
                }

                protected override Size MeasureOverride(Size availableSize)
                {
                        if (double.IsInfinity(availableSize.Width)
                            && !this.CheckItemWidth())
                        {
                                throw new Exception("无法确定元素占用宽度，请指定ItemWidth。");
                        }

                        if (double.IsInfinity(availableSize.Height)
                                && !this.CheckItemHeight())
                        {
                                throw new Exception("无法确定元素占用宽度，请指定ItemHeight。");
                        }

                        var itemsControl = ItemsControl.GetItemsOwner(this);

                        this._sumCount = itemsControl != null && itemsControl.HasItems ? itemsControl.Items.Count : 0;
                        //this._sumCount = itemsControl != null && itemsControl.HasItems ? itemsControl.Items.Cast<object>().Where(_p => itemsControl.Items.Filter == null || itemsControl.Items.Filter(_p)).Count() : 0;

                        bool flag = false;

                        if (double.IsInfinity(availableSize.Width)
                               && double.IsInfinity(availableSize.Height))
                        {
                                flag = true;

                                availableSize = new Size(this._sumCount * this.ItemWidth, this.ItemHeight);
                        }
                        else if (double.IsInfinity(availableSize.Width))
                        {
                                flag = true;

                                availableSize = new Size(this._sumCount * this.ItemWidth, availableSize.Height);
                        }
                        else if (double.IsInfinity(availableSize.Height))
                        {
                                flag = true;

                                availableSize = new Size(availableSize.Width, this._sumCount * this.ItemHeight);
                        }

                        if (this.Rows > 0)
                        {
                                if (this.Columns > 0)
                                {
                                        this._rows = this.Rows;
                                        this._columns = this.Columns;
                                }
                                else if (this.CheckItemWidth())
                                {
                                        this._rows = this.Rows;
                                        this._columns = Math.Max(1, (int)(availableSize.Width / this.ItemWidth));
                                }
                                else
                                {
                                        throw new Exception("无法确定子项大小。");
                                }
                        }
                        else
                        {
                                if (this.Columns > 0)
                                {
                                        if (this.CheckItemHeight())
                                        {
                                                this._rows = Math.Max(1, (int)(availableSize.Height / this.ItemHeight));
                                                this._columns = this.Columns;
                                        }
                                        else
                                        {
                                                throw new Exception("无法确定子项大小。");
                                        }
                                }
                                else
                                {
                                        if (!this.CheckItemHeight()
                                                || !this.CheckItemWidth())
                                        {
                                                throw new Exception("无法确定子项大小。");
                                        }

                                        this._rows = Math.Max(1, (int)(availableSize.Height / this.ItemHeight));
                                        this._columns = Math.Max(1, (int)(availableSize.Width / this.ItemWidth));
                                }
                        }

                        this._eachSize = new Size(availableSize.Width / this._columns, availableSize.Height / this._rows);

                        this._eachPageCount = this._rows * this._columns;


                        int maxpage = this._sumCount / this._eachPageCount;
                        if (this._sumCount % this._eachPageCount > 0)
                        {
                                maxpage++;
                        }
                        this._pageCount = maxpage;


                        int maxline = this._sumCount / this._columns;
                        if (this._sumCount % this._columns > 0)
                        {
                                maxline++;
                        }
                        this.LineCount = maxline;

                        int currentlineindex = Math.Max(0, Math.Min(this.LineIndex, maxline - 1));

                        this._startIndex = currentlineindex * this._columns;

                        this._endIndex = Math.Min(this._startIndex + this._eachPageCount - 1, this._sumCount - 1);

                        //this._currentPageCount = Math.Min(this._eachPageCount, this._sumCount - currentpageindex * this._eachPageCount);

                        if (this._isMeasureArrangedFirstFlag)
                        {
                                this.MeasureCore();
                        }
                        else
                        {
                                this._buffer.ReSet(null);
                        }

                        if (flag)
                        {
                                this._rows = (int)Math.Ceiling((double)this._sumCount / this._columns);

                                availableSize = new Size(this._eachSize.Width * this._columns, this._eachSize.Height * this._rows);
                        }

                        this._actualSize = availableSize;

                        this.UpdateScrollInfo();

                        return availableSize;
                }
                private void MeasureCore()
                {
                        // 注意，在第一次使用 ItemContainerGenerator之前要先访问一下InternalChildren, 
                        // 否则ItemContainerGenerator为null，是一个Bug
                        UIElementCollection children = InternalChildren;
                        IItemContainerGenerator generator = ItemContainerGenerator;

                        if (generator != null)
                        {
                                // 获取第一个可视元素位置信息
                                GeneratorPosition position = generator.GeneratorPositionFromIndex(this._startIndex);
                                // 根据元素位置信息计算子元素索引
                                int childIndex = position.Offset == 0 ? position.Index : position.Index + 1;

                                using (generator.StartAt(position, GeneratorDirection.Forward, true))
                                {
                                        for (int itemIndex = this._startIndex; itemIndex <= this._endIndex; itemIndex++, childIndex++)
                                        {
                                                bool isNewlyRealized;   // 用以指示新生成的元素是否是新实体化的

                                                // 生成下一个子元素
                                                var child = (UIElement)generator.GenerateNext(out isNewlyRealized);

                                                if (child == null)
                                                {
                                                        break;
                                                }

                                                if (isNewlyRealized)
                                                {
                                                        if (childIndex >= children.Count)
                                                        {
                                                                AddInternalChild(child);
                                                        }
                                                        else
                                                        {
                                                                InsertInternalChild(childIndex, child);
                                                        }
                                                        generator.PrepareItemContainer(child);
                                                }

                                                // 测算子元素布局
                                                child.Measure(this._eachSize);
                                        }
                                }
                        }

                        this.CleanUpItems(this._startIndex, this._endIndex);
                }

                protected override void BringIndexIntoView(int index)
                {
                        base.BringIndexIntoView(index);
                }

                private void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated)
                {
                        UIElementCollection children = this.InternalChildren;
                        IItemContainerGenerator generator = this.ItemContainerGenerator;

                        // 清除不需要显示的子元素，注意从集合后向前操作，以免造成操作过程中元素索引发生改变
                        for (int i = children.Count - 1; i > -1; i--)
                        {
                                // 通过已显示的子元素的位置信息得出元素索引
                                var childGeneratorPos = new GeneratorPosition(i, 0);
                                int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);

                                // 移除不再显示的元素
                                if (itemIndex < minDesiredGenerated || itemIndex > maxDesiredGenerated)
                                {
                                        generator.Remove(childGeneratorPos, 1);
                                        RemoveInternalChildRange(i, 1);
                                }
                        }
                }

                private void UpdateScrollInfo()
                {
                        //this.ExtentWidth = _actualSize.Width * this.PageCount;
                        this.ExtentHeight = _actualSize.Height * this._pageCount;

                        if (this.ScrollOwner != null
                              && (this.CanHorizontallyScroll || this.CanVerticallyScroll))
                        {
                                this.ScrollOwner.InvalidateScrollInfo();
                        }
                }

                private int _rows, _columns;
                private Size _eachSize;
                private int _sumCount = 0, _eachPageCount = 0, _pageCount = 0;
                private int _startIndex = 0, _endIndex = 0;
                private int _currentPageCount = 0;

                protected override Size ArrangeOverride(Size finalSize)
                {
                        if (this._isMeasureArrangedFirstFlag)
                        {
                                this.ArrangeCore();
                        }
                        else
                        {
                                this._buffer.ReSet(null);
                        }

                        return finalSize;
                }
                private void ArrangeCore()
                {
                        int rowindex = 0,
                            columnindex = 0;

                        if (this.InternalChildren != null)
                        {
                                foreach (UIElement child in this.InternalChildren)
                                {
                                        double x = columnindex * _eachSize.Width;
                                        double y = rowindex * _eachSize.Height;

                                        child.Arrange(new Rect(new Point(x, y), _eachSize));

                                        if (columnindex == this._columns - 1)
                                        {
                                                columnindex = 0;
                                                rowindex++;
                                                continue;
                                        }

                                        columnindex++;
                                }
                        }
                }

                private Size _actualSize;

                private void UpdateScrollSize()
                {
                        //this.ExtentWidth = _actualSize.Width * this.PageCount;
                        this.ExtentHeight = _actualSize.Height * this._pageCount;
                }

                public bool CanVerticallyScroll
                {
                        get;
                        set;
                }
                public bool CanHorizontallyScroll
                {
                        get;
                        set;
                }

                public double ExtentWidth
                {
                        get;
                        private set;
                }

                public double ExtentHeight
                {
                        get;
                        private set;
                }

                public double ViewportWidth => _actualSize.Width;

                public double ViewportHeight => _actualSize.Height;

                public double HorizontalOffset
                {
                        get;
                        set;
                }
                //public double HorizontalOffset
                //{
                //        get => this.PageIndex * this.ViewportWidth;
                //        private set
                //        {
                //                double count = value / this.ExtentWidth * this.PageCount;
                //                count = Math.Round(count, 0, MidpointRounding.AwayFromZero);
                //                this.PageIndex = (int)count;
                //        }
                //}

                public double VerticalOffset
                {
                        get => this.LineIndex * this._eachSize.Height;
                        private set
                        {
                                double count = value / this.ExtentHeight * this._eachSize.Height;
                                count = Math.Round(count, 0, MidpointRounding.AwayFromZero);
                                this.LineIndex = (int)count;
                        }
                }

                private ScrollViewer _scrollOwner;
                public ScrollViewer ScrollOwner
                {
                        get => this._scrollOwner;
                        set => this._scrollOwner = value;
                }

                public void LineUp()
                {
                }

                public void LineDown()
                {
                }

                public void LineLeft()
                {
                }

                public void LineRight()
                {
                }

                public void PageUp()
                {
                        this.LineIndex--;
                }

                public void PageDown()
                {
                        this.LineIndex++;
                }

                public void PageLeft()
                {
                        //this.PageIndex--;
                }

                public void PageRight()
                {
                        //this.PageIndex++;
                }

                public void MouseWheelUp()
                {
                        this.LineIndex--;
                }

                public void MouseWheelDown()
                {
                        this.LineIndex++;
                }

                public void MouseWheelLeft()
                {
                        //this.PageIndex--;
                }

                public void MouseWheelRight()
                {
                        //this.PageIndex++;
                }

                public void SetHorizontalOffset(double offset)
                {
                        //this.HorizontalOffset = offset;
                }

                public void SetVerticalOffset(double offset)
                {
                        this.VerticalOffset = offset;
                }

                public Rect MakeVisible(Visual visual, Rect rectangle)
                {
                        return rectangle;
                }
        }
}
