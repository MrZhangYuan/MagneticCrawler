using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace UltimatePresentation.Presentation
{
        internal static class MultiValueHelper
        {
                public static void CheckValue<T>(object[] values, int index)
                {
                        if (!(values[index] is T) && (values[index] != null || typeof(T).IsValueType))
                        {
                                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "", new object[]
                                {
                                        index,
                                        typeof(T).FullName
                                }));
                        }
                }
                public static void CheckType<T>(Type[] types, int index)
                {
                        if (!types[index].IsAssignableFrom(typeof(T)))
                        {
                                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "", new object[]
                                {
                                        index,
                                        typeof(T).FullName
                                }));
                        }
                }
        }
        public class MultiValueConverter<TSource1, TSource2, TTarget> : IMultiValueConverter
        {
                public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
                {
                        if (values.Length != 2)
                        {
                                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "", new object[]
                                {
                                        2
                                }));
                        }
                        for (int i = 0; i < values.Length; i++)
                        {
                                object obj = values[i];
                                //if (obj == DependencyProperty.UnsetValue || obj == BindingOperations.DisconnectedSource)
                                if (obj == DependencyProperty.UnsetValue)
                                {
                                        return default(TTarget);
                                }
                        }
                        MultiValueHelper.CheckValue<TSource1>(values, 0);
                        MultiValueHelper.CheckValue<TSource2>(values, 1);
                        if (!targetType.IsAssignableFrom(typeof(TTarget)))
                        {
                                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "", new object[]
                                {
                                        typeof(TTarget).FullName
                                }));
                        }
                        return this.Convert((TSource1)((object)values[0]), (TSource2)((object)values[1]), parameter, culture);
                }
                public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
                {
                        if (targetTypes.Length != 2)
                        {
                                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "", new object[]
                                {
                                        2
                                }));
                        }
                        if (!(value is TTarget) && (value != null || typeof(TTarget).IsValueType))
                        {
                                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "", new object[]
                                {
                                        typeof(TTarget).FullName
                                }));
                        }
                        MultiValueHelper.CheckType<TSource1>(targetTypes, 0);
                        MultiValueHelper.CheckType<TSource2>(targetTypes, 1);
                        TSource1 tSource;
                        TSource2 tSource2;
                        this.ConvertBack((TTarget)((object)value), out tSource, out tSource2, parameter, culture);
                        return new object[]
                        {
                                tSource,
                                tSource2
                        };
                }
                protected virtual TTarget Convert(TSource1 value1, TSource2 value2, object parameter, CultureInfo culture)
                {
                        throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "", new object[]
                        {
                                "Convert"
                        }));
                }
                protected virtual void ConvertBack(TTarget value, out TSource1 out1, out TSource2 out2, object parameter, CultureInfo culture)
                {
                        throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "", new object[]
                        {
                                "ConvertBack"
                        }));
                }
        }
        public class BooleanOrConverter : MultiValueConverter<bool, bool, bool>
        {
                protected override bool Convert(bool value1, bool value2, object parameter, CultureInfo culture)
                {
                        return value1 || value2;
                }
        }
        public static class Boxes
        {
                public static readonly object BooleanTrue = true;
                public static readonly object BooleanFalse = false;
                public static object Box(bool value)
                {
                        if (!value)
                        {
                                return Boxes.BooleanFalse;
                        }
                        return Boxes.BooleanTrue;
                }
                public static object Box(bool? nullableValue)
                {
                        if (!nullableValue.HasValue)
                        {
                                return null;
                        }
                        return Boxes.Box(nullableValue.Value);
                }
        }
        public class GlyphButton : Button, INonClientArea
        {
                public static readonly DependencyProperty PressedBackgroundProperty;
                public static readonly DependencyProperty PressedBorderBrushProperty;
                public static readonly DependencyProperty PressedBorderThicknessProperty;
                public static readonly DependencyProperty HoverBackgroundProperty;
                public static readonly DependencyProperty HoverBorderBrushProperty;
                public static readonly DependencyProperty HoverBorderThicknessProperty;
                public static readonly DependencyProperty GlyphForegroundProperty;
                public static readonly DependencyProperty HoverForegroundProperty;
                public static readonly DependencyProperty PressedForegroundProperty;
                public static readonly DependencyProperty IsCheckedProperty;
                public Brush PressedBackground
                {
                        get
                        {
                                return (Brush)base.GetValue(GlyphButton.PressedBackgroundProperty);
                        }
                        set
                        {
                                base.SetValue(GlyphButton.PressedBackgroundProperty, value);
                        }
                }
                public Brush PressedBorderBrush
                {
                        get
                        {
                                return (Brush)base.GetValue(GlyphButton.PressedBorderBrushProperty);
                        }
                        set
                        {
                                base.SetValue(GlyphButton.PressedBorderBrushProperty, value);
                        }
                }
                public Thickness PressedBorderThickness
                {
                        get
                        {
                                return (Thickness)base.GetValue(GlyphButton.PressedBorderThicknessProperty);
                        }
                        set
                        {
                                base.SetValue(GlyphButton.PressedBorderThicknessProperty, value);
                        }
                }
                public Brush HoverBackground
                {
                        get
                        {
                                return (Brush)base.GetValue(GlyphButton.HoverBackgroundProperty);
                        }
                        set
                        {
                                base.SetValue(GlyphButton.HoverBackgroundProperty, value);
                        }
                }
                public Brush HoverBorderBrush
                {
                        get
                        {
                                return (Brush)base.GetValue(GlyphButton.HoverBorderBrushProperty);
                        }
                        set
                        {
                                base.SetValue(GlyphButton.HoverBorderBrushProperty, value);
                        }
                }
                public Thickness HoverBorderThickness
                {
                        get
                        {
                                return (Thickness)base.GetValue(GlyphButton.HoverBorderThicknessProperty);
                        }
                        set
                        {
                                base.SetValue(GlyphButton.HoverBorderThicknessProperty, value);
                        }
                }
                public Brush GlyphForeground
                {
                        get
                        {
                                return (Brush)base.GetValue(GlyphButton.GlyphForegroundProperty);
                        }
                        set
                        {
                                base.SetValue(GlyphButton.GlyphForegroundProperty, value);
                        }
                }
                public Brush HoverForeground
                {
                        get
                        {
                                return (Brush)base.GetValue(GlyphButton.HoverForegroundProperty);
                        }
                        set
                        {
                                base.SetValue(GlyphButton.HoverForegroundProperty, value);
                        }
                }
                public Brush PressedForeground
                {
                        get
                        {
                                return (Brush)base.GetValue(GlyphButton.PressedForegroundProperty);
                        }
                        set
                        {
                                base.SetValue(GlyphButton.PressedForegroundProperty, value);
                        }
                }
                public bool IsChecked
                {
                        get
                        {
                                return (bool)base.GetValue(GlyphButton.IsCheckedProperty);
                        }
                        set
                        {
                                base.SetValue(GlyphButton.IsCheckedProperty, Boxes.Box(value));
                        }
                }


                public CornerRadius CornerRadius
                {
                        get
                        {
                                return (CornerRadius)GetValue(CornerRadiusProperty);
                        }
                        set
                        {
                                SetValue(CornerRadiusProperty, value);
                        }
                }
                public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(GlyphButton), new PropertyMetadata(new CornerRadius(0)));

                static GlyphButton()
                {
                        GlyphButton.PressedBackgroundProperty = DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(GlyphButton));
                        GlyphButton.PressedBorderBrushProperty = DependencyProperty.Register("PressedBorderBrush", typeof(Brush), typeof(GlyphButton));
                        GlyphButton.PressedBorderThicknessProperty = DependencyProperty.Register("PressedBorderThickness", typeof(Thickness), typeof(GlyphButton));
                        GlyphButton.HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(GlyphButton));
                        GlyphButton.HoverBorderBrushProperty = DependencyProperty.Register("HoverBorderBrush", typeof(Brush), typeof(GlyphButton));
                        GlyphButton.HoverBorderThicknessProperty = DependencyProperty.Register("HoverBorderThickness", typeof(Thickness), typeof(GlyphButton));
                        GlyphButton.GlyphForegroundProperty = DependencyProperty.Register("GlyphForeground", typeof(Brush), typeof(GlyphButton));
                        GlyphButton.HoverForegroundProperty = DependencyProperty.Register("HoverForeground", typeof(Brush), typeof(GlyphButton));
                        GlyphButton.PressedForegroundProperty = DependencyProperty.Register("PressedForeground", typeof(Brush), typeof(GlyphButton));
                        GlyphButton.IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(GlyphButton));
                        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(GlyphButton), new FrameworkPropertyMetadata(typeof(GlyphButton)));
                }
                int INonClientArea.HitTest(Point point)
                {
                        return 1;
                }
        }
}
