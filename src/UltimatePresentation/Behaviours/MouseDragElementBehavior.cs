using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UltimatePresentation.Presentation;

namespace UltimatePresentation.Behaviours
{
    public enum DragOrientation
    {
        Horizontal,
        Vertical,
        Both
    }

    public class MouseDragElementBehavior : Behavior<FrameworkElement>
    {
        private bool settingPosition;
        private Point relativePosition;
        private Transform cachedRenderTransform;
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register(
                "X",
                typeof(double),
                typeof(MouseDragElementBehavior),
                new PropertyMetadata(double.NaN, new PropertyChangedCallback(MouseDragElementBehavior.OnXChanged)));

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register(
                "Y",
                typeof(double),
                typeof(MouseDragElementBehavior),
                new PropertyMetadata(double.NaN, new PropertyChangedCallback(MouseDragElementBehavior.OnYChanged)));

        public static readonly DependencyProperty ConstrainToParentBoundsProperty =
            DependencyProperty.Register(
                "ConstrainToParentBounds",
                typeof(bool),
                typeof(MouseDragElementBehavior),
                new PropertyMetadata(false, new PropertyChangedCallback(MouseDragElementBehavior.OnConstrainToParentBoundsChanged)));

        public event MouseEventHandler DragBegun;
        public event MouseEventHandler Dragging;
        public event MouseEventHandler DragFinished;
        public double X
        {
            get
            {
                return (double)base.GetValue(MouseDragElementBehavior.XProperty);
            }
            set
            {
                base.SetValue(MouseDragElementBehavior.XProperty, value);
            }
        }
        public double Y
        {
            get
            {
                return (double)base.GetValue(MouseDragElementBehavior.YProperty);
            }
            set
            {
                base.SetValue(MouseDragElementBehavior.YProperty, value);
            }
        }
        public bool ConstrainToParentBounds
        {
            get
            {
                return (bool)base.GetValue(MouseDragElementBehavior.ConstrainToParentBoundsProperty);
            }
            set
            {
                base.SetValue(MouseDragElementBehavior.ConstrainToParentBoundsProperty, value);
            }
        }
        private Point ActualPosition
        {
            get
            {
                GeneralTransform transform = base.AssociatedObject.TransformToVisual(this.RootElement);
                Point transformOffset = MouseDragElementBehavior.GetTransformOffset(transform);
                return new Point(transformOffset.X, transformOffset.Y);
            }
        }
        private Rect ElementBounds
        {
            get
            {
                Rect layoutRect = ExtendedVisualStateManager.GetLayoutRect(base.AssociatedObject);
                return new Rect(new Point(0.0, 0.0), new Size(layoutRect.Width, layoutRect.Height));
            }
        }
        private FrameworkElement ParentElement
        {
            get
            {
                return base.AssociatedObject.Parent as FrameworkElement;
            }
        }
        private UIElement RootElement
        {
            get
            {
                DependencyObject dependencyObject = base.AssociatedObject;
                for (DependencyObject dependencyObject2 = dependencyObject; dependencyObject2 != null; dependencyObject2 = VisualTreeHelper.GetParent(dependencyObject))
                {
                    dependencyObject = dependencyObject2;
                }
                return dependencyObject as UIElement;
            }
        }
        private Transform RenderTransform
        {
            get
            {
                if (this.cachedRenderTransform == null || !object.ReferenceEquals(this.cachedRenderTransform, base.AssociatedObject.RenderTransform))
                {
                    Transform renderTransform = MouseDragElementBehavior.CloneTransform(base.AssociatedObject.RenderTransform);
                    this.RenderTransform = renderTransform;
                }
                return this.cachedRenderTransform;
            }
            set
            {
                if (this.cachedRenderTransform != value)
                {
                    this.cachedRenderTransform = value;
                    base.AssociatedObject.RenderTransform = value;
                }
            }
        }
        private static void OnXChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            MouseDragElementBehavior mouseDragElementBehavior = (MouseDragElementBehavior)sender;
            mouseDragElementBehavior.UpdatePosition(new Point((double)args.NewValue, mouseDragElementBehavior.Y));
        }
        private static void OnYChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            MouseDragElementBehavior mouseDragElementBehavior = (MouseDragElementBehavior)sender;
            mouseDragElementBehavior.UpdatePosition(new Point(mouseDragElementBehavior.X, (double)args.NewValue));
        }
        private static void OnConstrainToParentBoundsChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            MouseDragElementBehavior mouseDragElementBehavior = (MouseDragElementBehavior)sender;
            mouseDragElementBehavior.UpdatePosition(new Point(mouseDragElementBehavior.X, mouseDragElementBehavior.Y));
        }
        private void UpdatePosition(Point point)
        {
            if (!this.settingPosition && base.AssociatedObject != null)
            {
                GeneralTransform transform = base.AssociatedObject.TransformToVisual(this.RootElement);
                Point transformOffset = MouseDragElementBehavior.GetTransformOffset(transform);
                double x = double.IsNaN(point.X) ? 0.0 : (point.X - transformOffset.X);
                double y = double.IsNaN(point.Y) ? 0.0 : (point.Y - transformOffset.Y);
                this.ApplyTranslation(x, y);
            }
        }
        private void ApplyTranslation(double x, double y)
        {
            if (this.ParentElement != null)
            {
                GeneralTransform transform = this.RootElement.TransformToVisual(this.ParentElement);
                Point point = MouseDragElementBehavior.TransformAsVector(transform, x, y);
                x = point.X;
                y = point.Y;
                if (this.ConstrainToParentBounds)
                {
                    FrameworkElement parentElement = this.ParentElement;
                    Rect rect = new Rect(0.0, 0.0, parentElement.ActualWidth, parentElement.ActualHeight);
                    GeneralTransform generalTransform = base.AssociatedObject.TransformToVisual(parentElement);
                    Rect rect2 = this.ElementBounds;
                    rect2 = generalTransform.TransformBounds(rect2);
                    Rect rect3 = rect2;
                    rect3.X += x;
                    rect3.Y += y;
                    if (!MouseDragElementBehavior.RectContainsRect(rect, rect3))
                    {
                        if (rect3.X < rect.Left)
                        {
                            double num = rect3.X - rect.Left;
                            x -= num;
                        }
                        else
                        {
                            if (rect3.Right > rect.Right)
                            {
                                double num2 = rect3.Right - rect.Right;
                                x -= num2;
                            }
                        }
                        if (rect3.Y < rect.Top)
                        {
                            double num3 = rect3.Y - rect.Top;
                            y -= num3;
                        }
                        else
                        {
                            if (rect3.Bottom > rect.Bottom)
                            {
                                double num4 = rect3.Bottom - rect.Bottom;
                                y -= num4;
                            }
                        }
                    }
                }
                this.ApplyTranslationTransform(x, y);
            }
        }
        internal void ApplyTranslationTransform(double x, double y)
        {
            Transform renderTransform = this.RenderTransform;
            TranslateTransform translateTransform = renderTransform as TranslateTransform;
            if (translateTransform == null)
            {
                TransformGroup transformGroup = renderTransform as TransformGroup;
                MatrixTransform matrixTransform = renderTransform as MatrixTransform;
                if (transformGroup != null)
                {
                    if (transformGroup.Children.Count > 0)
                    {
                        translateTransform = (transformGroup.Children[transformGroup.Children.Count - 1] as TranslateTransform);
                    }
                    if (translateTransform == null)
                    {
                        translateTransform = new TranslateTransform();
                        transformGroup.Children.Add(translateTransform);
                    }
                }
                else
                {
                    if (matrixTransform != null)
                    {
                        Matrix matrix = matrixTransform.Matrix;
                        matrix.OffsetX += x;
                        matrix.OffsetY += y;
                        this.RenderTransform = new MatrixTransform
                        {
                            Matrix = matrix
                        };
                        return;
                    }
                    TransformGroup transformGroup2 = new TransformGroup();
                    translateTransform = new TranslateTransform();
                    if (renderTransform != null)
                    {
                        transformGroup2.Children.Add(renderTransform);
                    }
                    transformGroup2.Children.Add(translateTransform);
                    this.RenderTransform = transformGroup2;
                }
            }
            //translateTransform.X += x;
            //translateTransform.Y += y;


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            switch (this.DragOrientation)
            {
                case DragOrientation.Horizontal:
                    translateTransform.X += x;
                    translateTransform.Y = 0;
                    break;
                case DragOrientation.Vertical:
                    translateTransform.X = 0;
                    translateTransform.Y += y;
                    break;
                case DragOrientation.Both:
                    translateTransform.X += x;
                    translateTransform.Y += y;
                    break;
            }
        }

        public DragOrientation DragOrientation
        {
            get
            {
                return (DragOrientation)GetValue(DragOrientationProperty);
            }
            set
            {
                SetValue(DragOrientationProperty, value);
            }
        }
        public static readonly DependencyProperty DragOrientationProperty =
            DependencyProperty.Register(
                "DragOrientation",
                typeof(DragOrientation),
                typeof(MouseDragElementBehavior),
                new PropertyMetadata(DragOrientation.Both));












        internal static Transform CloneTransform(Transform transform)
        {
            if (transform == null)
            {
                return null;
            }
            transform.GetType();
            ScaleTransform scaleTransform;
            if ((scaleTransform = (transform as ScaleTransform)) != null)
            {
                return new ScaleTransform
                {
                    CenterX = scaleTransform.CenterX,
                    CenterY = scaleTransform.CenterY,
                    ScaleX = scaleTransform.ScaleX,
                    ScaleY = scaleTransform.ScaleY
                };
            }
            RotateTransform rotateTransform;
            if ((rotateTransform = (transform as RotateTransform)) != null)
            {
                return new RotateTransform
                {
                    Angle = rotateTransform.Angle,
                    CenterX = rotateTransform.CenterX,
                    CenterY = rotateTransform.CenterY
                };
            }
            SkewTransform skewTransform;
            if ((skewTransform = (transform as SkewTransform)) != null)
            {
                return new SkewTransform
                {
                    AngleX = skewTransform.AngleX,
                    AngleY = skewTransform.AngleY,
                    CenterX = skewTransform.CenterX,
                    CenterY = skewTransform.CenterY
                };
            }
            TranslateTransform translateTransform;
            if ((translateTransform = (transform as TranslateTransform)) != null)
            {
                return new TranslateTransform
                {
                    X = translateTransform.X,
                    Y = translateTransform.Y
                };
            }
            MatrixTransform matrixTransform;
            if ((matrixTransform = (transform as MatrixTransform)) != null)
            {
                return new MatrixTransform
                {
                    Matrix = matrixTransform.Matrix
                };
            }
            TransformGroup transformGroup;
            if ((transformGroup = (transform as TransformGroup)) != null)
            {
                TransformGroup transformGroup2 = new TransformGroup();
                foreach (Transform current in transformGroup.Children)
                {
                    transformGroup2.Children.Add(MouseDragElementBehavior.CloneTransform(current));
                }
                return transformGroup2;
            }
            return null;
        }
        private void UpdatePosition()
        {
            GeneralTransform transform = base.AssociatedObject.TransformToVisual(this.RootElement);
            Point transformOffset = MouseDragElementBehavior.GetTransformOffset(transform);
            this.X = transformOffset.X;
            this.Y = transformOffset.Y;
        }
        internal void StartDrag(Point positionInElementCoordinates)
        {
            this.relativePosition = positionInElementCoordinates;
            base.AssociatedObject.CaptureMouse();
            base.AssociatedObject.MouseMove += new MouseEventHandler(this.OnMouseMove);
            base.AssociatedObject.LostMouseCapture += new MouseEventHandler(this.OnLostMouseCapture);
            base.AssociatedObject.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.OnMouseLeftButtonUp), false);
        }
        internal void HandleDrag(Point newPositionInElementCoordinates)
        {
            double x = newPositionInElementCoordinates.X - this.relativePosition.X;
            double y = newPositionInElementCoordinates.Y - this.relativePosition.Y;
            GeneralTransform transform = base.AssociatedObject.TransformToVisual(this.RootElement);
            Point point = MouseDragElementBehavior.TransformAsVector(transform, x, y);
            this.settingPosition = true;
            this.ApplyTranslation(point.X, point.Y);
            this.UpdatePosition();
            this.settingPosition = false;
        }
        internal void EndDrag()
        {
            base.AssociatedObject.MouseMove -= new MouseEventHandler(this.OnMouseMove);
            base.AssociatedObject.LostMouseCapture -= new MouseEventHandler(this.OnLostMouseCapture);
            base.AssociatedObject.RemoveHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.OnMouseLeftButtonUp));
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.StartDrag(e.GetPosition(base.AssociatedObject));
            if (this.DragBegun != null)
            {
                this.DragBegun(this, e);
            }
        }
        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            this.EndDrag();
            if (this.DragFinished != null)
            {
                this.DragFinished(this, e);
            }
        }
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //base.AssociatedObject.ReleaseMouseCapture();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (base.AssociatedObject != null)
            {
                base.AssociatedObject.ReleaseMouseCapture();
            }
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            this.HandleDrag(e.GetPosition(base.AssociatedObject));
            if (this.Dragging != null)
            {
                this.Dragging(this, e);
            }
        }
        private static bool RectContainsRect(Rect rect1, Rect rect2)
        {
            return !rect1.IsEmpty && !rect2.IsEmpty && (rect1.X <= rect2.X && rect1.Y <= rect2.Y && rect1.X + rect1.Width >= rect2.X + rect2.Width) && rect1.Y + rect1.Height >= rect2.Y + rect2.Height;
        }
        private static Point TransformAsVector(GeneralTransform transform, double x, double y)
        {
            Point point = transform.Transform(new Point(0.0, 0.0));
            Point point2 = transform.Transform(new Point(x, y));
            return new Point(point2.X - point.X, point2.Y - point.Y);
        }
        private static Point GetTransformOffset(GeneralTransform transform)
        {
            return transform.Transform(new Point(0.0, 0.0));
        }
        protected override void OnAttached()
        {
            base.AssociatedObject.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OnMouseLeftButtonDown), false);
        }
        protected override void OnDetaching()
        {
            base.AssociatedObject.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OnMouseLeftButtonDown));
        }
    }
}
