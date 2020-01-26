using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using UltimateCore;
using UltimatePresentation.Native;

namespace UltimatePresentation.Presentation
{
        public class SplitterGrip : Thumb
        {
                public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(SplitterGrip), (PropertyMetadata)new FrameworkPropertyMetadata((object)Orientation.Vertical));
                public static readonly DependencyProperty ResizeBehaviorProperty = DependencyProperty.Register(nameof(ResizeBehavior), typeof(GridResizeBehavior), typeof(SplitterGrip), (PropertyMetadata)new FrameworkPropertyMetadata((object)GridResizeBehavior.CurrentAndNext));

                static SplitterGrip()
                {
                        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterGrip), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(SplitterGrip)));
                }

                public SplitterGrip()
                {
                        AutomationProperties.SetAutomationId((DependencyObject)this, nameof(SplitterGrip));
                }

                public Orientation Orientation
                {
                        get
                        {
                                return (Orientation)this.GetValue(SplitterGrip.OrientationProperty);
                        }
                        set
                        {
                                this.SetValue(SplitterGrip.OrientationProperty, (object)value);
                        }
                }

                public GridResizeBehavior ResizeBehavior
                {
                        get
                        {
                                return (GridResizeBehavior)this.GetValue(SplitterGrip.ResizeBehaviorProperty);
                        }
                        set
                        {
                                this.SetValue(SplitterGrip.ResizeBehaviorProperty, (object)value);
                        }
                }
        }

        public class SplitterMeasureData
        {
                public SplitterMeasureData(UIElement element)
                {
                        this.Element = element;
                        this.AttachedLength = SplitterPanel.GetSplitterLength(element);
                }

                public static IList<SplitterMeasureData> FromElements(IList elements)
                {
                        List<SplitterMeasureData> splitterMeasureDataList = new List<SplitterMeasureData>(elements.Count);
                        foreach (UIElement element in (IEnumerable)elements)
                        {
                                if (element != null)
                                        splitterMeasureDataList.Add(new SplitterMeasureData(element));
                        }
                        return (IList<SplitterMeasureData>)splitterMeasureDataList;
                }

                public UIElement Element { get; private set; }

                public SplitterLength AttachedLength { get; set; }

                public bool IsMinimumReached { get; set; }

                public bool IsMaximumReached { get; set; }

                public Rect MeasuredBounds { get; set; }
        }

        public enum SplitterUnitType
        {
                Fill,
                Stretch,
        }

        public class SplitterLengthConverter : TypeConverter
        {
                public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
                {
                        switch (Type.GetTypeCode(sourceType))
                        {
                                case TypeCode.Int16:
                                case TypeCode.UInt16:
                                case TypeCode.Int32:
                                case TypeCode.UInt32:
                                case TypeCode.Int64:
                                case TypeCode.UInt64:
                                case TypeCode.Single:
                                case TypeCode.Double:
                                case TypeCode.Decimal:
                                case TypeCode.String:
                                        return true;
                                default:
                                        return false;
                        }
                }

                public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
                {
                        if (destinationType != typeof(InstanceDescriptor))
                                return destinationType == typeof(string);
                        return true;
                }

                public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
                {
                        if (value == null || !this.CanConvertFrom(value.GetType()))
                                throw this.GetConvertFromException(value);
                        if (value is string)
                                return (object)SplitterLengthConverter.FromString((string)value, culture);
                        double d = Convert.ToDouble(value, (IFormatProvider)culture);
                        if (double.IsNaN(d))
                                d = 1.0;
                        return (object)new SplitterLength(d, SplitterUnitType.Stretch);
                }

                public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
                {
                        if (destinationType == (Type)null)
                                throw new ArgumentNullException(nameof(destinationType));
                        if (value != null && value is SplitterLength)
                        {
                                SplitterLength length = (SplitterLength)value;
                                if (destinationType == typeof(string))
                                        return (object)SplitterLengthConverter.ToString(length, culture);
                                if (destinationType.IsEquivalentTo(typeof(InstanceDescriptor)))
                                        return (object)new InstanceDescriptor((MemberInfo)typeof(SplitterLength).GetConstructor(new Type[2]
                                        {
                                                typeof (double),
                                                typeof (SplitterUnitType)
                                        }), (ICollection)new object[2]
                                        {
                                                (object) length.Value,
                                                (object) length.SplitterUnitType
                                        });
                        }
                        throw this.GetConvertToException(value, destinationType);
                }

                internal static SplitterLength FromString(string s, CultureInfo cultureInfo)
                {
                        string str = s.Trim();
                        double num = 1.0;
                        SplitterUnitType unitType = SplitterUnitType.Stretch;
                        //if (str == "*")
                        //        unitType = SplitterUnitType.Fill;
                        //else
                        //        num = Convert.ToDouble(str, (IFormatProvider)cultureInfo);
                        if (str.Contains("*"))
                        {
                                unitType = SplitterUnitType.Fill;

                                double va = 0;

                                if (double.TryParse(str.Replace("*", ""), out va)
                                        && va > 1)
                                {
                                        num = va;
                                }
                        }
                        else
                                num = Convert.ToDouble(str, (IFormatProvider)cultureInfo);
                        return new SplitterLength(num, unitType);
                }

                internal static string ToString(SplitterLength length, CultureInfo cultureInfo)
                {
                        if (length.SplitterUnitType == SplitterUnitType.Fill)
                                return "*";
                        return Convert.ToString(length.Value, (IFormatProvider)cultureInfo);
                }
        }

        [TypeConverter(typeof(SplitterLengthConverter))]
        public struct SplitterLength : IEquatable<SplitterLength>
        {
                private double unitValue;
                private SplitterUnitType unitType;

                public SplitterLength(double value)
                        : this(value, SplitterUnitType.Stretch)
                {
                }

                public SplitterLength(double value, SplitterUnitType unitType)
                {
                        this.unitValue = value;
                        this.unitType = unitType;
                }

                public SplitterUnitType SplitterUnitType
                {
                        get
                        {
                                return this.unitType;
                        }
                }

                public double Value
                {
                        get
                        {
                                return this.unitValue;
                        }
                }

                public bool IsFill
                {
                        get
                        {
                                return this.SplitterUnitType == SplitterUnitType.Fill;
                        }
                }

                public bool IsStretch
                {
                        get
                        {
                                return this.SplitterUnitType == SplitterUnitType.Stretch;
                        }
                }

                public static bool operator ==(SplitterLength obj1, SplitterLength obj2)
                {
                        if (obj1.SplitterUnitType == obj2.SplitterUnitType)
                                return obj1.Value == obj2.Value;
                        return false;
                }

                public static bool operator !=(SplitterLength obj1, SplitterLength obj2)
                {
                        return !(obj1 == obj2);
                }

                public override bool Equals(object obj)
                {
                        if (obj is SplitterLength)
                                return this == (SplitterLength)obj;
                        return false;
                }

                public override int GetHashCode()
                {
                        return (int)((int)this.unitValue + this.unitType);
                }

                public bool Equals(SplitterLength other)
                {
                        return this == other;
                }

                public override string ToString()
                {
                        return SplitterLengthConverter.ToString(this, CultureInfo.InvariantCulture);
                }
        }

        public class SplitterResizePreviewWindow : Control
        {
                private HwndSource hwndSource;

                static SplitterResizePreviewWindow()
                {
                        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterResizePreviewWindow), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(SplitterResizePreviewWindow)));
                }

                public void Move(double deviceLeft, double deviceTop)
                {
                        if (this.hwndSource == null)
                                return;
                        NativeMethodsUltimate.SetWindowPos(this.hwndSource.Handle, IntPtr.Zero, (int)deviceLeft, (int)deviceTop, 0, 0, 85);
                }

                public void Show(UIElement parentElement)
                {
                        HwndSource hwndSource = PresentationSource.FromVisual((Visual)parentElement) as HwndSource;
                        this.EnsureWindow(hwndSource == null ? IntPtr.Zero : hwndSource.Handle);
                        Point screen = parentElement.PointToScreen(new Point(0.0, 0.0));
                        Size deviceUnits = parentElement.RenderSize.LogicalToDeviceUnits();
                        NativeMethodsUltimate.SetWindowPos(this.hwndSource.Handle, IntPtr.Zero, (int)screen.X, (int)screen.Y, (int)deviceUnits.Width, (int)deviceUnits.Height, 84);
                }

                public void Hide()
                {
                        using (this.hwndSource)
                                this.hwndSource = (HwndSource)null;
                }

                private void EnsureWindow(IntPtr owner)
                {
                        if (this.hwndSource != null)
                                return;
                        HwndSourceParameters parameters = new HwndSourceParameters(nameof(SplitterResizePreviewWindow));
                        int num = -2013265880;
                        parameters.Width = 0;
                        parameters.Height = 0;
                        parameters.PositionX = 0;
                        parameters.PositionY = 0;
                        parameters.WindowStyle = num;
                        parameters.UsesPerPixelOpacity = true;
                        parameters.ParentWindow = owner;
                        this.hwndSource = new HwndSource(parameters);
                        this.hwndSource.SizeToContent = SizeToContent.Manual;
                        this.hwndSource.RootVisual = (Visual)this;
                }
        }

        public class SplitterPanel : Panel
        {
                public static readonly DependencyProperty SplitterLengthProperty = DependencyProperty.RegisterAttached("SplitterLength", typeof(SplitterLength), typeof(SplitterPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)new SplitterLength(100.0), FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));
                public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(SplitterPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));
                public static readonly DependencyProperty ShowResizePreviewProperty = DependencyProperty.Register(nameof(ShowResizePreview), typeof(bool), typeof(SplitterPanel), (PropertyMetadata)new FrameworkPropertyMetadata(Boxes.BooleanFalse));
                public static readonly DependencyProperty MinimumLengthProperty = DependencyProperty.RegisterAttached("MinimumLength", typeof(double), typeof(SplitterPanel), (PropertyMetadata)new FrameworkPropertyMetadata(0d));
                public static readonly DependencyProperty MaximumLengthProperty = DependencyProperty.RegisterAttached("MaximumLength", typeof(double), typeof(SplitterPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)double.MaxValue));
                private static readonly DependencyPropertyKey ActualSplitterLengthPropertyKey = DependencyProperty.RegisterAttachedReadOnly("ActualSplitterLength", typeof(double), typeof(SplitterPanel), (PropertyMetadata)new FrameworkPropertyMetadata(0d));
                private static readonly DependencyPropertyKey IndexPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Index", typeof(int), typeof(SplitterPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)-1));
                private static readonly DependencyPropertyKey IsFirstPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsFirst", typeof(bool), typeof(SplitterPanel), (PropertyMetadata)new FrameworkPropertyMetadata(Boxes.BooleanFalse));
                private static readonly DependencyPropertyKey IsLastPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsLast", typeof(bool), typeof(SplitterPanel), (PropertyMetadata)new FrameworkPropertyMetadata(Boxes.BooleanFalse));
                public static readonly DependencyProperty ActualSplitterLengthProperty = SplitterPanel.ActualSplitterLengthPropertyKey.DependencyProperty;
                public static readonly DependencyProperty IndexProperty = SplitterPanel.IndexPropertyKey.DependencyProperty;
                public static readonly DependencyProperty IsFirstProperty = SplitterPanel.IsFirstPropertyKey.DependencyProperty;
                public static readonly DependencyProperty IsLastProperty = SplitterPanel.IsLastPropertyKey.DependencyProperty;
                private SplitterResizePreviewWindow currentPreviewWindow;

                static SplitterPanel()
                {
                        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(SplitterPanel)));
                }

                public SplitterPanel()
                {
                        this.AddHandler(Thumb.DragStartedEvent, (Delegate)new DragStartedEventHandler(this.OnSplitterDragStarted));
                        AutomationProperties.SetAutomationId((DependencyObject)this, nameof(SplitterPanel));
                }

                private bool IsShowingResizePreview
                {
                        get
                        {
                                return this.currentPreviewWindow != null;
                        }
                }

                public static double GetActualSplitterLength(UIElement element)
                {
                        return (double)element.GetValue(SplitterPanel.ActualSplitterLengthProperty);
                }

                protected static void SetActualSplitterLength(UIElement element, double value)
                {
                        element.SetValue(SplitterPanel.ActualSplitterLengthPropertyKey, (object)value);
                }

                public static int GetIndex(UIElement element)
                {
                        return (int)element.GetValue(SplitterPanel.IndexProperty);
                }

                public static bool GetIsFirst(UIElement element)
                {
                        return (bool)element.GetValue(SplitterPanel.IsFirstProperty);
                }

                protected static void SetIsFirst(UIElement element, bool value)
                {
                        element.SetValue(SplitterPanel.IsFirstPropertyKey, Boxes.Box(value));
                }

                public static bool GetIsLast(UIElement element)
                {
                        return (bool)element.GetValue(SplitterPanel.IsLastProperty);
                }

                protected static void SetIsLast(UIElement element, bool value)
                {
                        element.SetValue(SplitterPanel.IsLastPropertyKey, Boxes.Box(value));
                }

                protected static void SetIndex(UIElement element, int value)
                {
                        element.SetValue(SplitterPanel.IndexPropertyKey, (object)value);
                }

                public static SplitterLength GetSplitterLength(UIElement element)
                {
                        if (element == null)
                                throw new ArgumentNullException(nameof(element));
                        return (SplitterLength)element.GetValue(SplitterPanel.SplitterLengthProperty);
                }

                public static void SetSplitterLength(UIElement element, SplitterLength value)
                {
                        if (element == null)
                                throw new ArgumentNullException(nameof(element));
                        element.SetValue(SplitterPanel.SplitterLengthProperty, (object)value);
                }

                public static double GetMinimumLength(UIElement element)
                {
                        if (element == null)
                                throw new ArgumentNullException(nameof(element));
                        return (double)element.GetValue(SplitterPanel.MinimumLengthProperty);
                }

                public static void SetMinimumLength(UIElement element, double value)
                {
                        if (element == null)
                                throw new ArgumentNullException(nameof(element));
                        element.SetValue(SplitterPanel.MinimumLengthProperty, (object)value);
                }

                public static double GetMaximumLength(UIElement element)
                {
                        if (element == null)
                                throw new ArgumentNullException(nameof(element));
                        return (double)element.GetValue(SplitterPanel.MaximumLengthProperty);
                }

                public static void SetMaximumLength(UIElement element, double value)
                {
                        if (element == null)
                                throw new ArgumentNullException(nameof(element));
                        element.SetValue(SplitterPanel.MaximumLengthProperty, (object)value);
                }

                public Orientation Orientation
                {
                        get
                        {
                                return (Orientation)this.GetValue(SplitterPanel.OrientationProperty);
                        }
                        set
                        {
                                this.SetValue(SplitterPanel.OrientationProperty, (object)value);
                        }
                }

                public bool ShowResizePreview
                {
                        get
                        {
                                return (bool)this.GetValue(SplitterPanel.ShowResizePreviewProperty);
                        }
                        set
                        {
                                this.SetValue(SplitterPanel.ShowResizePreviewProperty, Boxes.Box(value));
                        }
                }

                private void UpdateIndices()
                {
                        int count = this.InternalChildren.Count;
                        int num = this.InternalChildren.Count - 1;
                        for (int index = 0; index < count; ++index)
                        {
                                UIElement internalChild = this.InternalChildren[index];
                                if (internalChild != null)
                                {
                                        SplitterPanel.SetIndex(internalChild, index);
                                        SplitterPanel.SetIsFirst(internalChild, index == 0);
                                        SplitterPanel.SetIsLast(internalChild, index == num);
                                }
                        }
                }

                protected override Size MeasureOverride(Size availableSize)
                {
                        this.UpdateIndices();
                        return SplitterPanel.Measure(availableSize, this.Orientation, (IEnumerable<SplitterMeasureData>)SplitterMeasureData.FromElements((IList)this.InternalChildren), true);
                }

                private static Size MeasureNonreal(Size availableSize, Orientation orientation, IEnumerable<SplitterMeasureData> measureData, bool remeasureElements)
                {
                        double num1 = 0.0;
                        double num2 = 0.0;
                        foreach (SplitterMeasureData splitterMeasureData in measureData)
                        {
                                if (remeasureElements)
                                        splitterMeasureData.Element.Measure(availableSize);
                                if (orientation == Orientation.Horizontal)
                                {
                                        num1 += splitterMeasureData.Element.DesiredSize.Width;
                                        num2 = Math.Max(num2, splitterMeasureData.Element.DesiredSize.Height);
                                }
                                else
                                {
                                        num1 = Math.Max(num1, splitterMeasureData.Element.DesiredSize.Width);
                                        num2 += splitterMeasureData.Element.DesiredSize.Height;
                                }
                        }
                        Rect rect = new Rect(0.0, 0.0, num1, num2);
                        foreach (SplitterMeasureData splitterMeasureData in measureData)
                        {
                                if (orientation == Orientation.Horizontal)
                                {
                                        rect.Width = splitterMeasureData.Element.DesiredSize.Width;
                                        splitterMeasureData.MeasuredBounds = rect;
                                        rect.X += rect.Width;
                                }
                                else
                                {
                                        rect.Height = splitterMeasureData.Element.DesiredSize.Height;
                                        splitterMeasureData.MeasuredBounds = rect;
                                        rect.Y += rect.Height;
                                }
                        }
                        return new Size(num1, num2);
                }

                public static Size Measure(Size availableSize, Orientation orientation, IEnumerable<SplitterMeasureData> measureData, bool remeasureElements)
                {
                        double num1 = 0.0;
                        double num2 = 0.0;
                        double num3 = 0.0;
                        double num4 = 0.0;
                        if (orientation == Orientation.Horizontal && availableSize.Width.IsNonreal() || orientation == Orientation.Vertical && availableSize.Height.IsNonreal())
                                return SplitterPanel.MeasureNonreal(availableSize, orientation, measureData, remeasureElements);
                        foreach (SplitterMeasureData splitterMeasureData in measureData)
                        {
                                SplitterLength attachedLength = splitterMeasureData.AttachedLength;
                                double minimumLength = SplitterPanel.GetMinimumLength(splitterMeasureData.Element);
                                if (attachedLength.IsStretch)
                                {
                                        num1 += attachedLength.Value;
                                        num4 += minimumLength;
                                }
                                else
                                {
                                        num2 += attachedLength.Value;
                                        num3 += minimumLength;
                                }
                                splitterMeasureData.IsMinimumReached = false;
                                splitterMeasureData.IsMaximumReached = false;
                        }
                        double num5 = num4 + num3;
                        double width = availableSize.Width;
                        double height = availableSize.Height;
                        double num6 = orientation == Orientation.Horizontal ? width : height;
                        double num7 = num2 == 0.0 ? 0.0 : Math.Max(0.0, num6 - num1);
                        double num8 = num7 == 0.0 ? num6 : num1;
                        double num9 = num6;
                        if (num5 <= num9)
                        {
                                foreach (SplitterMeasureData splitterMeasureData in measureData)
                                {
                                        SplitterLength attachedLength = splitterMeasureData.AttachedLength;
                                        double maximumLength = SplitterPanel.GetMaximumLength(splitterMeasureData.Element);
                                        if (attachedLength.IsStretch && (num1 == 0.0 ? 0.0 : attachedLength.Value / num1 * num8) > maximumLength)
                                        {
                                                splitterMeasureData.IsMaximumReached = true;
                                                if (num1 == attachedLength.Value)
                                                {
                                                        num1 = maximumLength;
                                                        splitterMeasureData.AttachedLength = new SplitterLength(maximumLength);
                                                }
                                                else
                                                {
                                                        num1 -= attachedLength.Value;
                                                        splitterMeasureData.AttachedLength = new SplitterLength(num1);
                                                        double num10 = num1;
                                                        num1 = num10 + num10;
                                                }
                                                num7 = num2 == 0.0 ? 0.0 : Math.Max(0.0, num6 - num1);
                                                num8 = num7 == 0.0 ? num6 : num1;
                                        }
                                }
                                if (num7 < num3)
                                {
                                        num7 = num3;
                                        num8 = num6 - num7;
                                }
                                foreach (SplitterMeasureData splitterMeasureData in measureData)
                                {
                                        SplitterLength attachedLength = splitterMeasureData.AttachedLength;
                                        double minimumLength = SplitterPanel.GetMinimumLength(splitterMeasureData.Element);
                                        if (attachedLength.IsFill)
                                        {
                                                if ((num2 == 0.0 ? 0.0 : attachedLength.Value / num2 * num7) < minimumLength)
                                                {
                                                        splitterMeasureData.IsMinimumReached = true;
                                                        num7 -= minimumLength;
                                                        num2 -= attachedLength.Value;
                                                }
                                        }
                                        else if ((num1 == 0.0 ? 0.0 : attachedLength.Value / num1 * num8) < minimumLength)
                                        {
                                                splitterMeasureData.IsMinimumReached = true;
                                                num8 -= minimumLength;
                                                num1 -= attachedLength.Value;
                                        }
                                }
                        }
                        Size availableSize1 = new Size(width, height);
                        Rect rect = new Rect(0.0, 0.0, width, height);
                        foreach (SplitterMeasureData splitterMeasureData in measureData)
                        {
                                SplitterLength attachedLength = splitterMeasureData.AttachedLength;
                                double num10 = splitterMeasureData.IsMinimumReached ? SplitterPanel.GetMinimumLength(splitterMeasureData.Element) : (!attachedLength.IsFill ? (num1 == 0.0 ? 0.0 : attachedLength.Value / num1 * num8) : (num2 == 0.0 ? 0.0 : attachedLength.Value / num2 * num7));
                                if (remeasureElements)
                                        SplitterPanel.SetActualSplitterLength(splitterMeasureData.Element, num10);
                                if (orientation == Orientation.Horizontal)
                                {
                                        availableSize1.Width = num10;
                                        splitterMeasureData.MeasuredBounds = new Rect(rect.Left, rect.Top, num10, rect.Height);
                                        rect.X += num10;
                                        if (remeasureElements)
                                                splitterMeasureData.Element.Measure(availableSize1);
                                }
                                else
                                {
                                        availableSize1.Height = num10;
                                        splitterMeasureData.MeasuredBounds = new Rect(rect.Left, rect.Top, rect.Width, num10);
                                        rect.Y += num10;
                                        if (remeasureElements)
                                                splitterMeasureData.Element.Measure(availableSize1);
                                }
                        }
                        return new Size(width, height);
                }

                protected override Size ArrangeOverride(Size finalSize)
                {
                        Rect finalRect = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
                        foreach (UIElement internalChild in this.InternalChildren)
                        {
                                if (internalChild != null)
                                {
                                        double actualSplitterLength = SplitterPanel.GetActualSplitterLength(internalChild);
                                        if (this.Orientation == Orientation.Horizontal)
                                        {
                                                finalRect.Width = actualSplitterLength;
                                                internalChild.Arrange(finalRect);
                                                finalRect.X += actualSplitterLength;
                                        }
                                        else
                                        {
                                                finalRect.Height = actualSplitterLength;
                                                internalChild.Arrange(finalRect);
                                                finalRect.Y += actualSplitterLength;
                                        }
                                }
                        }
                        return finalSize;
                }

                private void OnSplitterDragStarted(object sender, DragStartedEventArgs args)
                {
                        SplitterGrip originalSource = args.OriginalSource as SplitterGrip;
                        if (originalSource == null)
                                return;
                        args.Handled = true;
                        originalSource.DragDelta += new DragDeltaEventHandler(this.OnSplitterResized);
                        originalSource.DragCompleted += new DragCompletedEventHandler(this.OnSplitterDragCompleted);
                        if (!this.ShowResizePreview)
                                return;
                        this.currentPreviewWindow = new SplitterResizePreviewWindow();
                        this.currentPreviewWindow.Show((UIElement)originalSource);
                }

                private void OnSplitterDragCompleted(object sender, DragCompletedEventArgs args)
                {
                        SplitterGrip grip = sender as SplitterGrip;
                        if (grip == null)
                                return;
                        args.Handled = true;
                        if (this.IsShowingResizePreview)
                        {
                                this.currentPreviewWindow.Hide();
                                this.currentPreviewWindow = (SplitterResizePreviewWindow)null;
                                if (!args.Canceled)
                                {
                                        Point logicalUnits = new Point(args.HorizontalChange, args.VerticalChange).DeviceToLogicalUnits();
                                        this.CommitResize(grip, logicalUnits.X, logicalUnits.Y);
                                }
                        }
                        grip.DragDelta -= new DragDeltaEventHandler(this.OnSplitterResized);
                        grip.DragCompleted -= new DragCompletedEventHandler(this.OnSplitterDragCompleted);
                }

                private void OnSplitterResized(object sender, DragDeltaEventArgs args)
                {
                        SplitterGrip grip = sender as SplitterGrip;
                        if (grip == null)
                                return;
                        args.Handled = true;
                        if (this.IsShowingResizePreview)
                                this.TrackResizePreview(grip, args.HorizontalChange, args.VerticalChange);
                        else
                                this.CommitResize(grip, args.HorizontalChange, args.VerticalChange);
                }

                private void CommitResize(SplitterGrip grip, double horizontalChange, double verticalChange)
                {
                        int gripIndex;
                        int resizeIndex1;
                        int resizeIndex2;
                        if (!this.GetResizeIndices(grip, out gripIndex, out resizeIndex1, out resizeIndex2))
                                return;
                        double pixelAmount = this.Orientation == Orientation.Horizontal ? horizontalChange : verticalChange;
                        this.ResizeChildren(resizeIndex1, resizeIndex2, pixelAmount);
                }

                private void TrackResizePreview(SplitterGrip grip, double horizontalChange, double verticalChange)
                {
                        int gripIndex;
                        int resizeIndex1;
                        int resizeIndex2;
                        if (!this.GetResizeIndices(grip, out gripIndex, out resizeIndex1, out resizeIndex2))
                                return;
                        double pixelAmount = this.Orientation == Orientation.Horizontal ? horizontalChange : verticalChange;
                        IList<SplitterMeasureData> splitterMeasureDataList = SplitterMeasureData.FromElements((IList)this.InternalChildren);
                        this.ResizeChildrenCore(splitterMeasureDataList[resizeIndex1], splitterMeasureDataList[resizeIndex2], pixelAmount);
                        SplitterPanel.Measure(this.RenderSize, this.Orientation, (IEnumerable<SplitterMeasureData>)splitterMeasureDataList, false);
                        Point point = grip.TransformToAncestor((Visual)this).Transform(new Point(0.0, 0.0));
                        if (this.Orientation == Orientation.Horizontal)
                                point.X += splitterMeasureDataList[gripIndex].MeasuredBounds.Width - this.InternalChildren[gripIndex].RenderSize.Width;
                        else
                                point.Y += splitterMeasureDataList[gripIndex].MeasuredBounds.Height - this.InternalChildren[gripIndex].RenderSize.Height;
                        Point screen = this.PointToScreen(point);
                        this.currentPreviewWindow.Move((double)(int)screen.X, (double)(int)screen.Y);
                }

                private bool GetResizeIndices(SplitterGrip grip, out int gripIndex, out int resizeIndex1, out int resizeIndex2)
                {
                        for (int index = 0; index < this.InternalChildren.Count; ++index)
                        {
                                if (this.InternalChildren[index].IsAncestorOf((DependencyObject)grip))
                                {
                                        gripIndex = index;
                                        switch (grip.ResizeBehavior)
                                        {
                                                case GridResizeBehavior.CurrentAndNext:
                                                        resizeIndex1 = index;
                                                        resizeIndex2 = index + 1;
                                                        break;
                                                case GridResizeBehavior.PreviousAndCurrent:
                                                        resizeIndex1 = index - 1;
                                                        resizeIndex2 = index;
                                                        break;
                                                case GridResizeBehavior.PreviousAndNext:
                                                        resizeIndex1 = index - 1;
                                                        resizeIndex2 = index + 1;
                                                        break;
                                                default:
                                                        throw new InvalidOperationException("BasedOnAlignment is not a valid resize behavior");
                                        }
                                        if (resizeIndex1 >= 0 && resizeIndex2 >= 0 && resizeIndex1 < this.InternalChildren.Count)
                                                return resizeIndex2 < this.InternalChildren.Count;
                                        return false;
                                }
                        }
                        gripIndex = -1;
                        resizeIndex1 = -1;
                        resizeIndex2 = -1;
                        return false;
                }

                internal void ResizeChildren(int index1, int index2, double pixelAmount)
                {
                        SplitterMeasureData child1 = new SplitterMeasureData(this.InternalChildren[index1]);
                        SplitterMeasureData child2 = new SplitterMeasureData(this.InternalChildren[index2]);
                        if (!this.ResizeChildrenCore(child1, child2, pixelAmount))
                                return;
                        SplitterPanel.SetSplitterLength(child1.Element, child1.AttachedLength);
                        SplitterPanel.SetSplitterLength(child2.Element, child2.AttachedLength);
                        this.InvalidateMeasure();
                }

                private bool ResizeChildrenCore(SplitterMeasureData child1, SplitterMeasureData child2, double pixelAmount)
                {
                        UIElement element1 = child1.Element;
                        UIElement element2 = child2.Element;
                        SplitterLength attachedLength1 = child1.AttachedLength;
                        SplitterLength attachedLength2 = child2.AttachedLength;
                        double actualSplitterLength1 = SplitterPanel.GetActualSplitterLength(element1);
                        double actualSplitterLength2 = SplitterPanel.GetActualSplitterLength(element2);
                        double num1 = Math.Max(0.0, Math.Min(actualSplitterLength1 + actualSplitterLength2, actualSplitterLength1 + pixelAmount));
                        double num2 = Math.Max(0.0, Math.Min(actualSplitterLength1 + actualSplitterLength2, actualSplitterLength2 - pixelAmount));
                        double minimumLength1 = SplitterPanel.GetMinimumLength(element1);
                        double minimumLength2 = SplitterPanel.GetMinimumLength(element2);
                        if (minimumLength1 + minimumLength2 > num1 + num2)
                                return false;
                        if (num1 < minimumLength1)
                        {
                                num2 -= minimumLength1 - num1;
                                num1 = minimumLength1;
                        }
                        if (num2 < minimumLength2)
                        {
                                num1 -= minimumLength2 - num2;
                                num2 = minimumLength2;
                        }
                        if (attachedLength1.IsFill && attachedLength2.IsFill || attachedLength1.IsStretch && attachedLength2.IsStretch)
                        {
                                SplitterMeasureData splitterMeasureData = child1;
                                double num3 = num1;
                                SplitterLength splitterLength = new SplitterLength(num3 / (num3 + num2) * (attachedLength1.Value + attachedLength2.Value), attachedLength1.SplitterUnitType);
                                splitterMeasureData.AttachedLength = splitterLength;
                                child2.AttachedLength = new SplitterLength(num2 / (num1 + num2) * (attachedLength1.Value + attachedLength2.Value), attachedLength1.SplitterUnitType);
                        }
                        else if (attachedLength1.IsFill)
                                child2.AttachedLength = new SplitterLength(num2, SplitterUnitType.Stretch);
                        else
                                child1.AttachedLength = new SplitterLength(num1, SplitterUnitType.Stretch);
                        return true;
                }
        }
}
