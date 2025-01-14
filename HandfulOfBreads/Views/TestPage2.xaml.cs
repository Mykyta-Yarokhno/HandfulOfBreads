namespace HandfulOfBreads.Views;

public partial class TestPage2 : ContentPage
{
    private readonly PixelGridDrawable _drawable = new();
    private const int PixelSize = 40;
    private bool _isDrawing;
    private bool _isDragging;
    private Point _startPosition1, _startPosition2;

    public IDrawable Drawable { get; }

    public TestPage2()
    {
        InitializeComponent();
        Drawable = _drawable;
        BindingContext = this;

        _drawable.InitializeGrid(15, 15, PixelSize);

        PixelGraphicsView.WidthRequest = 15 * PixelSize;
        PixelGraphicsView.HeightRequest = 15 * PixelSize;

        PixelGraphicsViewContainer.WidthRequest = 15 * PixelSize * 2;
        PixelGraphicsViewContainer.HeightRequest = 15 * PixelSize * 2;
    }

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

            _startPosition1 = e.Touches[0];
            _startPosition2 = e.Touches[1];
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
            var currentTouch1 = e.Touches[0];
            var currentTouch2 = e.Touches[1];

            double avgOffsetX = ((currentTouch1.X - _startPosition1.X) + (currentTouch2.X - _startPosition2.X)) / 2;
            double avgOffsetY = ((currentTouch1.Y - _startPosition1.Y) + (currentTouch2.Y - _startPosition2.Y)) / 2;

            PixelGraphicsView.TranslationX += avgOffsetX;
            PixelGraphicsView.TranslationY += avgOffsetY;
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

    private bool _isPanelVisible = false;
    private async void OnToggleClicked(object sender, EventArgs e)
    {
        if (_isPanelVisible)
        {
            await SidePanel.TranslateTo(-SidePanel.Width, 0, 250);
            SidePanel.WidthRequest = 0;
        }
        else
        {
            SidePanel.WidthRequest = 50;
            await SidePanel.TranslateTo(0, 0, 250);
        }

        _isPanelVisible = !_isPanelVisible;
    }
}
