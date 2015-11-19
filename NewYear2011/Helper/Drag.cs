using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace NewYear2011.Helper
{
    public class Drag : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty IsMovableProperty =
            DependencyProperty.Register("IsMovable", typeof(bool),
                                        typeof(Drag), new PropertyMetadata(null));

        [Category("Target Properties")]
        public bool IsMovable { get; set; }

        private bool _isDragging = false;
        private Point _offset;
        private readonly TranslateTransform _elementTranslate = new TranslateTransform();
        private TranslateTransform _imgTranslate = new TranslateTransform();        

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObjectLoaded;
            AssociatedObject.HorizontalAlignment = HorizontalAlignment.Left;
            AssociatedObject.VerticalAlignment = VerticalAlignment.Top;
            AssociatedObject.MouseLeftButtonDown += AssociatedObjectMouseLeftButtonDown;
        }

        void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            //AssociatedObject.RenderTransform = _elementTranslate;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= AssociatedObjectMouseLeftButtonDown;
        }

        private void AssociatedObjectMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_isDragging) return;
            FrameworkElement parent = AssociatedObject.Parent as FrameworkElement;
            Point newPosition = e.GetPosition(parent);

            // Move the dragon via the new position less the offset            
            (AssociatedObject.RenderTransform as TranslateTransform).X = (newPosition.X - _offset.X);
            (AssociatedObject.RenderTransform as TranslateTransform).Y = (newPosition.Y - _offset.Y);            
        }

        private void AssociatedObjectMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!_isDragging) return;
            Panel panel = AssociatedObject.Parent as Panel;

            // turn off drag
            _isDragging = false;

            // Free the Mouse            
            AssociatedObject.ReleaseMouseCapture();
            AssociatedObject.MouseMove -= AssociatedObjectMouseMove;
            AssociatedObject.MouseLeftButtonUp -= AssociatedObjectMouseLeftButtonUp;
            
            if (IsMovable)
            {
                _elementTranslate.X = _imgTranslate.X;
                _elementTranslate.Y = _imgTranslate.Y;
            }
            _imgTranslate = new TranslateTransform();
            _offset = new Point(0, 0);
            AssociatedObject.Opacity = 1;            
        }



        private void AssociatedObjectMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDragging = true;

            AssociatedObject.Opacity = .35;

            
            AssociatedObject.CaptureMouse();
            AssociatedObject.MouseMove +=AssociatedObjectMouseMove;
            AssociatedObject.MouseLeftButtonUp += AssociatedObjectMouseLeftButtonUp;
            _offset = e.GetPosition(AssociatedObject);            
        }
    }
}
