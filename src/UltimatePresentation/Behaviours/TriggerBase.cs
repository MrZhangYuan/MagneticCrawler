using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace UltimatePresentation.Behaviours
{
        public class PreviewInvokeEventArgs : EventArgs
        {
                public bool Cancelling
                {
                        get;
                        set;
                }
        }

        [ContentProperty("Actions")]
        public abstract class TriggerBase : Animatable, IAttachedObject
        {
                private DependencyObject associatedObject;
                private Type associatedObjectTypeConstraint;
                private static readonly DependencyPropertyKey ActionsPropertyKey = DependencyProperty.RegisterReadOnly("Actions", typeof(TriggerActionCollection), typeof(TriggerBase), new FrameworkPropertyMetadata());
                public static readonly DependencyProperty ActionsProperty = TriggerBase.ActionsPropertyKey.DependencyProperty;
                public event EventHandler<PreviewInvokeEventArgs> PreviewInvoke;
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
                public TriggerActionCollection Actions
                {
                        get
                        {
                                return (TriggerActionCollection)base.GetValue(TriggerBase.ActionsProperty);
                        }
                }
                DependencyObject IAttachedObject.AssociatedObject
                {
                        get
                        {
                                return this.AssociatedObject;
                        }
                }
                internal TriggerBase(Type associatedObjectTypeConstraint)
                {
                        this.associatedObjectTypeConstraint = associatedObjectTypeConstraint;
                        TriggerActionCollection value = new TriggerActionCollection();
                        base.SetValue(TriggerBase.ActionsPropertyKey, value);
                }
                protected void InvokeActions(object parameter)
                {
                        if (this.PreviewInvoke != null)
                        {
                                PreviewInvokeEventArgs previewInvokeEventArgs = new PreviewInvokeEventArgs();
                                this.PreviewInvoke(this, previewInvokeEventArgs);
                                if (previewInvokeEventArgs.Cancelling)
                                {
                                        return;
                                }
                        }
                        foreach (TriggerAction current in this.Actions)
                        {
                                current.CallInvoke(parameter);
                        }
                }
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
                                        throw new InvalidOperationException("CannotHostTriggerMultipleTimesExceptionMessage");
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
                                this.Actions.Attach(dependencyObject);
                                this.OnAttached();
                        }
                }
                public void Detach()
                {
                        this.OnDetaching();
                        base.WritePreamble();
                        this.associatedObject = null;
                        base.WritePostscript();
                        this.Actions.Detach();
                }
        }
}
