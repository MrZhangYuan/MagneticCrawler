using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace UltimatePresentation.Behaviours
{
        public class ExtendedVisualStateManager : VisualStateManager
        {
                internal class WrapperCanvas : Canvas
                {
                        internal static readonly DependencyProperty SimulationProgressProperty = DependencyProperty.Register("SimulationProgress", typeof(double), typeof(ExtendedVisualStateManager.WrapperCanvas), new PropertyMetadata(0.0, new PropertyChangedCallback(ExtendedVisualStateManager.WrapperCanvas.SimulationProgressChanged)));
                        public Rect OldRect
                        {
                                get;
                                set;
                        }
                        public Rect NewRect
                        {
                                get;
                                set;
                        }
                        public Dictionary<DependencyProperty, object> LocalValueCache
                        {
                                get;
                                set;
                        }
                        public Visibility DestinationVisibilityCache
                        {
                                get;
                                set;
                        }
                        public double SimulationProgress
                        {
                                get
                                {
                                        return (double)base.GetValue(ExtendedVisualStateManager.WrapperCanvas.SimulationProgressProperty);
                                }
                                set
                                {
                                        base.SetValue(ExtendedVisualStateManager.WrapperCanvas.SimulationProgressProperty, value);
                                }
                        }
                        private static void SimulationProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                        {
                                ExtendedVisualStateManager.WrapperCanvas wrapperCanvas = d as ExtendedVisualStateManager.WrapperCanvas;
                                double num = (double)e.NewValue;
                                if (wrapperCanvas != null && wrapperCanvas.Children.Count > 0)
                                {
                                        FrameworkElement frameworkElement = wrapperCanvas.Children[0] as FrameworkElement;
                                        frameworkElement.Width = Math.Max(0.0, wrapperCanvas.OldRect.Width * num + wrapperCanvas.NewRect.Width * (1.0 - num));
                                        frameworkElement.Height = Math.Max(0.0, wrapperCanvas.OldRect.Height * num + wrapperCanvas.NewRect.Height * (1.0 - num));
                                        Canvas.SetLeft(frameworkElement, num * (wrapperCanvas.OldRect.Left - wrapperCanvas.NewRect.Left));
                                        Canvas.SetTop(frameworkElement, num * (wrapperCanvas.OldRect.Top - wrapperCanvas.NewRect.Top));
                                }
                        }
                }
                internal class OriginalLayoutValueRecord
                {
                        public FrameworkElement Element
                        {
                                get;
                                set;
                        }
                        public DependencyProperty Property
                        {
                                get;
                                set;
                        }
                        public object Value
                        {
                                get;
                                set;
                        }
                }
                private class DummyEasingFunction : EasingFunctionBase
                {
                        public static readonly DependencyProperty DummyValueProperty = DependencyProperty.Register("DummyValue", typeof(double), typeof(ExtendedVisualStateManager.DummyEasingFunction), new PropertyMetadata(0.0));
                        public double DummyValue
                        {
                                get
                                {
                                        return (double)base.GetValue(ExtendedVisualStateManager.DummyEasingFunction.DummyValueProperty);
                                }
                                set
                                {
                                        base.SetValue(ExtendedVisualStateManager.DummyEasingFunction.DummyValueProperty, value);
                                }
                        }
                        protected override Freezable CreateInstanceCore()
                        {
                                return new ExtendedVisualStateManager.DummyEasingFunction();
                        }
                        protected override double EaseInCore(double normalizedTime)
                        {
                                return this.DummyValue;
                        }
                }
                public static readonly DependencyProperty UseFluidLayoutProperty = DependencyProperty.RegisterAttached("UseFluidLayout", typeof(bool), typeof(ExtendedVisualStateManager), new PropertyMetadata(false));
                public static readonly DependencyProperty RuntimeVisibilityPropertyProperty = DependencyProperty.RegisterAttached("RuntimeVisibilityProperty", typeof(DependencyProperty), typeof(ExtendedVisualStateManager), new PropertyMetadata(null));
                internal static readonly DependencyProperty OriginalLayoutValuesProperty = DependencyProperty.RegisterAttached("OriginalLayoutValues", typeof(List<ExtendedVisualStateManager.OriginalLayoutValueRecord>), typeof(ExtendedVisualStateManager), new PropertyMetadata(null));
                internal static readonly DependencyProperty LayoutStoryboardProperty = DependencyProperty.RegisterAttached("LayoutStoryboard", typeof(Storyboard), typeof(ExtendedVisualStateManager), new PropertyMetadata(null));
                internal static readonly DependencyProperty CurrentStateProperty = DependencyProperty.RegisterAttached("CurrentState", typeof(VisualState), typeof(ExtendedVisualStateManager), new PropertyMetadata(null));
                public static readonly DependencyProperty TransitionEffectProperty = DependencyProperty.RegisterAttached("TransitionEffect", typeof(TransitionEffect), typeof(ExtendedVisualStateManager), new PropertyMetadata(null));
                internal static readonly DependencyProperty TransitionEffectStoryboardProperty = DependencyProperty.RegisterAttached("TransitionEffectStoryboard", typeof(Storyboard), typeof(ExtendedVisualStateManager), new PropertyMetadata(null));
                internal static readonly DependencyProperty DidCacheBackgroundProperty = DependencyProperty.RegisterAttached("DidCacheBackground", typeof(bool), typeof(ExtendedVisualStateManager), new PropertyMetadata(false));
                internal static readonly DependencyProperty CachedBackgroundProperty = DependencyProperty.RegisterAttached("CachedBackground", typeof(object), typeof(ExtendedVisualStateManager), new PropertyMetadata(null));
                internal static readonly DependencyProperty CachedEffectProperty = DependencyProperty.RegisterAttached("CachedEffect", typeof(Effect), typeof(ExtendedVisualStateManager), new PropertyMetadata(null));
                private static List<FrameworkElement> MovingElements;
                private static Storyboard LayoutTransitionStoryboard;
                private static List<DependencyProperty> LayoutProperties = new List<DependencyProperty>
		{
			Grid.ColumnProperty,
			Grid.ColumnSpanProperty,
			Grid.RowProperty,
			Grid.RowSpanProperty,
			Canvas.LeftProperty,
			Canvas.TopProperty,
			FrameworkElement.WidthProperty,
			FrameworkElement.HeightProperty,
			FrameworkElement.MinWidthProperty,
			FrameworkElement.MinHeightProperty,
			FrameworkElement.MaxWidthProperty,
			FrameworkElement.MaxHeightProperty,
			FrameworkElement.MarginProperty,
			FrameworkElement.HorizontalAlignmentProperty,
			FrameworkElement.VerticalAlignmentProperty,
			UIElement.VisibilityProperty,
			StackPanel.OrientationProperty
		};
                private static List<DependencyProperty> ChildAffectingLayoutProperties = new List<DependencyProperty>
		{
			StackPanel.OrientationProperty
		};
                private bool changingState;
                public static bool IsRunningFluidLayoutTransition
                {
                        get
                        {
                                return ExtendedVisualStateManager.LayoutTransitionStoryboard != null;
                        }
                }
                public static bool GetUseFluidLayout(DependencyObject obj)
                {
                        return (bool)obj.GetValue(ExtendedVisualStateManager.UseFluidLayoutProperty);
                }
                public static void SetUseFluidLayout(DependencyObject obj, bool value)
                {
                        obj.SetValue(ExtendedVisualStateManager.UseFluidLayoutProperty, value);
                }
                public static DependencyProperty GetRuntimeVisibilityProperty(DependencyObject obj)
                {
                        return (DependencyProperty)obj.GetValue(ExtendedVisualStateManager.RuntimeVisibilityPropertyProperty);
                }
                public static void SetRuntimeVisibilityProperty(DependencyObject obj, DependencyProperty value)
                {
                        obj.SetValue(ExtendedVisualStateManager.RuntimeVisibilityPropertyProperty, value);
                }
                internal static List<ExtendedVisualStateManager.OriginalLayoutValueRecord> GetOriginalLayoutValues(DependencyObject obj)
                {
                        return (List<ExtendedVisualStateManager.OriginalLayoutValueRecord>)obj.GetValue(ExtendedVisualStateManager.OriginalLayoutValuesProperty);
                }
                internal static void SetOriginalLayoutValues(DependencyObject obj, List<ExtendedVisualStateManager.OriginalLayoutValueRecord> value)
                {
                        obj.SetValue(ExtendedVisualStateManager.OriginalLayoutValuesProperty, value);
                }
                internal static Storyboard GetLayoutStoryboard(DependencyObject obj)
                {
                        return (Storyboard)obj.GetValue(ExtendedVisualStateManager.LayoutStoryboardProperty);
                }
                internal static void SetLayoutStoryboard(DependencyObject obj, Storyboard value)
                {
                        obj.SetValue(ExtendedVisualStateManager.LayoutStoryboardProperty, value);
                }
                internal static VisualState GetCurrentState(DependencyObject obj)
                {
                        return (VisualState)obj.GetValue(ExtendedVisualStateManager.CurrentStateProperty);
                }
                internal static void SetCurrentState(DependencyObject obj, VisualState value)
                {
                        obj.SetValue(ExtendedVisualStateManager.CurrentStateProperty, value);
                }
                public static TransitionEffect GetTransitionEffect(DependencyObject obj)
                {
                        return (TransitionEffect)obj.GetValue(ExtendedVisualStateManager.TransitionEffectProperty);
                }
                public static void SetTransitionEffect(DependencyObject obj, TransitionEffect value)
                {
                        obj.SetValue(ExtendedVisualStateManager.TransitionEffectProperty, value);
                }
                internal static Storyboard GetTransitionEffectStoryboard(DependencyObject obj)
                {
                        return (Storyboard)obj.GetValue(ExtendedVisualStateManager.TransitionEffectStoryboardProperty);
                }
                internal static void SetTransitionEffectStoryboard(DependencyObject obj, Storyboard value)
                {
                        obj.SetValue(ExtendedVisualStateManager.TransitionEffectStoryboardProperty, value);
                }
                internal static bool GetDidCacheBackground(DependencyObject obj)
                {
                        return (bool)obj.GetValue(ExtendedVisualStateManager.DidCacheBackgroundProperty);
                }
                internal static void SetDidCacheBackground(DependencyObject obj, bool value)
                {
                        obj.SetValue(ExtendedVisualStateManager.DidCacheBackgroundProperty, value);
                }
                internal static object GetCachedBackground(DependencyObject obj)
                {
                        return obj.GetValue(ExtendedVisualStateManager.CachedBackgroundProperty);
                }
                internal static void SetCachedBackground(DependencyObject obj, object value)
                {
                        obj.SetValue(ExtendedVisualStateManager.CachedBackgroundProperty, value);
                }
                internal static Effect GetCachedEffect(DependencyObject obj)
                {
                        return (Effect)obj.GetValue(ExtendedVisualStateManager.CachedEffectProperty);
                }
                internal static void SetCachedEffect(DependencyObject obj, Effect value)
                {
                        obj.SetValue(ExtendedVisualStateManager.CachedEffectProperty, value);
                }
                private static bool IsVisibilityProperty(DependencyProperty property)
                {
                        return property == UIElement.VisibilityProperty || property.Name == "RuntimeVisibility";
                }
                private static DependencyProperty LayoutPropertyFromTimeline(Timeline timeline, bool forceRuntimeProperty)
                {
                        PropertyPath targetProperty = Storyboard.GetTargetProperty(timeline);
                        if (targetProperty == null || targetProperty.PathParameters == null || targetProperty.PathParameters.Count == 0)
                        {
                                return null;
                        }
                        DependencyProperty dependencyProperty = targetProperty.PathParameters[0] as DependencyProperty;
                        if (dependencyProperty != null)
                        {
                                if (dependencyProperty.Name == "RuntimeVisibility" && dependencyProperty.OwnerType.Name.EndsWith("DesignTimeProperties", StringComparison.Ordinal))
                                {
                                        if (!ExtendedVisualStateManager.LayoutProperties.Contains(dependencyProperty))
                                        {
                                                ExtendedVisualStateManager.LayoutProperties.Add(dependencyProperty);
                                        }
                                        if (!forceRuntimeProperty)
                                        {
                                                return UIElement.VisibilityProperty;
                                        }
                                        return dependencyProperty;
                                }
                                else
                                {
                                        if (dependencyProperty.Name == "RuntimeWidth" && dependencyProperty.OwnerType.Name.EndsWith("DesignTimeProperties", StringComparison.Ordinal))
                                        {
                                                if (!ExtendedVisualStateManager.LayoutProperties.Contains(dependencyProperty))
                                                {
                                                        ExtendedVisualStateManager.LayoutProperties.Add(dependencyProperty);
                                                }
                                                if (!forceRuntimeProperty)
                                                {
                                                        return FrameworkElement.WidthProperty;
                                                }
                                                return dependencyProperty;
                                        }
                                        else
                                        {
                                                if (dependencyProperty.Name == "RuntimeHeight" && dependencyProperty.OwnerType.Name.EndsWith("DesignTimeProperties", StringComparison.Ordinal))
                                                {
                                                        if (!ExtendedVisualStateManager.LayoutProperties.Contains(dependencyProperty))
                                                        {
                                                                ExtendedVisualStateManager.LayoutProperties.Add(dependencyProperty);
                                                        }
                                                        if (!forceRuntimeProperty)
                                                        {
                                                                return FrameworkElement.HeightProperty;
                                                        }
                                                        return dependencyProperty;
                                                }
                                                else
                                                {
                                                        if (ExtendedVisualStateManager.LayoutProperties.Contains(dependencyProperty))
                                                        {
                                                                return dependencyProperty;
                                                        }
                                                }
                                        }
                                }
                        }
                        return null;
                }
                protected override bool GoToStateCore(FrameworkElement control, FrameworkElement stateGroupsRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
                {
                        if (this.changingState)
                        {
                                return false;
                        }
                        if (group == null || state == null)
                        {
                                return false;
                        }
                        VisualState currentState = ExtendedVisualStateManager.GetCurrentState(group);
                        if (currentState == state)
                        {
                                return true;
                        }
                        VisualTransition transition = ExtendedVisualStateManager.FindTransition(group, currentState, state);
                        bool animateWithTransitionEffect = ExtendedVisualStateManager.PrepareTransitionEffectImage(stateGroupsRoot, useTransitions, transition);
                        if (!ExtendedVisualStateManager.GetUseFluidLayout(group))
                        {
                                return this.TransitionEffectAwareGoToStateCore(control, stateGroupsRoot, stateName, group, state, useTransitions, transition, animateWithTransitionEffect, currentState);
                        }
                        Storyboard storyboard = ExtendedVisualStateManager.ExtractLayoutStoryboard(state);
                        List<ExtendedVisualStateManager.OriginalLayoutValueRecord> list = ExtendedVisualStateManager.GetOriginalLayoutValues(group);
                        if (list == null)
                        {
                                list = new List<ExtendedVisualStateManager.OriginalLayoutValueRecord>();
                                ExtendedVisualStateManager.SetOriginalLayoutValues(group, list);
                        }
                        if (!useTransitions)
                        {
                                if (ExtendedVisualStateManager.LayoutTransitionStoryboard != null)
                                {
                                        ExtendedVisualStateManager.StopAnimations();
                                }
                                bool result = this.TransitionEffectAwareGoToStateCore(control, stateGroupsRoot, stateName, group, state, useTransitions, transition, animateWithTransitionEffect, currentState);
                                ExtendedVisualStateManager.SetLayoutStoryboardProperties(control, stateGroupsRoot, storyboard, list);
                                return result;
                        }
                        if (storyboard.Children.Count == 0 && list.Count == 0)
                        {
                                return this.TransitionEffectAwareGoToStateCore(control, stateGroupsRoot, stateName, group, state, useTransitions, transition, animateWithTransitionEffect, currentState);
                        }
                        try
                        {
                                this.changingState = true;
                                stateGroupsRoot.UpdateLayout();
                                List<FrameworkElement> list2 = ExtendedVisualStateManager.FindTargetElements(control, stateGroupsRoot, storyboard, list, ExtendedVisualStateManager.MovingElements);
                                Dictionary<FrameworkElement, Rect> rectsOfTargets = ExtendedVisualStateManager.GetRectsOfTargets(list2, ExtendedVisualStateManager.MovingElements);
                                Dictionary<FrameworkElement, double> oldOpacities = ExtendedVisualStateManager.GetOldOpacities(control, stateGroupsRoot, storyboard, list, ExtendedVisualStateManager.MovingElements);
                                if (ExtendedVisualStateManager.LayoutTransitionStoryboard != null)
                                {
                                        stateGroupsRoot.LayoutUpdated -= new EventHandler(ExtendedVisualStateManager.control_LayoutUpdated);
                                        ExtendedVisualStateManager.StopAnimations();
                                        stateGroupsRoot.UpdateLayout();
                                }
                                this.TransitionEffectAwareGoToStateCore(control, stateGroupsRoot, stateName, group, state, useTransitions, transition, animateWithTransitionEffect, currentState);
                                ExtendedVisualStateManager.SetLayoutStoryboardProperties(control, stateGroupsRoot, storyboard, list);
                                stateGroupsRoot.UpdateLayout();
                                Dictionary<FrameworkElement, Rect> rectsOfTargets2 = ExtendedVisualStateManager.GetRectsOfTargets(list2, null);
                                ExtendedVisualStateManager.MovingElements = new List<FrameworkElement>();
                                foreach (FrameworkElement current in list2)
                                {
                                        if (rectsOfTargets[current] != rectsOfTargets2[current])
                                        {
                                                ExtendedVisualStateManager.MovingElements.Add(current);
                                        }
                                }
                                foreach (FrameworkElement current2 in oldOpacities.Keys)
                                {
                                        if (!ExtendedVisualStateManager.MovingElements.Contains(current2))
                                        {
                                                ExtendedVisualStateManager.MovingElements.Add(current2);
                                        }
                                }
                                ExtendedVisualStateManager.WrapMovingElementsInCanvases(ExtendedVisualStateManager.MovingElements, rectsOfTargets, rectsOfTargets2);
                                stateGroupsRoot.LayoutUpdated += new EventHandler(ExtendedVisualStateManager.control_LayoutUpdated);
                                ExtendedVisualStateManager.LayoutTransitionStoryboard = ExtendedVisualStateManager.CreateLayoutTransitionStoryboard(transition, ExtendedVisualStateManager.MovingElements, oldOpacities);
                                ExtendedVisualStateManager.LayoutTransitionStoryboard.Completed += delegate(object sender, EventArgs args)
                                {
                                        stateGroupsRoot.LayoutUpdated -= new EventHandler(ExtendedVisualStateManager.control_LayoutUpdated);
                                        ExtendedVisualStateManager.StopAnimations();
                                };
                                ExtendedVisualStateManager.LayoutTransitionStoryboard.Begin();
                        }
                        finally
                        {
                                this.changingState = false;
                        }
                        return true;
                }
                private static void control_LayoutUpdated(object sender, EventArgs e)
                {
                        if (ExtendedVisualStateManager.LayoutTransitionStoryboard != null)
                        {
                                foreach (FrameworkElement current in ExtendedVisualStateManager.MovingElements)
                                {
                                        ExtendedVisualStateManager.WrapperCanvas wrapperCanvas = current.Parent as ExtendedVisualStateManager.WrapperCanvas;
                                        if (wrapperCanvas != null)
                                        {
                                                Rect layoutRect = ExtendedVisualStateManager.GetLayoutRect(wrapperCanvas);
                                                Rect newRect = wrapperCanvas.NewRect;
                                                TranslateTransform translateTransform = wrapperCanvas.RenderTransform as TranslateTransform;
                                                double num = (translateTransform == null) ? 0.0 : translateTransform.X;
                                                double num2 = (translateTransform == null) ? 0.0 : translateTransform.Y;
                                                double num3 = newRect.Left - layoutRect.Left;
                                                double num4 = newRect.Top - layoutRect.Top;
                                                if (num != num3 || num2 != num4)
                                                {
                                                        if (translateTransform == null)
                                                        {
                                                                translateTransform = new TranslateTransform();
                                                                wrapperCanvas.RenderTransform = translateTransform;
                                                        }
                                                        translateTransform.X = num3;
                                                        translateTransform.Y = num4;
                                                }
                                        }
                                }
                        }
                }
                private static void StopAnimations()
                {
                        if (ExtendedVisualStateManager.LayoutTransitionStoryboard != null)
                        {
                                ExtendedVisualStateManager.LayoutTransitionStoryboard.Stop();
                                ExtendedVisualStateManager.LayoutTransitionStoryboard = null;
                        }
                        if (ExtendedVisualStateManager.MovingElements != null)
                        {
                                ExtendedVisualStateManager.UnwrapMovingElementsFromCanvases(ExtendedVisualStateManager.MovingElements);
                                ExtendedVisualStateManager.MovingElements = null;
                        }
                }
                private static bool PrepareTransitionEffectImage(FrameworkElement stateGroupsRoot, bool useTransitions, VisualTransition transition)
                {
                        TransitionEffect transitionEffect = (transition == null) ? null : ExtendedVisualStateManager.GetTransitionEffect(transition);
                        bool result = false;
                        if (transitionEffect != null)
                        {
                                transitionEffect = transitionEffect.CloneCurrentValue();
                                if (useTransitions)
                                {
                                        result = true;
                                        int pixelWidth = (int)Math.Max(1.0, stateGroupsRoot.ActualWidth);
                                        int pixelHeight = (int)Math.Max(1.0, stateGroupsRoot.ActualHeight);
                                        RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(pixelWidth, pixelHeight, 96.0, 96.0, PixelFormats.Pbgra32);
                                        renderTargetBitmap.Render(stateGroupsRoot);
                                        transitionEffect.OldImage = new ImageBrush
                                        {
                                                ImageSource = renderTargetBitmap
                                        };
                                }
                                Storyboard transitionEffectStoryboard = ExtendedVisualStateManager.GetTransitionEffectStoryboard(stateGroupsRoot);
                                if (transitionEffectStoryboard != null)
                                {
                                        transitionEffectStoryboard.Stop();
                                        ExtendedVisualStateManager.FinishTransitionEffectAnimation(stateGroupsRoot);
                                }
                                if (useTransitions)
                                {
                                        ExtendedVisualStateManager.TransferLocalValue(stateGroupsRoot, UIElement.EffectProperty, ExtendedVisualStateManager.CachedEffectProperty);
                                        stateGroupsRoot.Effect = transitionEffect;
                                }
                        }
                        return result;
                }
                private bool TransitionEffectAwareGoToStateCore(FrameworkElement control, FrameworkElement stateGroupsRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions, VisualTransition transition, bool animateWithTransitionEffect, VisualState previousState)
                {
                        IEasingFunction generatedEasingFunction = null;
                        if (animateWithTransitionEffect)
                        {
                                generatedEasingFunction = transition.GeneratedEasingFunction;
                                transition.GeneratedEasingFunction = new ExtendedVisualStateManager.DummyEasingFunction
                                {
                                        DummyValue = ExtendedVisualStateManager.FinishesWithZeroOpacity(control, stateGroupsRoot, state, previousState) ? 0.01 : 0.0
                                };
                        }
                        bool flag = base.GoToStateCore(control, stateGroupsRoot, stateName, group, state, useTransitions);
                        if (animateWithTransitionEffect)
                        {
                                transition.GeneratedEasingFunction = generatedEasingFunction;
                                if (flag)
                                {
                                        ExtendedVisualStateManager.AnimateTransitionEffect(stateGroupsRoot, transition);
                                }
                        }
                        ExtendedVisualStateManager.SetCurrentState(group, state);
                        return flag;
                }
                private static bool FinishesWithZeroOpacity(FrameworkElement control, FrameworkElement stateGroupsRoot, VisualState state, VisualState previousState)
                {
                        if (state.Storyboard != null)
                        {
                                foreach (Timeline current in state.Storyboard.Children)
                                {
                                        if (ExtendedVisualStateManager.TimelineIsAnimatingRootOpacity(current, control, stateGroupsRoot))
                                        {
                                                bool flag;
                                                object valueFromTimeline = ExtendedVisualStateManager.GetValueFromTimeline(current, out flag);
                                                return flag && valueFromTimeline is double && (double)valueFromTimeline == 0.0;
                                        }
                                }
                        }
                        if (previousState != null && previousState.Storyboard != null)
                        {
                                foreach (Timeline current2 in previousState.Storyboard.Children)
                                {
                                        ExtendedVisualStateManager.TimelineIsAnimatingRootOpacity(current2, control, stateGroupsRoot);
                                }
                                double num = (double)stateGroupsRoot.GetAnimationBaseValue(UIElement.OpacityProperty);
                                return num == 0.0;
                        }
                        return stateGroupsRoot.Opacity == 0.0;
                }
                private static bool TimelineIsAnimatingRootOpacity(Timeline timeline, FrameworkElement control, FrameworkElement stateGroupsRoot)
                {
                        if (ExtendedVisualStateManager.GetTimelineTarget(control, stateGroupsRoot, timeline) != stateGroupsRoot)
                        {
                                return false;
                        }
                        PropertyPath targetProperty = Storyboard.GetTargetProperty(timeline);
                        return targetProperty != null && targetProperty.PathParameters != null && targetProperty.PathParameters.Count != 0 && targetProperty.PathParameters[0] == UIElement.OpacityProperty;
                }
                private static void AnimateTransitionEffect(FrameworkElement stateGroupsRoot, VisualTransition transition)
                {
                        Effect arg_18_0 = stateGroupsRoot.Effect;
                        DoubleAnimation doubleAnimation = new DoubleAnimation();
                        doubleAnimation.Duration = transition.GeneratedDuration;
                        doubleAnimation.EasingFunction = transition.GeneratedEasingFunction;
                        doubleAnimation.From = new double?(0.0);
                        doubleAnimation.To = new double?(1.0);
                        Storyboard sb = new Storyboard();
                        sb.Duration = transition.GeneratedDuration;
                        sb.Children.Add(doubleAnimation);
                        Storyboard.SetTarget(doubleAnimation, stateGroupsRoot);
                        Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(0).(1)", new DependencyProperty[]
			{
				UIElement.EffectProperty,
				TransitionEffect.ProgressProperty
			}));
                        Panel panel = stateGroupsRoot as Panel;
                        if (panel != null && panel.Background == null)
                        {
                                ExtendedVisualStateManager.SetDidCacheBackground(panel, true);
                                ExtendedVisualStateManager.TransferLocalValue(panel, Panel.BackgroundProperty, ExtendedVisualStateManager.CachedBackgroundProperty);
                                panel.Background = Brushes.Transparent;
                        }
                        sb.Completed += delegate(object sender, EventArgs e)
                        {
                                Storyboard transitionEffectStoryboard = ExtendedVisualStateManager.GetTransitionEffectStoryboard(stateGroupsRoot);
                                if (transitionEffectStoryboard == sb)
                                {
                                        ExtendedVisualStateManager.FinishTransitionEffectAnimation(stateGroupsRoot);
                                }
                        };
                        ExtendedVisualStateManager.SetTransitionEffectStoryboard(stateGroupsRoot, sb);
                        sb.Begin();
                }
                private static void FinishTransitionEffectAnimation(FrameworkElement stateGroupsRoot)
                {
                        ExtendedVisualStateManager.SetTransitionEffectStoryboard(stateGroupsRoot, null);
                        ExtendedVisualStateManager.TransferLocalValue(stateGroupsRoot, ExtendedVisualStateManager.CachedEffectProperty, UIElement.EffectProperty);
                        if (ExtendedVisualStateManager.GetDidCacheBackground(stateGroupsRoot))
                        {
                                ExtendedVisualStateManager.TransferLocalValue(stateGroupsRoot, ExtendedVisualStateManager.CachedBackgroundProperty, Panel.BackgroundProperty);
                                ExtendedVisualStateManager.SetDidCacheBackground(stateGroupsRoot, false);
                        }
                }
                private static VisualTransition FindTransition(VisualStateGroup group, VisualState previousState, VisualState state)
                {
                        string b = (previousState != null) ? previousState.Name : string.Empty;
                        string b2 = (state != null) ? state.Name : string.Empty;
                        int num = -1;
                        VisualTransition result = null;
                        if (group.Transitions != null)
                        {
                                foreach (VisualTransition visualTransition in group.Transitions)
                                {
                                        int num2 = 0;
                                        if (visualTransition.From == b)
                                        {
                                                num2++;
                                        }
                                        else
                                        {
                                                if (!string.IsNullOrEmpty(visualTransition.From))
                                                {
                                                        continue;
                                                }
                                        }
                                        if (visualTransition.To == b2)
                                        {
                                                num2 += 2;
                                        }
                                        else
                                        {
                                                if (!string.IsNullOrEmpty(visualTransition.To))
                                                {
                                                        continue;
                                                }
                                        }
                                        if (num2 > num)
                                        {
                                                num = num2;
                                                result = visualTransition;
                                        }
                                }
                        }
                        return result;
                }
                private static Storyboard ExtractLayoutStoryboard(VisualState state)
                {
                        Storyboard storyboard = null;
                        if (state.Storyboard != null)
                        {
                                storyboard = ExtendedVisualStateManager.GetLayoutStoryboard(state.Storyboard);
                                if (storyboard == null)
                                {
                                        storyboard = new Storyboard();
                                        for (int i = state.Storyboard.Children.Count - 1; i >= 0; i--)
                                        {
                                                Timeline timeline = state.Storyboard.Children[i];
                                                if (ExtendedVisualStateManager.LayoutPropertyFromTimeline(timeline, false) != null)
                                                {
                                                        state.Storyboard.Children.RemoveAt(i);
                                                        storyboard.Children.Add(timeline);
                                                }
                                        }
                                        ExtendedVisualStateManager.SetLayoutStoryboard(state.Storyboard, storyboard);
                                }
                        }
                        if (storyboard == null)
                        {
                                return new Storyboard();
                        }
                        return storyboard;
                }
                private static List<FrameworkElement> FindTargetElements(FrameworkElement control, FrameworkElement templateRoot, Storyboard layoutStoryboard, List<ExtendedVisualStateManager.OriginalLayoutValueRecord> originalValueRecords, List<FrameworkElement> movingElements)
                {
                        List<FrameworkElement> list = new List<FrameworkElement>();
                        if (movingElements != null)
                        {
                                list.AddRange(movingElements);
                        }
                        foreach (Timeline current in layoutStoryboard.Children)
                        {
                                FrameworkElement frameworkElement = (FrameworkElement)ExtendedVisualStateManager.GetTimelineTarget(control, templateRoot, current);
                                if (frameworkElement != null)
                                {
                                        if (!list.Contains(frameworkElement))
                                        {
                                                list.Add(frameworkElement);
                                        }
                                        if (ExtendedVisualStateManager.ChildAffectingLayoutProperties.Contains(ExtendedVisualStateManager.LayoutPropertyFromTimeline(current, false)))
                                        {
                                                Panel panel = frameworkElement as Panel;
                                                if (panel != null)
                                                {
                                                        foreach (FrameworkElement frameworkElement2 in panel.Children)
                                                        {
                                                                if (!list.Contains(frameworkElement2) && !(frameworkElement2 is ExtendedVisualStateManager.WrapperCanvas))
                                                                {
                                                                        list.Add(frameworkElement2);
                                                                }
                                                        }
                                                }
                                        }
                                }
                        }
                        foreach (ExtendedVisualStateManager.OriginalLayoutValueRecord current2 in originalValueRecords)
                        {
                                if (!list.Contains(current2.Element))
                                {
                                        list.Add(current2.Element);
                                }
                                if (ExtendedVisualStateManager.ChildAffectingLayoutProperties.Contains(current2.Property))
                                {
                                        Panel panel2 = current2.Element as Panel;
                                        if (panel2 != null)
                                        {
                                                foreach (FrameworkElement frameworkElement3 in panel2.Children)
                                                {
                                                        if (!list.Contains(frameworkElement3) && !(frameworkElement3 is ExtendedVisualStateManager.WrapperCanvas))
                                                        {
                                                                list.Add(frameworkElement3);
                                                        }
                                                }
                                        }
                                }
                        }
                        for (int i = 0; i < list.Count; i++)
                        {
                                FrameworkElement frameworkElement4 = list[i];
                                FrameworkElement frameworkElement5 = VisualTreeHelper.GetParent(frameworkElement4) as FrameworkElement;
                                if (movingElements != null && movingElements.Contains(frameworkElement4) && frameworkElement5 is ExtendedVisualStateManager.WrapperCanvas)
                                {
                                        frameworkElement5 = (VisualTreeHelper.GetParent(frameworkElement5) as FrameworkElement);
                                }
                                if (frameworkElement5 != null)
                                {
                                        if (!list.Contains(frameworkElement5))
                                        {
                                                list.Add(frameworkElement5);
                                        }
                                        for (int j = 0; j < VisualTreeHelper.GetChildrenCount(frameworkElement5); j++)
                                        {
                                                FrameworkElement frameworkElement6 = VisualTreeHelper.GetChild(frameworkElement5, j) as FrameworkElement;
                                                if (frameworkElement6 != null && !list.Contains(frameworkElement6) && !(frameworkElement6 is ExtendedVisualStateManager.WrapperCanvas))
                                                {
                                                        list.Add(frameworkElement6);
                                                }
                                        }
                                }
                        }
                        return list;
                }
                private static object GetTimelineTarget(FrameworkElement control, FrameworkElement templateRoot, Timeline timeline)
                {
                        string targetName = Storyboard.GetTargetName(timeline);
                        if (string.IsNullOrEmpty(targetName))
                        {
                                return null;
                        }
                        if (control is UserControl)
                        {
                                return control.FindName(targetName);
                        }
                        return templateRoot.FindName(targetName);
                }
                private static Dictionary<FrameworkElement, Rect> GetRectsOfTargets(List<FrameworkElement> targets, List<FrameworkElement> movingElements)
                {
                        Dictionary<FrameworkElement, Rect> dictionary = new Dictionary<FrameworkElement, Rect>();
                        foreach (FrameworkElement current in targets)
                        {
                                Rect layoutRect;
                                if (movingElements != null && movingElements.Contains(current) && current.Parent is ExtendedVisualStateManager.WrapperCanvas)
                                {
                                        ExtendedVisualStateManager.WrapperCanvas wrapperCanvas = current.Parent as ExtendedVisualStateManager.WrapperCanvas;
                                        layoutRect = ExtendedVisualStateManager.GetLayoutRect(wrapperCanvas);
                                        TranslateTransform translateTransform = wrapperCanvas.RenderTransform as TranslateTransform;
                                        double left = Canvas.GetLeft(current);
                                        double top = Canvas.GetTop(current);
                                        layoutRect = new Rect(layoutRect.Left + (double.IsNaN(left) ? 0.0 : left) + ((translateTransform == null) ? 0.0 : translateTransform.X), layoutRect.Top + (double.IsNaN(top) ? 0.0 : top) + ((translateTransform == null) ? 0.0 : translateTransform.Y), current.ActualWidth, current.ActualHeight);
                                }
                                else
                                {
                                        layoutRect = ExtendedVisualStateManager.GetLayoutRect(current);
                                }
                                dictionary.Add(current, layoutRect);
                        }
                        return dictionary;
                }
                internal static Rect GetLayoutRect(FrameworkElement element)
                {
                        double num = element.ActualWidth;
                        double num2 = element.ActualHeight;
                        if (element is Image || element is MediaElement)
                        {
                                if (element.Parent is Canvas)
                                {
                                        num = (double.IsNaN(element.Width) ? num : element.Width);
                                        num2 = (double.IsNaN(element.Height) ? num2 : element.Height);
                                }
                                else
                                {
                                        num = element.RenderSize.Width;
                                        num2 = element.RenderSize.Height;
                                }
                        }
                        num = ((element.Visibility == Visibility.Collapsed) ? 0.0 : num);
                        num2 = ((element.Visibility == Visibility.Collapsed) ? 0.0 : num2);
                        Thickness margin = element.Margin;
                        Rect layoutSlot = LayoutInformation.GetLayoutSlot(element);
                        double x = 0.0;
                        double y = 0.0;
                        switch (element.HorizontalAlignment)
                        {
                                case HorizontalAlignment.Left:
                                        x = layoutSlot.Left + margin.Left;
                                        break;
                                case HorizontalAlignment.Center:
                                        x = (layoutSlot.Left + margin.Left + layoutSlot.Right - margin.Right) / 2.0 - num / 2.0;
                                        break;
                                case HorizontalAlignment.Right:
                                        x = layoutSlot.Right - margin.Right - num;
                                        break;
                                case HorizontalAlignment.Stretch:
                                        x = Math.Max(layoutSlot.Left + margin.Left, (layoutSlot.Left + margin.Left + layoutSlot.Right - margin.Right) / 2.0 - num / 2.0);
                                        break;
                        }
                        switch (element.VerticalAlignment)
                        {
                                case VerticalAlignment.Top:
                                        y = layoutSlot.Top + margin.Top;
                                        break;
                                case VerticalAlignment.Center:
                                        y = (layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0 - num2 / 2.0;
                                        break;
                                case VerticalAlignment.Bottom:
                                        y = layoutSlot.Bottom - margin.Bottom - num2;
                                        break;
                                case VerticalAlignment.Stretch:
                                        y = Math.Max(layoutSlot.Top + margin.Top, (layoutSlot.Top + margin.Top + layoutSlot.Bottom - margin.Bottom) / 2.0 - num2 / 2.0);
                                        break;
                        }
                        return new Rect(x, y, num, num2);
                }
                private static Dictionary<FrameworkElement, double> GetOldOpacities(FrameworkElement control, FrameworkElement templateRoot, Storyboard layoutStoryboard, List<ExtendedVisualStateManager.OriginalLayoutValueRecord> originalValueRecords, List<FrameworkElement> movingElements)
                {
                        Dictionary<FrameworkElement, double> dictionary = new Dictionary<FrameworkElement, double>();
                        if (movingElements != null)
                        {
                                foreach (FrameworkElement current in movingElements)
                                {
                                        ExtendedVisualStateManager.WrapperCanvas wrapperCanvas = current.Parent as ExtendedVisualStateManager.WrapperCanvas;
                                        if (wrapperCanvas != null)
                                        {
                                                dictionary.Add(current, wrapperCanvas.Opacity);
                                        }
                                }
                        }
                        for (int i = originalValueRecords.Count - 1; i >= 0; i--)
                        {
                                ExtendedVisualStateManager.OriginalLayoutValueRecord originalLayoutValueRecord = originalValueRecords[i];
                                double value;
                                if (ExtendedVisualStateManager.IsVisibilityProperty(originalLayoutValueRecord.Property) && !dictionary.TryGetValue(originalLayoutValueRecord.Element, out value))
                                {
                                        value = (((Visibility)originalLayoutValueRecord.Element.GetValue(originalLayoutValueRecord.Property) == Visibility.Visible) ? 1.0 : 0.0);
                                        dictionary.Add(originalLayoutValueRecord.Element, value);
                                }
                        }
                        foreach (Timeline current2 in layoutStoryboard.Children)
                        {
                                FrameworkElement frameworkElement = (FrameworkElement)ExtendedVisualStateManager.GetTimelineTarget(control, templateRoot, current2);
                                DependencyProperty dependencyProperty = ExtendedVisualStateManager.LayoutPropertyFromTimeline(current2, true);
                                double value2;
                                if (frameworkElement != null && ExtendedVisualStateManager.IsVisibilityProperty(dependencyProperty) && !dictionary.TryGetValue(frameworkElement, out value2))
                                {
                                        value2 = (((Visibility)frameworkElement.GetValue(dependencyProperty) == Visibility.Visible) ? 1.0 : 0.0);
                                        dictionary.Add(frameworkElement, value2);
                                }
                        }
                        return dictionary;
                }
                private static void SetLayoutStoryboardProperties(FrameworkElement control, FrameworkElement templateRoot, Storyboard layoutStoryboard, List<ExtendedVisualStateManager.OriginalLayoutValueRecord> originalValueRecords)
                {
                        foreach (ExtendedVisualStateManager.OriginalLayoutValueRecord current in originalValueRecords)
                        {
                                ExtendedVisualStateManager.ReplaceCachedLocalValueHelper(current.Element, current.Property, current.Value);
                        }
                        originalValueRecords.Clear();
                        foreach (Timeline current2 in layoutStoryboard.Children)
                        {
                                FrameworkElement frameworkElement = (FrameworkElement)ExtendedVisualStateManager.GetTimelineTarget(control, templateRoot, current2);
                                DependencyProperty dependencyProperty = ExtendedVisualStateManager.LayoutPropertyFromTimeline(current2, true);
                                if (frameworkElement != null && dependencyProperty != null)
                                {
                                        bool flag;
                                        object valueFromTimeline = ExtendedVisualStateManager.GetValueFromTimeline(current2, out flag);
                                        if (flag)
                                        {
                                                originalValueRecords.Add(new ExtendedVisualStateManager.OriginalLayoutValueRecord
                                                {
                                                        Element = frameworkElement,
                                                        Property = dependencyProperty,
                                                        Value = ExtendedVisualStateManager.CacheLocalValueHelper(frameworkElement, dependencyProperty)
                                                });
                                                frameworkElement.SetValue(dependencyProperty, valueFromTimeline);
                                        }
                                }
                        }
                }
                private static object GetValueFromTimeline(Timeline timeline, out bool gotValue)
                {
                        ObjectAnimationUsingKeyFrames objectAnimationUsingKeyFrames = timeline as ObjectAnimationUsingKeyFrames;
                        if (objectAnimationUsingKeyFrames != null)
                        {
                                gotValue = true;
                                return objectAnimationUsingKeyFrames.KeyFrames[0].Value;
                        }
                        DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = timeline as DoubleAnimationUsingKeyFrames;
                        if (doubleAnimationUsingKeyFrames != null)
                        {
                                gotValue = true;
                                return doubleAnimationUsingKeyFrames.KeyFrames[0].Value;
                        }
                        DoubleAnimation doubleAnimation = timeline as DoubleAnimation;
                        if (doubleAnimation != null)
                        {
                                gotValue = true;
                                return doubleAnimation.To;
                        }
                        ThicknessAnimationUsingKeyFrames thicknessAnimationUsingKeyFrames = timeline as ThicknessAnimationUsingKeyFrames;
                        if (thicknessAnimationUsingKeyFrames != null)
                        {
                                gotValue = true;
                                return thicknessAnimationUsingKeyFrames.KeyFrames[0].Value;
                        }
                        ThicknessAnimation thicknessAnimation = timeline as ThicknessAnimation;
                        if (thicknessAnimation != null)
                        {
                                gotValue = true;
                                return thicknessAnimation.To;
                        }
                        Int32AnimationUsingKeyFrames int32AnimationUsingKeyFrames = timeline as Int32AnimationUsingKeyFrames;
                        if (int32AnimationUsingKeyFrames != null)
                        {
                                gotValue = true;
                                return int32AnimationUsingKeyFrames.KeyFrames[0].Value;
                        }
                        Int32Animation int32Animation = timeline as Int32Animation;
                        if (int32Animation != null)
                        {
                                gotValue = true;
                                return int32Animation.To;
                        }
                        gotValue = false;
                        return null;
                }
                private static void WrapMovingElementsInCanvases(List<FrameworkElement> movingElements, Dictionary<FrameworkElement, Rect> oldRects, Dictionary<FrameworkElement, Rect> newRects)
                {
                        foreach (FrameworkElement current in movingElements)
                        {
                                FrameworkElement frameworkElement = VisualTreeHelper.GetParent(current) as FrameworkElement;
                                ExtendedVisualStateManager.WrapperCanvas wrapperCanvas = new ExtendedVisualStateManager.WrapperCanvas();
                                wrapperCanvas.OldRect = oldRects[current];
                                wrapperCanvas.NewRect = newRects[current];
                                object value = ExtendedVisualStateManager.CacheLocalValueHelper(current, FrameworkElement.DataContextProperty);
                                current.DataContext = current.DataContext;
                                bool flag = true;
                                Panel panel = frameworkElement as Panel;
                                if (panel != null && !panel.IsItemsHost)
                                {
                                        int index = panel.Children.IndexOf(current);
                                        panel.Children.RemoveAt(index);
                                        panel.Children.Insert(index, wrapperCanvas);
                                }
                                else
                                {
                                        Decorator decorator = frameworkElement as Decorator;
                                        if (decorator != null)
                                        {
                                                decorator.Child = wrapperCanvas;
                                        }
                                        else
                                        {
                                                flag = false;
                                        }
                                }
                                if (flag)
                                {
                                        wrapperCanvas.Children.Add(current);
                                        ExtendedVisualStateManager.CopyLayoutProperties(current, wrapperCanvas, false);
                                        ExtendedVisualStateManager.ReplaceCachedLocalValueHelper(current, FrameworkElement.DataContextProperty, value);
                                }
                        }
                }
                private static void UnwrapMovingElementsFromCanvases(List<FrameworkElement> movingElements)
                {
                        foreach (FrameworkElement current in movingElements)
                        {
                                ExtendedVisualStateManager.WrapperCanvas wrapperCanvas = current.Parent as ExtendedVisualStateManager.WrapperCanvas;
                                if (wrapperCanvas != null)
                                {
                                        object value = ExtendedVisualStateManager.CacheLocalValueHelper(current, FrameworkElement.DataContextProperty);
                                        current.DataContext = current.DataContext;
                                        FrameworkElement frameworkElement = VisualTreeHelper.GetParent(wrapperCanvas) as FrameworkElement;
                                        wrapperCanvas.Children.Remove(current);
                                        Panel panel = frameworkElement as Panel;
                                        if (panel != null)
                                        {
                                                int index = panel.Children.IndexOf(wrapperCanvas);
                                                panel.Children.RemoveAt(index);
                                                panel.Children.Insert(index, current);
                                        }
                                        else
                                        {
                                                Decorator decorator = frameworkElement as Decorator;
                                                if (decorator != null)
                                                {
                                                        decorator.Child = current;
                                                }
                                        }
                                        ExtendedVisualStateManager.CopyLayoutProperties(wrapperCanvas, current, true);
                                        ExtendedVisualStateManager.ReplaceCachedLocalValueHelper(current, FrameworkElement.DataContextProperty, value);
                                }
                        }
                }
                private static void CopyLayoutProperties(FrameworkElement source, FrameworkElement target, bool restoring)
                {
                        ExtendedVisualStateManager.WrapperCanvas wrapperCanvas = (restoring ? source : target) as ExtendedVisualStateManager.WrapperCanvas;
                        if (wrapperCanvas.LocalValueCache == null)
                        {
                                wrapperCanvas.LocalValueCache = new Dictionary<DependencyProperty, object>();
                        }
                        foreach (DependencyProperty current in ExtendedVisualStateManager.LayoutProperties)
                        {
                                if (!ExtendedVisualStateManager.ChildAffectingLayoutProperties.Contains(current))
                                {
                                        if (restoring)
                                        {
                                                ExtendedVisualStateManager.ReplaceCachedLocalValueHelper(target, current, wrapperCanvas.LocalValueCache[current]);
                                        }
                                        else
                                        {
                                                object value = target.GetValue(current);
                                                object value2 = ExtendedVisualStateManager.CacheLocalValueHelper(source, current);
                                                wrapperCanvas.LocalValueCache[current] = value2;
                                                if (ExtendedVisualStateManager.IsVisibilityProperty(current))
                                                {
                                                        wrapperCanvas.DestinationVisibilityCache = (Visibility)source.GetValue(current);
                                                }
                                                else
                                                {
                                                        target.SetValue(current, source.GetValue(current));
                                                }
                                                source.SetValue(current, value);
                                        }
                                }
                        }
                }
                private static Storyboard CreateLayoutTransitionStoryboard(VisualTransition transition, List<FrameworkElement> movingElements, Dictionary<FrameworkElement, double> oldOpacities)
                {
                        Duration duration = (transition != null) ? transition.GeneratedDuration : new Duration(TimeSpan.Zero);
                        IEasingFunction easingFunction = (transition != null) ? transition.GeneratedEasingFunction : null;
                        Storyboard storyboard = new Storyboard();
                        storyboard.Duration = duration;
                        foreach (FrameworkElement current in movingElements)
                        {
                                ExtendedVisualStateManager.WrapperCanvas wrapperCanvas = current.Parent as ExtendedVisualStateManager.WrapperCanvas;
                                if (wrapperCanvas != null)
                                {
                                        DoubleAnimation doubleAnimation = new DoubleAnimation
                                        {
                                                From = new double?(1.0),
                                                To = new double?(0.0),
                                                Duration = duration
                                        };
                                        doubleAnimation.EasingFunction = easingFunction;
                                        Storyboard.SetTarget(doubleAnimation, wrapperCanvas);
                                        Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(ExtendedVisualStateManager.WrapperCanvas.SimulationProgressProperty));
                                        storyboard.Children.Add(doubleAnimation);
                                        wrapperCanvas.SimulationProgress = 1.0;
                                        Rect newRect = wrapperCanvas.NewRect;
                                        if (!ExtendedVisualStateManager.IsClose(wrapperCanvas.Width, newRect.Width))
                                        {
                                                DoubleAnimation doubleAnimation2 = new DoubleAnimation
                                                {
                                                        From = new double?(newRect.Width),
                                                        To = new double?(newRect.Width),
                                                        Duration = duration
                                                };
                                                Storyboard.SetTarget(doubleAnimation2, wrapperCanvas);
                                                Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath(FrameworkElement.WidthProperty));
                                                storyboard.Children.Add(doubleAnimation2);
                                        }
                                        if (!ExtendedVisualStateManager.IsClose(wrapperCanvas.Height, newRect.Height))
                                        {
                                                DoubleAnimation doubleAnimation3 = new DoubleAnimation
                                                {
                                                        From = new double?(newRect.Height),
                                                        To = new double?(newRect.Height),
                                                        Duration = duration
                                                };
                                                Storyboard.SetTarget(doubleAnimation3, wrapperCanvas);
                                                Storyboard.SetTargetProperty(doubleAnimation3, new PropertyPath(FrameworkElement.HeightProperty));
                                                storyboard.Children.Add(doubleAnimation3);
                                        }
                                        if (wrapperCanvas.DestinationVisibilityCache == Visibility.Collapsed)
                                        {
                                                Thickness margin = wrapperCanvas.Margin;
                                                if (!ExtendedVisualStateManager.IsClose(margin.Left, 0.0) || !ExtendedVisualStateManager.IsClose(margin.Top, 0.0) || !ExtendedVisualStateManager.IsClose(margin.Right, 0.0) || !ExtendedVisualStateManager.IsClose(margin.Bottom, 0.0))
                                                {
                                                        ObjectAnimationUsingKeyFrames objectAnimationUsingKeyFrames = new ObjectAnimationUsingKeyFrames
                                                        {
                                                                Duration = duration
                                                        };
                                                        DiscreteObjectKeyFrame keyFrame = new DiscreteObjectKeyFrame
                                                        {
                                                                KeyTime = TimeSpan.Zero,
                                                                Value = default(Thickness)
                                                        };
                                                        objectAnimationUsingKeyFrames.KeyFrames.Add(keyFrame);
                                                        Storyboard.SetTarget(objectAnimationUsingKeyFrames, wrapperCanvas);
                                                        Storyboard.SetTargetProperty(objectAnimationUsingKeyFrames, new PropertyPath(FrameworkElement.MarginProperty));
                                                        storyboard.Children.Add(objectAnimationUsingKeyFrames);
                                                }
                                                if (!ExtendedVisualStateManager.IsClose(wrapperCanvas.MinWidth, 0.0))
                                                {
                                                        DoubleAnimation doubleAnimation4 = new DoubleAnimation
                                                        {
                                                                From = new double?(0.0),
                                                                To = new double?(0.0),
                                                                Duration = duration
                                                        };
                                                        Storyboard.SetTarget(doubleAnimation4, wrapperCanvas);
                                                        Storyboard.SetTargetProperty(doubleAnimation4, new PropertyPath(FrameworkElement.MinWidthProperty));
                                                        storyboard.Children.Add(doubleAnimation4);
                                                }
                                                if (!ExtendedVisualStateManager.IsClose(wrapperCanvas.MinHeight, 0.0))
                                                {
                                                        DoubleAnimation doubleAnimation5 = new DoubleAnimation
                                                        {
                                                                From = new double?(0.0),
                                                                To = new double?(0.0),
                                                                Duration = duration
                                                        };
                                                        Storyboard.SetTarget(doubleAnimation5, wrapperCanvas);
                                                        Storyboard.SetTargetProperty(doubleAnimation5, new PropertyPath(FrameworkElement.MinHeightProperty));
                                                        storyboard.Children.Add(doubleAnimation5);
                                                }
                                        }
                                }
                        }
                        foreach (FrameworkElement current2 in oldOpacities.Keys)
                        {
                                ExtendedVisualStateManager.WrapperCanvas wrapperCanvas2 = current2.Parent as ExtendedVisualStateManager.WrapperCanvas;
                                if (wrapperCanvas2 != null)
                                {
                                        double num = oldOpacities[current2];
                                        double num2 = (wrapperCanvas2.DestinationVisibilityCache == Visibility.Visible) ? 1.0 : 0.0;
                                        if (!ExtendedVisualStateManager.IsClose(num, 1.0) || !ExtendedVisualStateManager.IsClose(num2, 1.0))
                                        {
                                                DoubleAnimation doubleAnimation6 = new DoubleAnimation
                                                {
                                                        From = new double?(num),
                                                        To = new double?(num2),
                                                        Duration = duration
                                                };
                                                doubleAnimation6.EasingFunction = easingFunction;
                                                Storyboard.SetTarget(doubleAnimation6, wrapperCanvas2);
                                                Storyboard.SetTargetProperty(doubleAnimation6, new PropertyPath(UIElement.OpacityProperty));
                                                storyboard.Children.Add(doubleAnimation6);
                                        }
                                }
                        }
                        return storyboard;
                }
                private static void TransferLocalValue(FrameworkElement element, DependencyProperty sourceProperty, DependencyProperty destProperty)
                {
                        object value = ExtendedVisualStateManager.CacheLocalValueHelper(element, sourceProperty);
                        ExtendedVisualStateManager.ReplaceCachedLocalValueHelper(element, destProperty, value);
                }
                private static object CacheLocalValueHelper(DependencyObject dependencyObject, DependencyProperty property)
                {
                        return dependencyObject.ReadLocalValue(property);
                }
                private static void ReplaceCachedLocalValueHelper(FrameworkElement element, DependencyProperty property, object value)
                {
                        if (value == DependencyProperty.UnsetValue)
                        {
                                element.ClearValue(property);
                                return;
                        }
                        BindingExpressionBase bindingExpressionBase = value as BindingExpressionBase;
                        if (bindingExpressionBase != null)
                        {
                                element.SetBinding(property, bindingExpressionBase.ParentBindingBase);
                                return;
                        }
                        element.SetValue(property, value);
                }
                private static bool IsClose(double a, double b)
                {
                        return Math.Abs(a - b) < 1E-07;
                }
        }
}
