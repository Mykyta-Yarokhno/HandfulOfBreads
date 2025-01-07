namespace HandfulOfBreads.Views;

public partial class TestPage2 : ContentPage
{
    private readonly PixelGridDrawable _drawable = new();
    private const int PixelSize = 40;
    private bool _isDrawing;
    private bool _isDragging;
    private Point _lastPosition;
    private double _initialX, _initialY;

    public TestPage2()
    {
        InitializeComponent();
        Drawable = _drawable;
        BindingContext = this;

        _drawable.InitializeGrid(15, 15, PixelSize);

        PixelGraphicsView.WidthRequest = 15 * PixelSize;
        PixelGraphicsView.HeightRequest = 15 * PixelSize;
    }

    public IDrawable Drawable { get; }

    private void OnStartInteraction(object sender, TouchEventArgs e)
    {
        if (e.Touches.Count() == 1) 
        {
            _isDrawing = true;
            HandleInteraction(e.Touches[0]);
        }
        else if (e.Touches.Count() == 2)
        {
            _isDragging = true;
            _lastPosition = e.Touches[0];
            _initialX = AbsoluteLayout.GetLayoutBounds(PixelGraphicsView).X;
            _initialY = AbsoluteLayout.GetLayoutBounds(PixelGraphicsView).Y;
        }
    }

    private void OnDragInteraction(object sender, TouchEventArgs e)
    {
        if (_isDrawing && e.Touches.Count() == 1)
        {
            HandleInteraction(e.Touches[0]);
        }
        else if (_isDragging && e.Touches.Count() == 2)
        {
            var touchPoint = e.Touches[0];
            double offsetX = touchPoint.X - _lastPosition.X;
            double offsetY = touchPoint.Y - _lastPosition.Y;

            AbsoluteLayout.SetLayoutBounds(PixelGraphicsView, new Rect(_initialX + offsetX, _initialY + offsetY, PixelGraphicsView.Width, PixelGraphicsView.Height));
        }
    }

    private void HandleInteraction(PointF touchPoint)
    {
        _drawable.TogglePixel(touchPoint.X, touchPoint.Y);
        PixelGraphicsView.Invalidate();
    }

    private void OnEndInteraction(object sender, TouchEventArgs e)
    {
        _isDragging = false;
        _isDrawing = false;
    }
}
