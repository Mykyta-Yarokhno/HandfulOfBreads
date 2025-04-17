using CommunityToolkit.Maui.Views;
using HandfulOfBreads.Graphics.DrawablePatterns;
using HandfulOfBreads.Services;
using HandfulOfBreads.Views.Popups;
using Microsoft.Maui.Graphics.Platform;
using SkiaSharp;
using System.Reflection;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace HandfulOfBreads.Views;

public partial class MainPage : ContentPage
{
    private IPatternDrawable _currentPattern;
    private const int PixelSize = 40;
    private bool _isDrawing;
    private bool _isDragging;
    private Point _startPosition1, _startPosition2;
    private double scale;
    private double minScale;

    private readonly GridSavingService _imageSavingService = new GridSavingService();

    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

    public IDrawable Drawable { get; }

    private List<List<Color>> _grid = null;

    public MainPage(int columns, int rows, string selectedPattern, List<List<Color>> grid = null)
    {
        InitializeComponent();

        Shell.SetNavBarIsVisible(this, false);

        if (grid != null)
        {
            _grid = grid;
            _currentPattern = new LoomPatternDrawable();
            PixelGraphicsView.WidthRequest = columns * PixelSize;
            PixelGraphicsView.HeightRequest = rows * PixelSize;
        }
        else if (selectedPattern == "Loom")
        {
            _currentPattern = new LoomPatternDrawable();

            PixelGraphicsView.WidthRequest = columns * PixelSize;
            PixelGraphicsView.HeightRequest = rows * PixelSize;
        }
        else if (selectedPattern == "Brick")
        {
            _currentPattern = new BrickPatternDrawable();

            PixelGraphicsView.WidthRequest = columns * PixelSize;
            PixelGraphicsView.HeightRequest = rows * PixelSize + 20;
        }
        //else if (selectedPattern == "Payote")
        //{
        //    _currentPattern = new PayotePatternDrawable();
        //}

        Drawable = _currentPattern;
        BindingContext = this;

        InitializeDrawable(rows, columns);



        PixelGraphicsViewContainer.WidthRequest = columns * PixelSize * 10;
        PixelGraphicsViewContainer.HeightRequest = rows * PixelSize * 10;
    }

    private void InitializeDrawable(int rows, int columns)
    {
        ImageSource imageSource = ImageSource.FromFile("bonk.png");

        var image = LoadImageAsIImage();

        if (_grid != null)
        {
            _currentPattern.InitializeGrid(rows, columns, PixelSize, image, _grid);
        }
        else
        {
            _currentPattern.InitializeGrid(rows, columns, PixelSize, image);
        }
        

    }

    private IImage? LoadImageAsIImage()
    {
        IImage image;

        Assembly assembly = GetType().GetTypeInfo().Assembly;

        using (Stream stream = assembly.GetManifestResourceStream("HandfulOfBreads.Resources.Images.bonk.png"))
        {

            image = PlatformImage.FromStream(stream);
        }

        return image;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        minScale = Math.Min(width / PixelGraphicsView.Width, height / PixelGraphicsView.Height);

        //scale = minScale;
        scale = 0.5;

        //PixelGraphicsViewContainer.Scale = minScale;
        PixelGraphicsViewContainer.Scale = scale;
    }

    private PointF? _previousTouchPoint = null;

    private PointF _onePoint;

    private  void OnStartInteraction(object sender, TouchEventArgs e)
    {
        if (e.Touches.Count() == 1)
        {
            _isDrawing = true;
            _previousTouchPoint = null;

            _onePoint = e.Touches[0];
        }
        else if (e.Touches.Count() == 2)
        {
            _isDragging = true;
            _isDrawing = false;

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
        if (_previousTouchPoint.HasValue)
        {
            var previousPoint = _previousTouchPoint.Value;
            float deltaX = touchPoint.X - previousPoint.X;
            float deltaY = touchPoint.Y - previousPoint.Y;
            int steps = (int)Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));

            for (int i = 1; i <= steps; i++)
            {
                float interpolatedX = previousPoint.X + deltaX * i / steps;
                float interpolatedY = previousPoint.Y + deltaY * i / steps;
                _currentPattern.TogglePixel(interpolatedX, interpolatedY);
            }
        }

        _currentPattern.TogglePixel(touchPoint.X, touchPoint.Y);
        PixelGraphicsView.Invalidate();

        _previousTouchPoint = touchPoint;
    }

    private void OnEndInteraction(object sender, TouchEventArgs e)
    {
        if (_isDrawing)
            _currentPattern.TogglePixel(_onePoint.X, _onePoint.Y);

        _previousTouchPoint = null;
        _isDrawing = false;
        PixelGraphicsView.Invalidate();
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

    private void OnZoomChanged(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.Text == "+")
            scale *= 1.1;
        else if (button.Text == "-")
            scale /= 1.1;

        //scale = Math.Max(minScale, Math.Min(scale, 1.0));

        PixelGraphicsViewContainer.Scale = scale;
    }

    private void OnMinimum(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        scale = minScale;
        PixelGraphicsViewContainer.Scale = minScale;
    }

    private void OnCenter(object sender, EventArgs e)
    {
        CenterPixelCanvas();
    }

    private void CenterPixelCanvas()
    {
        PixelGraphicsView.TranslationX = 0;
        PixelGraphicsView.TranslationY = 0;

        //Dispatcher.Dispatch(() => CanvasScroll.ScrollToAsync(PixelCanvas, ScrollToPosition.Center, false));
    }

    public async void OnSaveClicked(object sender, EventArgs e)
    {
        await _imageSavingService.SaveImageToGalleryAsync(_currentPattern);
    }

    private async void OnOpenColorPickerClicked(object sender, EventArgs e)
    {
        var colorPickerPopup = new ColorPickerPopup();
        var result = await this.ShowPopupAsync(colorPickerPopup);

        if (result is Color selectedColor)
        {
            _currentPattern.SelectedColor = selectedColor;
            OpenColorPickerButton.BackgroundColor = selectedColor;
        }
    }

    private async void OnNewButtonClicked(object sender, EventArgs e)
    {
        var result = new NewGraphicsViewPopup();
        await this.ShowPopupAsync(result);

        InitializeDrawable(result.FirstNumber, result.SecondNumber);
    }


    public bool IsBeadingActive { get; set; } = false;

    private void OnStartBeadingClicked(object sender, EventArgs e)
    {
        IsBeadingActive = true;

        OnPropertyChanged(nameof(IsBeadingActive));

        MoveRow(0);
    }

    private void OnMoveRowUpClicked(object sender, EventArgs e) => MoveRow(-1);
    private void OnMoveRowDownClicked(object sender, EventArgs e) => MoveRow(1);

    private void MoveRow(int direction)
    {
        _currentPattern.HighlightRow(direction);
        PixelGraphicsView.Invalidate();
    }

}

