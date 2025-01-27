﻿using CommunityToolkit.Maui.Views;
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

    private readonly ImageSavingService _imageSavingService = new ImageSavingService();

    public IDrawable Drawable { get; }

    public MainPage(int firstNumber, int secondNumber, string selectedPattern)
    {
        InitializeComponent();

        if (selectedPattern == "Loom")
        {
            _currentPattern = new LoomPatternDrawable();

            PixelGraphicsView.WidthRequest = firstNumber * PixelSize;
            PixelGraphicsView.HeightRequest = secondNumber * PixelSize;
        }
        else if (selectedPattern == "Brick")
        {
            _currentPattern = new BrickPatternDrawable();

            PixelGraphicsView.WidthRequest = firstNumber * PixelSize;
            PixelGraphicsView.HeightRequest = secondNumber * PixelSize + 20;
        }
        //else if (selectedPattern == "Payote")
        //{
        //    _currentPattern = new PayotePatternDrawable();
        //}

        Drawable = _currentPattern;
        BindingContext = this;

        InitializeDrawable(firstNumber, secondNumber);



        PixelGraphicsViewContainer.WidthRequest = firstNumber * PixelSize * 10;
        PixelGraphicsViewContainer.HeightRequest = secondNumber * PixelSize * 10;
    }

    private void InitializeDrawable(int firstNumber, int secondNumber)
    {
        ImageSource imageSource = ImageSource.FromFile("bonk.png");

        var image = LoadImageAsIImage();

        _currentPattern.InitializeGrid(secondNumber, firstNumber, PixelSize, image);

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

        scale = minScale;

        PixelGraphicsViewContainer.Scale = minScale;
    }

    private async void OnNewButtonClicked(object sender, EventArgs e)
    {
        var modalPage = new NewGraphicsViewPageModal();

        modalPage.Confirmed += OnModalConfirmed;

        await Navigation.PushModalAsync(modalPage);
    }

    private void OnModalConfirmed(object sender, (int firstNumber, int secondNumber) args)
    {
        InitializeDrawable(args.firstNumber, args.secondNumber);
    }

    private async void OnNavigateButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new TestPage2());
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
        _currentPattern.TogglePixel(touchPoint.X, touchPoint.Y);
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

        scale = Math.Max(minScale, Math.Min(scale, 1.0));

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
        _imageSavingService.SaveImageToGalleryAsync(_currentPattern);
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

}

