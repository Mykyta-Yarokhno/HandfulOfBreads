using HandfulOfBreads.ViewModels;

namespace HandfulOfBreads.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _viewModel;
        private const int PixelSize = 40;
        private double scale;
        private double minScale;

        public MainPage(int columns, int rows, string selectedPattern, List<List<Color>>? grid = null)
        {
            InitializeComponent();

            Shell.SetNavBarIsVisible(this, false);

            _viewModel = new MainPageViewModel(columns, rows, selectedPattern, grid);
            BindingContext = _viewModel;

            _viewModel.InvalidateRequested += OnInvalidateRequested;

            PixelGraphicsView.WidthRequest = columns * PixelSize;
            PixelGraphicsView.HeightRequest = selectedPattern == "Brick"
                ? rows * PixelSize + 20
                : rows * PixelSize;

            PixelGraphicsViewContainer.WidthRequest = columns * PixelSize * 10;
            PixelGraphicsViewContainer.HeightRequest = rows * PixelSize * 10;
        }

        private void OnInvalidateRequested()
        {
            PixelGraphicsView.Invalidate();
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
        private Point _startPosition1, _startPosition2;

        private bool _isDrawing;
        private bool _isDragging;

        private float _initialDistance;
        private double _initialScale;

        private void OnStartInteraction(object sender, TouchEventArgs e)
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

                //
                _initialDistance = Distance(_startPosition1, _startPosition2);
                _initialScale = PixelGraphicsView.Scale;
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

                //
                float currentDistance = Distance(currentTouch1, currentTouch2);

                if (_initialDistance > 0)
                {
                    double scaleFactor = currentDistance / _initialDistance;
                    PixelGraphicsView.Scale = _initialScale * scaleFactor;

                    PixelGraphicsView.Scale = Math.Clamp(PixelGraphicsView.Scale, 0.5, 5.0);
                }
            }
        }

        private float Distance(PointF p1, PointF p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
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
                    _viewModel.CurrentPattern.TogglePixel(interpolatedX, interpolatedY);
                }
            }

            _viewModel.CurrentPattern.TogglePixel(touchPoint.X, touchPoint.Y);
            PixelGraphicsView.Invalidate();

            _previousTouchPoint = touchPoint;
        }

        private void OnEndInteraction(object sender, TouchEventArgs e)
        {
            if (_isDrawing)
                _viewModel.CurrentPattern.TogglePixel(_onePoint.X, _onePoint.Y);

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
            PixelGraphicsView.TranslationX = 0;
            PixelGraphicsView.TranslationY = 0;

            //Dispatcher.Dispatch(() => CanvasScroll.ScrollToAsync(PixelCanvas, ScrollToPosition.Center, false));
        }

    }
}
