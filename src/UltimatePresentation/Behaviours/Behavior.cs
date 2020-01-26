using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace UltimatePresentation.Behaviours
{
        public abstract class Behavior : Animatable, IAttachedObject
        {
                private Type associatedType;
                private DependencyObject associatedObject;
                internal event EventHandler AssociatedObjectChanged;
                protected Type AssociatedType
                {
                        get
                        {
                                base.ReadPreamble();
                                return this.associatedType;
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
                DependencyObject IAttachedObject.AssociatedObject
                {
                        get
                        {
                                return this.AssociatedObject;
                        }
                }
                internal Behavior(Type associatedType)
                {
                        this.associatedType = associatedType;
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
                private void OnAssociatedObjectChanged()
                {
                        if (this.AssociatedObjectChanged != null)
                        {
                                this.AssociatedObjectChanged(this, new EventArgs());
                        }
                }
                public void Attach(DependencyObject dependencyObject)
                {
                        if (dependencyObject != this.AssociatedObject)
                        {
                                if (this.AssociatedObject != null)
                                {
                                        throw new InvalidOperationException();
                                }
                                if (dependencyObject != null && !this.AssociatedType.IsAssignableFrom(dependencyObject.GetType()))
                                {
                                        throw new InvalidOperationException();
                                }
                                base.WritePreamble();
                                this.associatedObject = dependencyObject;
                                base.WritePostscript();
                                this.OnAssociatedObjectChanged();
                                this.OnAttached();
                        }
                }
                public void Detach()
                {
                        this.OnDetaching();
                        base.WritePreamble();
                        this.associatedObject = null;
                        base.WritePostscript();
                        this.OnAssociatedObjectChanged();
                }
        }


        public abstract class Behavior<T> : Behavior where T : DependencyObject
        {
                protected new T AssociatedObject
                {
                        get
                        {
                                return (T)((object)base.AssociatedObject);
                        }
                }
                protected Behavior()
                        : base(typeof(T))
                {
                }
        }

}
