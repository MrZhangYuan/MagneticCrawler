using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace UltimatePresentation.Behaviours
{
        [DefaultTrigger(typeof(UIElement), typeof(EventTrigger), "MouseLeftButtonDown"), DefaultTrigger(typeof(ButtonBase), typeof(EventTrigger), "Click")]
        public abstract class TriggerAction : Animatable, IAttachedObject
        {
                private bool isHosted;
                private DependencyObject associatedObject;
                private Type associatedObjectTypeConstraint;
                public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(TriggerAction), new FrameworkPropertyMetadata(true));
                public bool IsEnabled
                {
                        get
                        {
                                return (bool)base.GetValue(TriggerAction.IsEnabledProperty);
                        }
                        set
                        {
                                base.SetValue(TriggerAction.IsEnabledProperty, value);
                        }
                }
                protected DependencyObject AssociatedObject
                {
                        get
                        {
                                base.ReadPreamble();
                                return this.associatedObject;
                        }
                }
                protected virtual Type AssociatedObjectTypeConstraint
                {
                        get
                        {
                                base.ReadPreamble();
                                return this.associatedObjectTypeConstraint;
                        }
                }
                internal bool IsHosted
                {
                        get
                        {
                                base.ReadPreamble();
                                return this.isHosted;
                        }
                        set
                        {
                                base.WritePreamble();
                                this.isHosted = value;
                                base.WritePostscript();
                        }
                }
                DependencyObject IAttachedObject.AssociatedObject
                {
                        get
                        {
                                return this.AssociatedObject;
                        }
                }
                internal TriggerAction(Type associatedObjectTypeConstraint)
                {
                        this.associatedObjectTypeConstraint = associatedObjectTypeConstraint;
                }
                internal void CallInvoke(object parameter)
                {
                        if (this.IsEnabled)
                        {
                                this.Invoke(parameter);
                        }
                }
                protected abstract void Invoke(object parameter);
                protected virtual void OnAttached()
                {
                }
                protected virtual void OnDetaching()
                {
                }
                protected override Freezable CreateInstanceCore()
                {
                        Type type = base.GetType();
                        return (Freezable)Activator.CreateInstance(type);
                }
                public void Attach(DependencyObject dependencyObject)
                {
                        if (dependencyObject != this.AssociatedObject)
                        {
                                if (this.AssociatedObject != null)
                                {
                                        throw new InvalidOperationException("CannotHostTriggerActionMultipleTimesExceptionMessage");
                                }
                                if (dependencyObject != null && !this.AssociatedObjectTypeConstraint.IsAssignableFrom(dependencyObject.GetType()))
                                {
                                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "TypeConstraintViolatedExceptionMessage_{0}_{1}_{2}", new object[]
					{
						base.GetType().Name,
						dependencyObject.GetType().Name,
						this.AssociatedObjectTypeConstraint.Name
					}));
                                }
                                base.WritePreamble();
                                this.associatedObject = dependencyObject;
                                base.WritePostscript();
                                this.OnAttached();
                        }
                }
                public void Detach()
                {
                        this.OnDetaching();
                        base.WritePreamble();
                        this.associatedObject = null;
                        base.WritePostscript();
                }
        }
}
