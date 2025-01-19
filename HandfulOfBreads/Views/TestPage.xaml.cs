using CommunityToolkit.Maui.Core;
using HandfulOfBreads.Graphics.DrawablePatterns;

namespace HandfulOfBreads.Views;
public partial class TestPage : ContentPage
{
    private readonly LoomPatternDrawable _drawable = new();
    private const int PixelSize = 20;
    private bool _isDrawing;

    public TestPage()
    {
        InitializeComponent();
        Drawable = _drawable;
        BindingContext = this;

        _drawable.InitializeGrid(32, 32, PixelSize);

    }

    public IDrawable Drawable { get; }

    private void OnStartInteraction(object sender, TouchEventArgs e)
    {
            _isDrawing = true;
            HandleInteraction(e.Touches[0]);
    }

    private void OnDragInteraction(object sender, TouchEventArgs e)
    {
        if (_isDrawing )
        {
            HandleInteraction(e.Touches[0]);
        }
    }

    private void HandleInteraction(PointF touchPoint)
    {
        _drawable.TogglePixel(touchPoint.X, touchPoint.Y);
        PixelGraphicsView.Invalidate();
    }
}